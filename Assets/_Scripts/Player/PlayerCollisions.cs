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
                collision.GetComponentInChildren<Animator>().Play("Hit");
                AudioManager.instance.PlaySFX("hit2");
                collision.tag = "HittedObstacle";
                break;
            case "MFuel":
                EventManager.OnMinecartFuelCollected();
                collision.gameObject.SetActive(false);
                break;
            case "EFuel":
                EventManager.OnExcavatorFuelCollected();
                collision.gameObject.SetActive(false);  
                break;
            case "Excavator":
                EventManager.OnPlayerGetExcavator();
                collision.gameObject.SetActive(false);
                break;
        }
    }
}
