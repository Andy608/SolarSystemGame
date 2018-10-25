using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OrbitTracker))]
public class OrbitTrackerEditor : Editor
{
    private Vector3 startDirection;

    private void OnSceneGUI()
    {
        //Getting a Field of View Reference
        OrbitTracker orbitTracker = (OrbitTracker)target;

        foreach (OrbitData data in orbitTracker.ChildList)
        {
            startDirection = data.StartDirection;

            Handles.color = data.DEBUG_COLOR;
            Handles.DrawLine(data.ParentTransform.position, data.ParentTransform.position + startDirection);

            Handles.color = data.DEBUG_COLOR;
            Handles.DrawLine(data.ParentTransform.position, data.ChildTransform.position);
        }

        foreach (OrbitData data in orbitTracker.ParentList)
        {
            startDirection = data.StartDirection;

            Handles.color = Color.white;
            Handles.DrawLine(data.ParentTransform.position, data.ParentTransform.position + startDirection);

            Handles.color = Color.white;
            Handles.DrawLine(data.ParentTransform.position, data.ChildTransform.position);
        }
    }
}
