using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile testObstacle;
    [SerializeField] private int maxSeparation;
    [SerializeField] private int minSeparation;
    private int initialX = 0;
    private bool[] map;

    private void Start()
    {
        map = new bool[100];
        int xPos = initialX;

        while (xPos < initialX + 100)
        {
            xPos += Random.Range(minSeparation, maxSeparation);
            map[xPos] = true;
            tilemap.SetTile(new Vector3Int(xPos, 0), testObstacle);
        }
    }

}
