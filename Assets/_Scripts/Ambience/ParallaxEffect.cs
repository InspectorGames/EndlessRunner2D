using System;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private List<ParallaxElement> parallaxElements;

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private bool stopParallax;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
        //Maybe here all the Parallax Elements could initialize "SpriteWidth" and "StartPosition"
        foreach(ParallaxElement element in parallaxElements)
        {
            element.Init();
        }
    }
    private void FixedUpdate()
    {
        //Debug.Log("Camera Positon: " + cameraTransform.position.x + " Previous Camera Position: " + previousCameraPosition.x);

        foreach (ParallaxElement element in parallaxElements)
        {
            float temp = (cameraTransform.position.x * (1 - element.ParallaxMult));
            float dist = cameraTransform.position.x * element.ParallaxMult;

            element.ElementTransform.position = new Vector3(element.StartPosition + dist, element.ElementTransform.position.y, element.ElementTransform.position.z);

            if (temp > element.StartPosition + element.SpriteWidth) element.StartPositionAdd(element.SpriteWidth);
            else if (temp < element.StartPosition - element.SpriteWidth) element.StartPositionAdd(-element.SpriteWidth);
        }
    }

    //private void LateUpdate()
    //{
        //if (!stopParallax)
        //{
        //    foreach(ParallaxElement element in parallaxElements)
        //    {
        //        float deltaX = (cameraTransform.position.x - previousCameraPosition.x) * element.ParallaxMult;
        //        float moveAmount = cameraTransform.position.x * (1 - element.ParallaxMult);
        //        element.ElementTransform.Translate(new Vector3(deltaX, 0, 0));

        //        if (moveAmount > element.StartPosition + element.SpriteWidth)
        //        {
        //            element.ElementTransform.Translate(new Vector3(element.SpriteWidth, 0, 0));
        //            element.StartPositionAdd(element.SpriteWidth);
        //        }
        //        else if (moveAmount < element.StartPosition - element.SpriteWidth)
        //        {
        //            element.ElementTransform.Translate(new Vector3(-element.SpriteWidth, 0, 0));
        //            element.StartPositionAdd(-element.SpriteWidth);
        //        }

        //    }
        //}
        
        //previousCameraPosition = cameraTransform.position;
    //}

    public void SetStopParallax(bool value)
    {
        stopParallax = value;
        previousCameraPosition = cameraTransform.position;
    }

    public void ResetElements()
    {
        previousCameraPosition = cameraTransform.position;
        foreach(ParallaxElement element in parallaxElements)
        {
            element.Reset();
        }
    }
}
[Serializable]
public class ParallaxElement
{
    [SerializeField] private Transform elementTransform;
    [SerializeField] private float parallaxMult;
    [SerializeField] private float spriteWidth;
    [SerializeField] private float startPosition;

    public Transform ElementTransform { get { return elementTransform; } }
    public float ParallaxMult { get { return parallaxMult; } }
    public float SpriteWidth { get { return spriteWidth; } }
    public float StartPosition { get { return startPosition; } }

    public void Init()
    {
        spriteWidth = elementTransform.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        startPosition = elementTransform.transform.position.x;
    }

    public void Reset()
    {
        elementTransform.position = Vector3.zero;
        startPosition = 0;
    }

    public void StartPositionAdd(float add)
    {
        startPosition += add;
    }
}