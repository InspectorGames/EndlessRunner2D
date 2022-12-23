using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapObjectType
{
    MFuel,
    EFuel,
    Obstacle
}

public class MapObstacleInfo
{
    private GameObject prefab;
    private int horizontalPosition;
    private int verticalPosition;
    private MapObjectType type = MapObjectType.Obstacle;
    private List<int> keyPos = new List<int>();

    public List<int> KeyPos { get { return keyPos; } }
    public GameObject Prefab { get { return prefab; } }
    public int HorizontalPosition { get { return horizontalPosition; } }
    public int VerticalPosition { get { return verticalPosition; } }
    public MapObjectType Type { get { return type; } }

    public MapObstacleInfo(int horizontalPosition, int verticalPosition, GameObject prefab, List<int> keyPos)
    {
        this.horizontalPosition = horizontalPosition;
        this.verticalPosition = verticalPosition;
        this.prefab = prefab;
        this.keyPos = new List<int>(keyPos);
    }
}

public class MapMFuelInfo
{
    private int verticalPosition;
    private int horizontalPosition;
    private MapObjectType type = MapObjectType.MFuel;

    public int VerticalPosition { get { return verticalPosition; } set { verticalPosition = value; } }
    public int HorizontalPosition { get { return horizontalPosition; } set { horizontalPosition = value; } }
    public MapObjectType Type { get { return type; } }

    public MapMFuelInfo(int verticalPosition, int horizontalPosition)
    {
        this.verticalPosition = verticalPosition;
        this.horizontalPosition = horizontalPosition;
    }
}

public class MapEFuelInfo
{
    private int verticalPosition;
    private int horizontalPosition;
    private MapObjectType type = MapObjectType.EFuel;

    public int VerticalPosition { get { return verticalPosition; } set { verticalPosition = value; } }
    public int HorizontalPosition { get { return horizontalPosition; } set { horizontalPosition = value; } }
    public MapObjectType Type { get { return type; } }

    public MapEFuelInfo(int verticalPosition, int horizontalPosition)
    {
        this.verticalPosition = verticalPosition;
        this.horizontalPosition = horizontalPosition;
    }
}