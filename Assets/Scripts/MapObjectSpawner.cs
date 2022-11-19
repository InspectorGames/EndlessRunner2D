using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectSpawner : MonoBehaviour
{
    
    [Space]

    [SerializeField] private ObstacleSO[] topObstacles;
    [SerializeField] private ObstacleSO[] bottomObstacles;
    private int bannedIndex;

    [SerializeField] private int maxSeparation;
    [SerializeField] private int minSeparation;

    [Space]
    [SerializeField] private GameObject minecartFuel;
    [SerializeField] private GameObject excavatorFuel;

    private void Start()
    {
        SpawnMapObjects();
    }

    public void SpawnMapObjects()
    {
        int nextObstaclePosition = 10;
        int nextFuel = 20;

        for(int i = 0; i < 100; i += 2)
        {
            ObstacleSO.RootPosition randomRoot = (ObstacleSO.RootPosition) Random.Range(0, 2);

            if(i >= nextObstaclePosition)
            {
                randomRoot = SpawnObstacle(i).rootPosition;
                nextObstaclePosition += Random.Range(minSeparation, maxSeparation);
            }

            if(i >= nextFuel)
            {
                SpawnMinecartFuel(i, randomRoot);
                nextFuel += 20;
            }
        }
    }


    public ObstacleSO SpawnObstacle(int xPosition)
    {
        ObstacleSO obstacleToSpawn;

        //Check if spawns bottom or upper obstacle
        if (Random.Range(0f, 1f) >= 0.3f) // Spawns Bottom Obstacle
        {
            obstacleToSpawn = SelectRandomObstacle(ObstacleSO.RootPosition.Bottom);

            if(Random.Range(0f,1f) >= 0.8f) // Spawn Additive Up Obstacle
            {
                Instantiate(SelectRandomObstacle(ObstacleSO.RootPosition.Top).prefab, new Vector3(xPosition + 4 + 0.5f, 0.5f, 0), Quaternion.identity);
            }
        }
        else
        {
            obstacleToSpawn = SelectRandomObstacle(ObstacleSO.RootPosition.Top);
        }

        Instantiate(obstacleToSpawn.prefab, new Vector3(xPosition + 0.5f, 0.5f, 0), Quaternion.identity);
        return obstacleToSpawn;
    }

    private ObstacleSO SelectRandomObstacle(ObstacleSO.RootPosition rootPosition)
    {

        switch (rootPosition)
        {
            case ObstacleSO.RootPosition.Top:
                return topObstacles[Random.Range(0, topObstacles.Length)];
            default:
            case ObstacleSO.RootPosition.Bottom:
                return bottomObstacles[Random.Range(0, bottomObstacles.Length)];
        }
    }

    public void SpawnMinecartFuel(int xPosition, ObstacleSO.RootPosition bannedPos)
    {
        int randomPos;
        do
        {
            randomPos = Random.Range(0, 2);
        } while (randomPos == ((int)bannedPos));

        switch (randomPos)
        {
            case 0:
                Instantiate(minecartFuel, new Vector3(xPosition + 0.5f, 5 + 0.5f, 0), Quaternion.identity);
                break;
            case 1:
                Instantiate(minecartFuel, new Vector3(xPosition + 0.5f, 3 + 0.5f, 0), Quaternion.identity);
                break;
            case 2:
                Instantiate(minecartFuel, new Vector3(xPosition + 0.5f, 0 + 0.5f, 0), Quaternion.identity);
                break;
        }
    }
}
