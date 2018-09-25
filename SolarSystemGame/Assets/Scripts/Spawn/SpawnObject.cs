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

    //private void Start()
    //{
    //    EventTrigger trigger = GetComponent<EventTrigger>();
    //    EventTrigger.Entry entry = new EventTrigger.Entry();
    //    entry.eventID = EventTriggerType.Drag;
    //    entry.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });
    //    trigger.triggers.Add(entry);
    //}

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            planetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            planetPosition.z = Z_PLANE;

            Instantiate(planetPrefab, planetPosition, Quaternion.identity);
        }
    }

    //public void OnDragDelegate(PointerEventData data)
    //{

    //}
}
