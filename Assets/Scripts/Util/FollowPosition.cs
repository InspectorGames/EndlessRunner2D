using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    [SerializeField] private bool followX, followY;

    private void Update()
    {
        Vector3 followingPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (followX) followingPosition.x = target.position.x + offset.x;
        if (followY) followingPosition.y = target.position.y + offset.y;

        transform.position = followingPosition;
    }
}
