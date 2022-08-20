/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using UnityEngine;

public class Spaceship : Enemy
{
    public GameObject spawnPosition1Object;
    public GameObject spawnPosition2Object;
    private Vector2 targetPosition;
    private bool isActive;

    void Start()
    {
        Inactivate();
    }

    void Update()
    {
        if (isActive)
        {
            transform.Translate(movementSpeed * new Vector2(xDirection, 0) * Time.deltaTime);
            if ((xDirection == 1 && transform.position.x >= targetPosition.x)
            || (xDirection == -1 && transform.position.x <= targetPosition.x))
            {
                Inactivate();
            }
        }
    }

    private void Inactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public void Spawn()
    {
        int positionRandom = Random.Range(0, 2);
        Vector2 spawnPosition1 = spawnPosition1Object.transform.position;
        Vector2 spawnPosition2 = spawnPosition2Object.transform.position;
        if (positionRandom % 1 == 0)
        {
            transform.position = spawnPosition1;
            targetPosition = spawnPosition2;
            xDirection = 1;
        }
        else
        {
            transform.position = spawnPosition2;
            targetPosition = spawnPosition1;
            xDirection = -1;
        }
        isActive = true;
    }

    public bool IsActive()
    {
        return isActive;
    }
}