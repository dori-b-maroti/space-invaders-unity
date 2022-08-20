/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using UnityEngine;
using UnityEngine.Events;

public class Alien : Enemy
{
    public UnityEvent<Alien> alienDied;

    private Alien topNeighbour = null;
    private uint columnIndex;
    private int shootProbability = 80;

    public override void Shoot()
    {
        if (Time.time >= lastShootTime + shootCooldown)
        {
            int shootRandom = Random.Range(0, 100);
            if (shootRandom >= shootProbability)
            {
                lastShootTime = Time.time;
                base.Shoot();
            }
        }
    }

    public void SetTopNeighbour(Alien alien)
    {
        topNeighbour = alien;
    }

    public Alien GetTopNeighbour()
    {
        return topNeighbour;
    }

    public void SetColumnIndex(uint index)
    {
        columnIndex = index;
    }

    public uint GetColumnIndex()
    {
        return columnIndex;
    }

    public void Move(float movementUnit)
    {
        transform.position = GetNextPosition(movementUnit);
    }

    public Vector2 GetNextPosition(float movementUnit)
    {
        Vector2 currentPosition = transform.position;
        return new Vector2(currentPosition.x + (movementSpeed * xDirection * movementUnit / 2), currentPosition.y);
    }

    private void ChangeDirection()
    {
        xDirection *= -1;
    }

    public void Drop(float dropUnit)
    {
        Vector2 currentPosition = transform.position;
        transform.position = new Vector2(currentPosition.x, currentPosition.y - dropUnit);
        ChangeDirection();
    }

    public void IncreaseSpeed(float speedIncrease)
    {
        movementSpeed += speedIncrease;
    }

    public void IncreaseShootProbability(int shootProbablityIncrease)
    {
        shootProbability -= shootProbablityIncrease;
    }

    public override void Die()
    {
        base.Die();
        gameObject.SetActive(false);
        alienDied.Invoke(this);
    }
}