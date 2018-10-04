using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 1000.0f;

    public float currentSize = 10.0f;

    public float minimumSize = 5.0f;
    public float maximumSize = 30.0f;

    private Camera objCamera;

    private void Start()
    {
        objCamera = GetComponent<Camera>();
        UpdateCameraSize(currentSize);
    }

    private void OnValidate()
    {
        objCamera = GetComponent<Camera>();
        UpdateCameraSize(currentSize);
    }

    private void Update()
    {
        ZoomCamera();
    }

    private void ZoomCamera()
    {
        UpdateCameraSize(currentSize - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
    }

    private void UpdateCameraSize(float size)
    {
        currentSize = Mathf.Lerp(Mathf.Clamp(size, minimumSize, maximumSize), currentSize, Time.deltaTime);
        objCamera.orthographicSize = currentSize;
    }


}
