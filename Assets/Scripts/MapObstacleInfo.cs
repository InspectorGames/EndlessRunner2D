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
    private int verticalPosition;
    private int verticalSize;
    //private int horizontalSize;
    private int horizontalPosition;
    private bool growUp;
    private MapObjectType type = MapObjectType.Obstacle;
    private List<int> keyPos = new List<int>();

    public int VerticalPosition { get { return verticalPosition; } }
    public int VerticalSize { get { return verticalSize; } }
    //public int HorizontalSize { get { return horizontalSize; } }
    public int HorizontalPosition { get { return horizontalPosition; } }
    public bool GrowUp { get { return growUp; } }
    public MapObjectType Type { get { return type; } }
    public List<int> KeyPos { get { return keyPos; } }

    //public MapObjectInfo(int orderNumber, int verticalSize, int horizontalSize, int mapPosition, MapObjectType type)
    //{
    //    this.orderNumber = orderNumber;
    //    this.verticalSize = verticalSize;
    //    //this.horizontalSize = horizontalSize;
    //    this.mapPosition = mapPosition;
    //    this.type = type;
    //}

    public MapObstacleInfo(int rootPosition, int verticalSize, int mapPosition, bool growUp)
    {
        this.verticalPosition = rootPosition;
        this.verticalSize = verticalSize;
        //this.horizontalSize = horizontalSize;
        this.horizontalPosition = mapPosition;
        this.growUp = growUp;
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