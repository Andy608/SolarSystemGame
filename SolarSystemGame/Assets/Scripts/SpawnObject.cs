using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will be turned into a manager script when I'm not in crunch mode.
public class SpawnObject : MonoBehaviour
{
    public GameObject planetPrefab;
    private Vector3 planetPosition;
    private const float Z_PLANE = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            planetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            planetPosition.z = Z_PLANE;

            Instantiate(planetPrefab, planetPosition, Quaternion.identity);
        }
    }
}
