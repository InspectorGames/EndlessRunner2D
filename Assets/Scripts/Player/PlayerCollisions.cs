using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    public delegate void HitObstacleAction();
    public static event HitObstacleAction HitObstacle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            HitObstacle?.Invoke();
        }
    }
}
