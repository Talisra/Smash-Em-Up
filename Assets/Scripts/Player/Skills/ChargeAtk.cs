using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAtk : Skill
{
    private GameManager gameManager;
    private Quaternion rotation = Quaternion.identity;
    private float currentZRot = 0;
    private float acceleration = 0;
    private int direction; // 1 = right, -1 = left.
    private bool isActive = false;
    private bool buttonReleased = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    public override void OnSmash(Enemy enemy)
    {
        if (enemy.transform.position.x < (gameManager.GetGameArea()[0] + enemy.GetComponent<Collider>().bounds.size.x*1.5f)
            ||
            enemy.transform.position.x > (gameManager.GetGameArea()[2] - enemy.GetComponent<Collider>().bounds.size.x*1.5f))
        {
            enemy.Squash();
        }
        else
        {
            if (!enemy.isHit)
            {
                enemy.Damage(1);
                enemy.HitByPlayer();
                enemy.GetComponent<Rigidbody>().AddForce(new Vector3(direction * 6000, 0, 0));
                ShowHitParticle(enemy.transform);
                AudioManager.Instance.Play("ChargeHit");
            }
        }
    }

    public override void OnStartAction()
    {
        isActive = true;
        direction = player.rb.velocity.x > 0 ? 1 : -1;
        player.animator.Play("Charge");
    }
    public override void OnEndAction()
    {
        isActive = false;
        currentZRot = 0;
        acceleration = 0;
        rotation = Quaternion.identity;
        buttonReleased = false;
        player.GiveControl();
    }

    private int CheckDirection()
    {
        Vector2 rawMousePosition = Input.mousePosition;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(rawMousePosition.x, rawMousePosition.y, -Camera.main.transform.position.z));
        return (mousePosition.x - player.transform.position.x > 0) ? 1 : -1;
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            if (!buttonReleased)
            {
                direction = CheckDirection();
            }
            currentZRot = Mathf.Clamp(currentZRot + 7.5f * -direction, -90, 90);
            rotation = Quaternion.Euler(0, 0, currentZRot);
            player.rb.MoveRotation(rotation);
            if (buttonReleased)
            {
                player.rb.AddForce(direction * 10 * acceleration, 0, 0);
                acceleration += 10;
            }
        }
    }

    public override void OnWallCollision(Collision collision)
    {
        collision.gameObject.GetComponent<Unpassable>().SlamWall(player.head.transform.position);
        player.CancelInv();
        CameraShake.Shake(0.75f, 0.3f);
        Invoke("Uncharge", 0.75f);
    }

    public override void OnInputRelease()
    {
        Debug.Log("asdf");
        buttonReleased = true;
        player.GainInv(2); //inv will break when colliding the wall
    }

    private void Uncharge()
    {
        player.animator.Play("Uncharged");
    }
}
