/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using UnityEngine;

public class Projectile : Actor
{
    public bool isOwnedByPlayer;

    private int yDirection = -1;

    void Start()
    {
        if (isOwnedByPlayer)
        {
            yDirection = 1;
        }
    }

    void Update()
    {
        transform.Translate(movementSpeed * new Vector2(0, yDirection) * Time.deltaTime);
        if ((transform.position.y < Game.gameFieldBottomBoundary.y) || (transform.position.y > Game.gameFieldTopBoundary.y))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBlock"))
        {
            Destroy(gameObject);
        }
        else if ((other.CompareTag("Player") && !isOwnedByPlayer)
            || (other.CompareTag("Enemy") && isOwnedByPlayer))
        {
            Character characterToAttack = other.gameObject.GetComponent<Character>();
            if (characterToAttack)
            {
                Destroy(gameObject);
                characterToAttack.Die();
            }
        }
    }
}