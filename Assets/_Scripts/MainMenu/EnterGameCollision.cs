using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;

public class EnterGameCollision : MonoBehaviour
{
    [SerializeField] private SignalSender enterGameSignal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enterGameSignal.SendSignal();
        }
    }
}
