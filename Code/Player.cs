/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using UnityEngine;
using UnityEngine.Events;

public class Player : Character
{
    public UnityEvent died;

    void Start()
    {
        lastShootTime = Time.time;
    }

    void Update()
    {
        Vector2 directionFromInput = Vector2.zero;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            directionFromInput.x = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            directionFromInput.x = -1;
        }

        if (Time.time >= lastShootTime + shootCooldown)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                lastShootTime = Time.time;
                Shoot();
            }
        }
        Vector2 translation = movementSpeed * directionFromInput * Time.deltaTime;
        Vector2 position = transform.position;
        if ((position.x + translation.x < Game.gameFieldTopBoundary.x) && (position.x + translation.x > Game.gameFieldBottomBoundary.x))
        {
            transform.Translate(movementSpeed * directionFromInput * Time.deltaTime);
        }
    }

    public override void Die()
    {
        gameObject.SetActive(false);
        died.Invoke();
    }
}