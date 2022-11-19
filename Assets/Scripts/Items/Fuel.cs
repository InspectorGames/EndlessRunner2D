using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    public delegate void TakeExcFuelAction();
    public static event TakeExcFuelAction TakeExcFuel;

    public delegate void TakeMinecartFuelAction();
    public static event TakeMinecartFuelAction TakeMinecFuel;

    public enum FuelType
    {
        Minecart,
        Excavator
    }

    [SerializeField] private FuelType type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (type)
            {
                case FuelType.Minecart:
                    TakeMinecFuel();
                    break;
                case FuelType.Excavator:
                    TakeExcFuel();
                    break;
            }
            
            Destroy(gameObject);
        }
    }
}
