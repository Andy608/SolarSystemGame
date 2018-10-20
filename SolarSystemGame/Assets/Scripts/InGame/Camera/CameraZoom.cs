using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 10.0f;

    [SerializeField] private float startingSize = 10.0f;
    private float currentSize;

    public float minimumSize = 5.0f;
    public float maximumSize = 30.0f;

    private Camera objCamera;

    private float zoomDist = 0;
    private float prevZoomDist = 0;
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
        if (zoomDist == 0) return;

        zoomOffset = prevZoomDist / zoomDist;
        UpdateCameraSize(currentSize * zoomOffset);
    }

    private void UpdateCameraSize(float size)
    {
        currentSize = Mathf.Lerp(Mathf.Clamp(size, minimumSize, maximumSize), currentSize, Time.deltaTime);
        objCamera.orthographicSize = currentSize;
    }
}
