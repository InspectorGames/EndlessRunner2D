using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineEffect : MonoBehaviour
{
    private Transform player;

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    private void Update()
    {
        transform.localScale = new Vector3(player.transform.position.x - transform.position.x + 2, transform.localScale.y);
    }
}
