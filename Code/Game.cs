/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public Player player;
    public Spaceship spaceShip;
    public Text livesLeftText;
    public Text score;
    public Text winText;
    public Text gameOverText;
    public List<Image> livesLeftImages;
    public static Vector2 gameFieldTopBoundary;
    public static Vector2 gameFieldBottomBoundary;
    public float alienMoveCooldown;

    private List<Alien> aliens = new List<Alien>();
    private Dictionary<uint, Alien> bottomAliens = new Dictionary<uint, Alien>();
    private uint aliensAliveAmount;
    private bool isPaused = false;
    private bool isPlayerRespawning = false;
    private uint currentScore = 0;
    private uint livesLeft = 3;
    private float alienLastMoveTime;
    private float playerLastDeathTime;
    private const float kAlienMoveCooldownDecrease = 0.15f;
    private const float kRespawnPlayerCooldown = 1f;
    private const uint kColumnsAmount = 11;
    private const uint kRowsAmount = 5;
    private const int kGameOverLineYValue = -2;
    private const float kAlienDropUnit = 0.25f;
    private const float kAlienMovementUnit = 0.125f;
    private const float kAlienSpeedIncrease = 0.1f;
    private const int kAlienShootProbablityIncrease = 5;

    void Start()
    {
        Cursor.visible = false;
        winText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        player.died.AddListener(OnPlayerDied);
        gameFieldBottomBoundary = GetComponent<BoxCollider2D>().bounds.min;
        gameFieldTopBoundary = GetComponent<BoxCollider2D>().bounds.max;

        GameObject enemiesParent = GameObject.Find("Enemies");
        for (uint i = 0; i < kRowsAmount; i++)
        {
            GameObject rowParent = GameObject.Find("Row" + (i + 1));
            if (!rowParent)
            {
                continue;
            }
            Transform transform = rowParent.transform;
            for (int j = 0, childCount = transform.childCount; j < childCount; j++)
            {
                Alien alien = transform.GetChild(j).GetComponent<Alien>();
                if (!alien)
                {
                    continue;
                }
                alien.increaseScore.AddListener(IncreaseScoreBy);
                alien.alienDied.AddListener(OnAlienDied);
                alien.SetColumnIndex((uint)j % (kColumnsAmount));
                aliens.Add(alien);
            }
        }
        aliensAliveAmount = (uint)aliens.Count;
        SetupAlienNeighbours();
    }

    private void SetupAlienNeighbours()
    {
        for (int i = 0, aliensAmount = aliens.Count; i < aliensAmount; i++)
        {
            Alien alien = aliens[i];
            if (i + kColumnsAmount < aliensAliveAmount)
            {
                int index = i + (int)kColumnsAmount;
                Alien alienBelow = aliens[i + (int)kColumnsAmount];
                alienBelow.SetTopNeighbour(alien);
            }
            else
            {
                bottomAliens[alien.GetColumnIndex()] = alien;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (aliensAliveAmount == 0)
        {
            Win();
        }

        if (Time.time < playerLastDeathTime + kRespawnPlayerCooldown)
        {
            return;
        }

        if (isPlayerRespawning)
        {
            RespawnPlayer();
            return;
        }

        if (isPaused)
        {
            return;
        }

        if (!spaceShip.IsActive())
        {
            SpawnSpaceship();
        }

        if (Time.time < alienLastMoveTime + alienMoveCooldown)
        {
            return;
        }

        MoveAliens();
        PerformBottomAliensShooting();
    }

    private void MoveAliens()
    {
        bool dropTriggered = false;
        alienLastMoveTime = Time.time;

        for (int i = 0, aliensAmount = aliens.Count; i < aliensAmount; i++)
        {
            Alien alien = aliens[i];
            if (!alien.gameObject.activeSelf)
            {
                continue;
            }
            alien.Move(kAlienMovementUnit);
            if (alien.transform.position.y <= kGameOverLineYValue)
            {
                Loose();
            }
            float alienNextXPosition = alien.GetNextPosition(kAlienMovementUnit).x;
            if ((alienNextXPosition >= gameFieldTopBoundary.x)
            || (alienNextXPosition <= gameFieldBottomBoundary.x))
            {
                dropTriggered = true;
            }
        }
        if (dropTriggered)
        {
            DropAliens();
            dropTriggered = false;
        }
    }

    private void PerformBottomAliensShooting()
    {
        for (uint i = 0; i < kColumnsAmount; i++)
        {
            Alien bottomAlien = bottomAliens[i];
            if (bottomAlien == null || bottomAlien.gameObject.activeSelf == false)
            {
                continue;
            }
            bottomAliens[i].Shoot();
        }
    }

    private void SpawnSpaceship()
    {
        int random = Random.Range(0, 10000);
        if (random == 9999)
        {
            spaceShip.gameObject.SetActive(true);
            spaceShip.Spawn();
        }
    }

    private void OnPlayerDied()
    {
        if (livesLeft-- == 0)
        {
            Loose();
        }
        else
        {
            isPaused = true;
            isPlayerRespawning = true;
            playerLastDeathTime = Time.time;
            livesLeftText.text = livesLeft.ToString();
            livesLeftImages[(int)livesLeft].gameObject.SetActive(false);
        }
    }

    private void RespawnPlayer()
    {
        isPlayerRespawning = false;
        isPaused = false;
        player.gameObject.SetActive(true);
        transform.position = new Vector2(0, 0);
    }

    private void DropAliens()
    {
        for (int i = 0, aliensAmount = aliens.Count; i < aliensAmount; i++)
        {
            Alien alien = aliens[i];
            alien.Drop(kAlienDropUnit);
            alien.IncreaseSpeed(kAlienSpeedIncrease);
            alien.IncreaseShootProbability(kAlienShootProbablityIncrease);
        }
        if (alienMoveCooldown - kAlienMoveCooldownDecrease > 0.0f)
        {
            alienMoveCooldown -= kAlienMoveCooldownDecrease;
        }
    }

    private void IncreaseScoreBy(uint scoreValue)
    {
        currentScore += scoreValue;
        score.text = currentScore.ToString("0000");
    }

    private void OnAlienDied(Alien alien)
    {
        --aliensAliveAmount;
        uint diedAlienColumnIndex = alien.GetColumnIndex();
        if (alien == bottomAliens[diedAlienColumnIndex])
        {
            Alien topNeighbour = alien.GetTopNeighbour();
            bottomAliens[diedAlienColumnIndex] = topNeighbour;
            while (topNeighbour && topNeighbour.gameObject.activeSelf == false)
            {
                bottomAliens[diedAlienColumnIndex] = topNeighbour;
                topNeighbour = topNeighbour.GetTopNeighbour();
            }
        }
    }

    private void EndGame()
    {
        isPaused = true;
        player.gameObject.SetActive(false);
        for (int i = 0, aliensAmount = aliens.Count; i < aliensAmount; i++)
        {
            Alien alien = aliens[i];
            alien.gameObject.SetActive(false);
        }
        spaceShip.gameObject.SetActive(false);
    }

    private void Win()
    {
        EndGame();
        winText.gameObject.SetActive(true);
    }

    private void Loose()
    {
        EndGame();
        gameOverText.gameObject.SetActive(true);
    }
}