using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Obstacle", menuName = "EndlessRunner/Obstacle")]
public class ObstacleSO : ScriptableObject
{
    public enum RootPosition
    {
        Top,
        Middle,
        Bottom
    }

    public GameObject prefab;
    public bool top, middle, bottom;
    public RootPosition rootPosition;
}
