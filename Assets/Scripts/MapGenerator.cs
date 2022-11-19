using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapGenerator : MonoBehaviour
{

    private List<MapObstacleInfo> obstacles = new List<MapObstacleInfo>();
    private List<MapMFuelInfo> mFuel = new List<MapMFuelInfo>();
    private List<MapEFuelInfo> eFuel = new List<MapEFuelInfo>();


    [SerializeField] private int verticalMapSize; // Real Map Size
    [SerializeField] private int verticalValidMapSize; // Map size that player can play in

    [Space]
    
    [SerializeField] private Tilemap testTilemap;
    [SerializeField] private Tile obstacleTile;
    [SerializeField] private Tile mFuelTile;
    [SerializeField] private GameObject pfMFuel;
    [SerializeField] private Tile eFuelTile;
    [SerializeField] private GameObject pfEFuel;

    [Space]


    [Header("Generation Settings")]
    [SerializeField] private List<int> keyPositions;
    private bool haveKeyPos = false;

    [Space]

    [Header("Obstacle Generation Settings")]
    [SerializeField] private int maxObstacleSeparation;
    [SerializeField] private int minObstacleSeparation;
    [SerializeField] private int maxGroundObstacleHeight; //It will be rounded to pair
    [SerializeField] [Range(0f, 1f)] private float ceilObstacleProb;
    [SerializeField] [Range(0f, 1f)] private float airObstacleProb;

    [Space]

    [Header("Minecart Fuel Generation Settings")]
    [SerializeField] private int minMFuel;
    [SerializeField] private int fuelTrail;

    [Space]

    [Header("Excavator Fuel Generation Settings")]
    [SerializeField] private int minEFuel;

    public void GenerateMap(int horizontalMapSize)
    {
        testTilemap.ClearAllTiles();
        if (!haveKeyPos)
        {
            for (int i = 1; i < verticalValidMapSize; i += 2)
            {
                keyPositions.Add(i);
            }
            haveKeyPos = true;
        }

        Pass_Obstacles(horizontalMapSize);
        Pass_EFuel_Positions(horizontalMapSize);
        Pass_MFuel_EFuel(horizontalMapSize);
        Pass_EFuel_VPos();
        DrawMap();
    }

    private void DrawMap()
    {
        foreach(MapObstacleInfo obstacle in obstacles)
        {
            for (int i = 0; i < obstacle.VerticalSize; i++)
            {
                if (obstacle.GrowUp)
                {
                    if (obstacle.VerticalPosition + i > verticalMapSize) break;
                    testTilemap.SetTile(new Vector3Int(obstacle.HorizontalPosition, obstacle.VerticalPosition + i), obstacleTile);
                }
                else
                {
                    if (obstacle.VerticalPosition - i < 0) break;
                    testTilemap.SetTile(new Vector3Int(obstacle.HorizontalPosition, obstacle.VerticalPosition - i), obstacleTile);
                }
            }

        }
        
        foreach(MapMFuelInfo fuel in mFuel)
        {
            //testTilemap.SetTile(new Vector3Int(fuel.HorizontalPosition, fuel.VerticalPosition), mFuelTile);
            Instantiate(pfMFuel, new Vector3(fuel.HorizontalPosition + 0.5f, fuel.VerticalPosition + 0.5f), Quaternion.identity);
        }

        foreach (MapEFuelInfo fuel in eFuel)
        {
            //testTilemap.SetTile(new Vector3Int(fuel.HorizontalPosition, fuel.VerticalPosition), eFuelTile);
            Instantiate(pfEFuel, new Vector3(fuel.HorizontalPosition + 0.5f, fuel.VerticalPosition + 0.5f), Quaternion.identity);
        }
        obstacles.Clear();
        mFuel.Clear();
        eFuel.Clear();
    }

    private void Pass_Obstacles(int horizontalMapSize)
    {
        int horizontalPosition = Random.Range(minObstacleSeparation, maxObstacleSeparation);
        if (horizontalPosition % 2 != 0) horizontalPosition++;

        while(horizontalPosition < horizontalMapSize)
        {
            //By default is a Grounded Obstacle (Rooted)
            int startVPosition = 0;
            bool growUp = true;

            int height = keyPositions[Random.Range(0, keyPositions.Count - 1)] + 1;
            
            if(Random.Range(0f,1f) < airObstacleProb)
            {
                //Air Obstacle
                startVPosition = keyPositions[Random.Range(0, keyPositions.Count - 1)];
                int endingVerticalPos = keyPositions[Random.Range(0, keyPositions.Count - 1)];

                if (startVPosition == keyPositions[keyPositions.Count - 1])
                {
                    endingVerticalPos = keyPositions[Random.Range(1, keyPositions.Count - 1)];
                }
                else if(startVPosition == keyPositions[0])
                {
                    endingVerticalPos = keyPositions[Random.Range(0, keyPositions.Count - 2)];
                }

                if (startVPosition >= endingVerticalPos)
                {
                    height = startVPosition - endingVerticalPos + 1;
                    growUp = false;
                }
                else
                {
                    height = endingVerticalPos - startVPosition + 1;
                    growUp = true;
                }

            }
            else if (Random.Range(0f, 1f) < ceilObstacleProb)
            {
                //Ceiling Obstacle (Rooted)
                growUp = false;
                startVPosition = verticalMapSize;
                height += verticalMapSize - verticalValidMapSize;
            }

            MapObstacleInfo obstacle = new MapObstacleInfo(startVPosition, height, horizontalPosition, growUp);
            if (growUp)
            {
                for(int i = startVPosition; i < height; i++)
                {
                    if (keyPositions.Contains(i))
                    {
                        obstacle.KeyPos.Add(i);
                    }
                }
            }
            else
            {
                for (int i = height; i >= 0; i--)
                {
                    if (keyPositions.Contains(startVPosition - i))
                    {
                        obstacle.KeyPos.Add(startVPosition - i);
                    }
                }
            }

            obstacles.Add(obstacle);

            horizontalPosition += Random.Range(minObstacleSeparation, maxObstacleSeparation);
            if (horizontalPosition % 2 != 0) horizontalPosition++;
        }
    }

    private void Pass_EFuel_Positions(int horizontalMapSize)
    {
        int stepDistance = horizontalMapSize / minEFuel;

        int minDistance = 0;
        int maxDistance = stepDistance;
        for (int i = 0; i < minEFuel; i++)
        {
            int position = Random.Range(minDistance, maxDistance);
            if (position % 2 != 0) position++; 
            eFuel.Add(new MapEFuelInfo(-1, position));
            minDistance += stepDistance;
            maxDistance += stepDistance;
        }
    }

    private void Pass_MFuel_EFuel(int horizontalMapSize)
    {

        int stepDistance = horizontalMapSize / minMFuel;
        
        int minDistance = fuelTrail * 2;
        int maxDistance = stepDistance;

        int horizontalPosition = Random.Range(minDistance,maxDistance);

        if(horizontalPosition % 2 != 0) horizontalPosition++;

        while (horizontalPosition < horizontalMapSize)
        {
            int verticalPosition;

            List<int> validKeyPos = GetKeyPosAvaliableInPosition(horizontalPosition);
            int nextFuelVKPIndex = Random.Range(0, validKeyPos.Count - 1);
            verticalPosition = validKeyPos[nextFuelVKPIndex];

            mFuel.Add(new MapMFuelInfo(verticalPosition,  horizontalPosition)); //Head
            Pass_EFuel_Fix(horizontalPosition, verticalPosition);

            for (int i = 1; i <= fuelTrail; i++) // Trail
            {

                int desiredPosition = verticalPosition - 2;

                if(desiredPosition <= 0)
                {
                    desiredPosition = keyPositions[0];
                }

                validKeyPos = GetKeyPosAvaliableInPosition(horizontalPosition - (2 * i));

                if (!validKeyPos.Contains(desiredPosition))
                {
                    desiredPosition = validKeyPos[0];
                }

                if(i != 4)
                {
                    List<int> validKeyPosNext = GetKeyPosAvaliableInPosition(horizontalPosition - (2 * (i + 1)));

                    if(validKeyPosNext.Count != keyPositions.Count)
                    {
                        if(validKeyPosNext[0] > desiredPosition + 2)
                        {
                            bool checkUp = true;
                            do
                            {
                                if(checkUp)
                                {
                                    desiredPosition += 2;
                                    if(desiredPosition > verticalValidMapSize) checkUp = false;
                                }
                                else
                                {
                                    desiredPosition -= 2;
                                }
                            } while (!validKeyPos.Contains(desiredPosition));
                        }
                    }
                }

                verticalPosition = desiredPosition;

                mFuel.Add(new MapMFuelInfo(verticalPosition, horizontalPosition - (2 * i))); //Trails
            }

            minDistance += stepDistance;
            maxDistance += stepDistance;
            horizontalPosition = Random.Range(minDistance, maxDistance);
            if (horizontalPosition % 2 != 0) horizontalPosition++;
        }

    }

    private void Pass_EFuel_Fix(int xPos, int yPos)
    {

        for (int i = 0; i < eFuel.Count; i++) // Horizontal Position Correction
        {
            if (eFuel[i].HorizontalPosition >= xPos - (fuelTrail * 2) && eFuel[i].HorizontalPosition <= xPos)
            {
                eFuel[i].HorizontalPosition = xPos + 2;

                List<int> eKeyPosValid = GetKeyPosAvaliableInPosition(eFuel[i].HorizontalPosition);

                bool checkDown = true;
                int desiredPosition = yPos;
                do
                {
                    if (checkDown)
                    {
                        desiredPosition -= 2;
                        if (desiredPosition <= 0) checkDown = false;
                    }
                    else
                    {
                        desiredPosition += 2;
                    }
                } while (!eKeyPosValid.Contains(desiredPosition));

                eFuel[i].VerticalPosition = desiredPosition;
            }
        }
    }

    private void Pass_EFuel_VPos()
    {
        for (int i = 0; i < eFuel.Count; i++)
        {
            if(eFuel[i].VerticalPosition < 0)
            {
                List<int> eKeyPosValid = GetKeyPosAvaliableInPosition(eFuel[i].HorizontalPosition);
                eFuel[i].VerticalPosition = eKeyPosValid[Random.Range(0, eKeyPosValid.Count - 1)];
            }
        }
    }

    private List<int> GetKeyPosAvaliableInPosition(int xPosition)
    {
        List<int> keyPos = new List<int>(keyPositions);
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (obstacles[i].HorizontalPosition == xPosition)
            {
                for(int j = 0; j < obstacles[i].KeyPos.Count; j++)
                {
                    keyPos.Remove(obstacles[i].KeyPos[j]);
                }
                break;
            }
        }
        return keyPos;
    }

}
