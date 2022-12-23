using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Obstacle", menuName = "EndlessRunner/Obstacle")]
public class ObstacleSO : ScriptableObject
{

    public GameObject prefab;
    public List<int> keyPos;
    public int size;
}
