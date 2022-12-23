using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerExcavator : MonoBehaviour
{
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private int excavationSize;
    [SerializeField] private int offset;
    [SerializeField] private GameObject excavationEffect;
    private bool canExcavate = true;

    private void Update()
    {
        if (canExcavate)
        {
            for(int y = 0; y < excavationSize; y++)
            {
                wallTilemap.SetTile(new Vector3Int((int) transform.position.x + offset, y), null);
            }
        }
    }

    public void Excavate(bool can)
    {
        canExcavate = can;
        excavationEffect.SetActive(can);
    }
}
