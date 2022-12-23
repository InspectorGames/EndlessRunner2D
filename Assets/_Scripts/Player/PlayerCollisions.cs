using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Obstacle":
                EventManager.OnObstacleHitted();
                collision.gameObject.SetActive(false);
                break;
            case "MFuel":
                EventManager.OnMinecartFuelCollected();
                collision.gameObject.SetActive(false);
                break;
            case "EFuel":
                EventManager.OnExcavatorFuelCollected();
                collision.gameObject.SetActive(false);  
                break;
        }
    }
}
