using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    private static float UNDEFINED = -1;

    public float zoomSpeed = 10.0f;

    [SerializeField] private float startingSize = 10.0f;
    private float currentSize;

    public float minimumSize = 5.0f;
    public float maximumSize = 30.0f;

    private float followVelocity;
    private float smoothTime = 0.6f;

    private Camera objCamera;

    private float zoomDist = 0;
    private float prevZoomDist = UNDEFINED;
    private float zoomOffset = 0;

    private void Start()
    {
        objCamera = GetComponent<Camera>();
        currentSize = startingSize;
        UpdateCameraSize(currentSize);
    }

    private void OnValidate()
    {
        objCamera = GetComponent<Camera>();
        currentSize = startingSize;
        UpdateCameraSize(currentSize);
    }

    private void OnEnable()
    {
        Managers.InputHandler.OnPinchBegan += ZoomBegan;
        Managers.InputHandler.OnPinchHeld += ZoomHeld;
        Managers.InputHandler.OnPinchEnded += ZoomEnded;
    }

    private void OnDisable()
    {
        Managers.InputHandler.OnPinchBegan -= ZoomBegan;
        Managers.InputHandler.OnPinchHeld -= ZoomHeld;
        Managers.InputHandler.OnPinchEnded -= ZoomEnded;
    }

    private void ZoomBegan(Touch first, Touch second)
    {
        zoomDist = (first.position - second.position).magnitude;
        prevZoomDist = zoomDist;

        Zoom();
    }

    private void ZoomHeld(Touch first, Touch second)
    {
        prevZoomDist = zoomDist;
        zoomDist = (first.position - second.position).magnitude;

        Zoom();
    }

    private void ZoomEnded(Touch first, Touch second)
    {
        Zoom();

        prevZoomDist = 0;
        zoomDist = 0;
    }

    private void Zoom()
    {
        zoomOffset = prevZoomDist / zoomDist;
        Debug.Log("ZoomDist: " + zoomDist + " prev" + prevZoomDist + " Offset" + zoomOffset);

        //Debug.Log("Initial: " + initialDistance + " current: " + currentDistanceNormalized);
        //Debug.Log("SIZE: " + currentSize * currentDistanceNormalized);
        UpdateCameraSize(currentSize * zoomOffset);
    }


    private void Update()
    {
        //ZoomCamera();
    }

    private void ZoomCamera()
    {
        UpdateCameraSize(currentSize - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
    }

    private void UpdateCameraSize(float size)
    {
        //Debug.Log("Size: " + size + " Min: " + minimumSize + " Max: " + maximumSize + " Clamp: " + Mathf.Clamp(size, minimumSize, maximumSize) + " Current: " + currentSize);
        currentSize = Mathf.Lerp(Mathf.Clamp(size, minimumSize, maximumSize), currentSize, Time.deltaTime);
        objCamera.orthographicSize = currentSize;
    }
}
