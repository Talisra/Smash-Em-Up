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
            enemy.Damage(1);
            enemy.GetComponent<Rigidbody>().AddForce(new Vector3(direction * 6000, 0, 0));
            ShowHitParticle(enemy.transform);
            audioManager.Play("ChargeHit");
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
        player.GiveControl();
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            currentZRot -= 7.5f * direction;
            rotation = Quaternion.Euler(0, 0,
                Mathf.Clamp(
                    currentZRot,
                    -90, 90));
            player.rb.MoveRotation(rotation);
            player.rb.AddForce(direction * 10 * acceleration, 0, 0);
            acceleration += 10;
        }
    }

    public override void OnWallCollision(Collision collision)
    {
        collision.gameObject.GetComponent<Unpassable>().SlamWall(player.head.transform.position);
        CameraShake.Shake(0.75f, 0.3f);
        Invoke("Uncharge", 0.75f);
    }

    private void Uncharge()
    {
        player.animator.Play("Uncharged");
    }
}
