using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Will be turned into a manager script when I'm not in crunch mode.
public class SpawnObject : MonoBehaviour
{
    public GameObject planetPrefab;
    private Vector3 planetPosition;
    private const float Z_PLANE = 0;

    private void OnEnable()
    {
        Managers.InputHandler.OnTap += Spawn;
    }

    private void OnDisable()
    {
        Managers.InputHandler.OnTap -= Spawn;
    }

    private void Spawn(Touch t)
    {
        planetPosition = Camera.main.ScreenToWorldPoint(t.position);
        planetPosition.z = Z_PLANE;

        Instantiate(planetPrefab, planetPosition, Quaternion.identity);
    }
}
