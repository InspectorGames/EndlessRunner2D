using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapGenerator : MonoBehaviour
{

    private enum ObstacleType
    {
        Ground,
        Air,
        TopDown
    }

    private List<MapObstacleInfo> obstacles = new List<MapObstacleInfo>();
    private List<MapMFuelInfo> mFuel = new List<MapMFuelInfo>();
    private List<MapEFuelInfo> eFuel = new List<MapEFuelInfo>();
    private List<GameObject> mapObjects = new List<GameObject>();

    [Header("Obstacles")]
    [SerializeField] private List<ObstacleSO> groundObstacles;
    //[SerializeField] private List<ObstacleSO> airObstacles;
    [SerializeField] private List<ObstacleSO> topDownObstacles;

    [Space]

    [Header("Wall")]
    [SerializeField] private GameObject wallPf;
    private GameObject wallI;
    
    [Space]

    [Header("Map Settings")]
    [SerializeField] private int verticalMapSize;

    [Space]

    [Header("Generation")]
    [SerializeField] private GameObject pfMFuel;
    [SerializeField] private GameObject pfEFuel;
    [SerializeField] private GameObject excavatorItem;

    [Space]


    [Header("Generation Settings")]
    [SerializeField] private List<int> keyPositions;
    private bool haveKeyPos = false;

    [Space]

    [Header("Obstacle Generation Settings")]
    [SerializeField] private int maxObstacleSeparationRange;
    [SerializeField] private int minObstacleSeparationRange;
    [SerializeField] private int separationReduction;
    [SerializeField] private int maximumMaxObstacleSeparation;
    [SerializeField] private int maximumMinObstacleSeparation;
    [SerializeField] private int minimumMaxObstacleSeparation;
    [SerializeField] private int minimumMinObstacleSeparation;
    [SerializeField] private int maxGroundObstacleHeight; //It will be rounded to pair
    [SerializeField] [Range(0, 10)] private int groundObstacleProb;
    [SerializeField] [Range(0, 10)] private int topDownObstacleProb;
    //[SerializeField] [Range(0, 10)] private int airObstacleProb;

    [Space]

    [Header("Minecart Fuel Generation Settings")]
    [SerializeField] private int minMFuel;
    [SerializeField] private int fuelTrail;

    [Space]

    [Header("Excavator Fuel Generation Settings")]
    [SerializeField] private int minEFuel;
    [SerializeField] private int maxEFuel;
    private int currentEFuel;

    public void GenerateMap(int startingX, int trackLength, int excavatorPos, bool makeHarder = false)
    {
        //testTilemap.ClearAllTiles();
        ClearAllMapObjects();
        if (!haveKeyPos)
        {
            for (int i = 1; i < verticalMapSize; i += 2)
            {
                keyPositions.Add(i);
            }
            haveKeyPos = true;
        }

        if (makeHarder)
        {
            if(maxObstacleSeparationRange > minimumMaxObstacleSeparation)
            {
                maxObstacleSeparationRange -= separationReduction;
            }
            else
            {
                maxObstacleSeparationRange = minimumMaxObstacleSeparation;
            }

            if (minObstacleSeparationRange > minimumMinObstacleSeparation)
            {
                minObstacleSeparationRange -= separationReduction;
            }
            else
            {
                minObstacleSeparationRange = minimumMinObstacleSeparation;
            }
        }

        mapObjects.Add(Instantiate(excavatorItem, new Vector3(excavatorPos, 1), Quaternion.identity));

        Pass_Obstacles(trackLength);
        Pass_EFuel_Positions(trackLength);
        Pass_MFuel_EFuel(trackLength);
        Pass_EFuel_VPos();
        DrawMap(startingX);
    }

    public void ClearAllMapObjects()
    {
        foreach(GameObject mapObject in mapObjects)
        {
            Destroy(mapObject);
        }
        mapObjects.Clear();
    }

    private void DrawMap(int startingX)
    {
        foreach(MapObstacleInfo obstacle in obstacles)
        {
            mapObjects.Add(Instantiate(obstacle.Prefab, new Vector3(startingX + obstacle.HorizontalPosition + 0.5f, obstacle.VerticalPosition + 0.5f), Quaternion.identity));
        }
        
        foreach(MapMFuelInfo fuel in mFuel)
        {
            //testTilemap.SetTile(new Vector3Int(fuel.HorizontalPosition, fuel.VerticalPosition), mFuelTile);
            mapObjects.Add(Instantiate(pfMFuel, new Vector3(startingX + fuel.HorizontalPosition + 0.5f, fuel.VerticalPosition + 0.5f), Quaternion.identity));
        }

        foreach (MapEFuelInfo fuel in eFuel)
        {
            //testTilemap.SetTile(new Vector3Int(fuel.HorizontalPosition, fuel.VerticalPosition), eFuelTile);
            mapObjects.Add(Instantiate(pfEFuel, new Vector3(startingX + fuel.HorizontalPosition + 0.5f, fuel.VerticalPosition + 0.5f), Quaternion.identity));
        }

        obstacles.Clear();
        mFuel.Clear();
        eFuel.Clear();
    }

    public void DrawWall(int wallX, int wallLength)
    {
        Destroy(wallI);
        //testTilemap.ClearAllTiles();

        //for (int y = 0; y <= verticalValidMapSize; y++)
        //{
        //    for (int x = 0; x < wallLength; x++)
        //    {
        //        testTilemap.SetTile(new Vector3Int(wallX + x, y), dirtTile);
        //    }
        //}

        wallI = Instantiate(wallPf, new Vector3(wallX, 3f, -1), Quaternion.identity);
    }

    private void Pass_Obstacles(int horizontalMapSize)
    {
        int horizontalPosition = Random.Range(minObstacleSeparationRange, maxObstacleSeparationRange);
        if (horizontalPosition % 2 != 0) horizontalPosition++;

        while (horizontalPosition < horizontalMapSize)
        {
            ObstacleSO obstacle;
            List<int> keyPos;
            int verticalPosition = 0;
            switch (SelectObstacleType())
            {
                default:
                case ObstacleType.Ground:
                    obstacle = groundObstacles[Random.Range(0, groundObstacles.Count)];
                    keyPos = obstacle.keyPos;
                    break;
                case ObstacleType.TopDown:
                    obstacle = topDownObstacles[Random.Range(0, topDownObstacles.Count)];
                    keyPos = obstacle.keyPos;
                    break;
            }

            obstacles.Add(new MapObstacleInfo(horizontalPosition, verticalPosition, obstacle.prefab, keyPos));

            horizontalPosition += Random.Range(minObstacleSeparationRange, maxObstacleSeparationRange);
            if (horizontalPosition % 2 != 0) horizontalPosition++;
        }

    }

    private ObstacleType SelectObstacleType()
    {
        int groundObstacleCount = groundObstacleProb;
        int topDownObstacleCount = topDownObstacleProb;

        ObstacleType[] obstacleTypes = new ObstacleType[groundObstacleCount + topDownObstacleCount];

        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            obstacleTypes[i] = (i < groundObstacleCount) ? ObstacleType.Ground : ObstacleType.TopDown;
        }

        return obstacleTypes[Random.Range(0, obstacleTypes.Length)];
    }

    private void Pass_EFuel_Positions(int horizontalMapSize)
    {
        currentEFuel = Random.Range(minEFuel, maxEFuel);
        int stepDistance = horizontalMapSize / currentEFuel;

        int minDistance = 0;
        int maxDistance = stepDistance;
        for (int i = 0; i < currentEFuel; i++)
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
                                    if(desiredPosition > verticalMapSize) checkUp = false;
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

    private void ResetSeparation()
    {
        maxObstacleSeparationRange = maximumMaxObstacleSeparation;
        minObstacleSeparationRange = maximumMinObstacleSeparation;
    }


    private void OnEnable()
    {
        EventManager.GameRestarted += ResetSeparation;
    }

    private void OnDisable()
    {
        EventManager.GameRestarted -= ResetSeparation;
    }
}
