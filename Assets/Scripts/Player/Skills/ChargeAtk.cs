using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAtk : Skill
{
    private GameManager gameManager;
    private List<Enemy> enemiesDragged = new List<Enemy>();
    private Quaternion rotation = Quaternion.identity;
    private float currentZRot = 0;
    private float acceleration = 0;
    private float yDragVelocity = 5;
    private int direction; // 1 = right, -1 = left.
    private bool isActive = false;
    private bool buttonReleased = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    public override void OnSmash(Enemy enemy)
    {
        enemy.StopMovement();
        enemy.transform.SetParent(player.transform);
        enemiesDragged.Add(enemy);
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
                float yForce = yDragVelocity * -player.GetDeltaY() - player.rb.velocity.y;
                player.rb.AddForce(new Vector3(0, yForce, 0), ForceMode.VelocityChange);
            }
            currentZRot = Mathf.Clamp(currentZRot + 7.5f * -direction, -90, 90);
            rotation = Quaternion.Euler(0, 0, currentZRot);
            player.rb.MoveRotation(rotation);
            if (buttonReleased)
            {
                player.rb.AddForce(direction * 30 * acceleration, 0, 0);
                acceleration += 5;
            }
        }
    }

    public override void OnWallCollision(Collision collision)
    {
        if (buttonReleased)
        {
            foreach (Enemy enemy in enemiesDragged)
            {
                enemy.transform.SetParent(null);
                enemy.Squash();
            }
            enemiesDragged.Clear();
            collision.gameObject.GetComponent<Unpassable>().SlamWall(player.head.transform.position);
            player.CancelInv();
            CameraEffects.Shake(0.75f, 0.3f);
            Invoke("Uncharge", 0.75f);
        }
    }

    public override void OnMapBonusCollision(MapBonus mapBonus)
    {
        mapBonus.Despawn();
    }

    public override void OnInputRelease()
    {
        buttonReleased = true;
        player.rb.velocity = Vector3.zero;
        player.GainInv(2); //inv will break when colliding the wall
    }

    private void Uncharge()
    {
        player.animator.Play("Uncharged");
    }
}
