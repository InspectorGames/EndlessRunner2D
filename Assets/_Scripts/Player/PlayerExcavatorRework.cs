using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExcavatorRework : MonoBehaviour
{
    [SerializeField] private List<Wall> walls = new List<Wall>();
    [SerializeField] private float fallingDistance;
    [SerializeField] private float fallingSpeed;

    private int currentWall;
    private Vector2 lastPosition;
    private bool playerIsMoving;
    private float timer;

    private void Awake()
    {
        foreach(Wall wall in walls)
        {
            wall.InitializeWall();
        }
    }

    private void Update()
    {
        if(timer > 0.5f)
        {
            DamageCurrentWall();
            timer = 0;
        }

        if (playerIsMoving)
        {
            MovePlayer();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private bool DamageCurrentWall()
    {
        if (playerIsMoving) return false;

        if (currentWall >= walls.Count) return true;

        if(walls[currentWall].DamageWall() <= 0)
        {
            StartMovingPlayer();
            currentWall++;
        }
        return false;
    }

    private void StartMovingPlayer()
    {
        playerIsMoving = true;
        lastPosition = transform.position;
    }

    private void MovePlayer()
    {
        Vector2 nextPos = Vector2.MoveTowards(transform.position, transform.position + transform.right, fallingSpeed * Time.deltaTime);
        if (Vector2.Distance(nextPos, lastPosition) < fallingDistance)
        {
            transform.position = nextPos;
        }
        else playerIsMoving = false;
    }
}

[System.Serializable]
public class Wall
{
    [SerializeField] private int wallHp;
    private int currentHp;

    public void InitializeWall()
    {
        currentHp = wallHp;
    }

    public int DamageWall()
    {
        return --currentHp;
    }
}
