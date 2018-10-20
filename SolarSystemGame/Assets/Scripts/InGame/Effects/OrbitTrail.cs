using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitTrail : MonoBehaviour
{
    private LineRenderer objLineRenderer;

    private List<Vector2> vertices = new List<Vector2>();

    [SerializeField] private float vertexSpacing = 0.1f;
    [SerializeField] private float maxTrailTime = 1.0f;

    private float timeAlive = 0.0f;

    private void Start()
    {
        objLineRenderer = GetComponent<LineRenderer>();
        AddVertex();
    }

    private void Update()
    {
        //What do we want
        //For x amount of seconds, I want a trail to show.
        //After x seconds I want the back of the trail to start disappearing.

        if (vertices.Count == 0 || Vector3.Distance(vertices.Last(), transform.position) > vertexSpacing)
        {
            AddVertex();
        }

        timeAlive += Time.deltaTime;

        if (timeAlive > maxTrailTime)
        {
            timeAlive -= Time.deltaTime;
            ShiftVertices();
        }
    }

    private void ShiftVertices()
    {
        vertices.RemoveAt(0);
        objLineRenderer.positionCount = vertices.Count;

        if (vertices.Count > 0)
        {
            for (int i = 0; i < vertices.Count - 1; ++i)
            {
                objLineRenderer.SetPosition(i, vertices[i + 1]);
            }

            //Debug.Log("VERTICE LENGTH: " + vertices.Count);
            objLineRenderer.SetPosition(vertices.Count - 1, transform.position);
        }
    }

    private void AddVertex()
    {
        //if (vertices.Count == maxTailLength)
        //{
        //    vertices.RemoveAt(0);
        //    vertices.Add(transform.position);

        //    objLineRenderer.positionCount = vertices.Count;

        //    for (int i = 0; i < vertices.Count - 1; ++i)
        //    {
        //        objLineRenderer.SetPosition(i, vertices[i + 1]);
        //    }
        //}
        //else
        //{
            vertices.Add(transform.position);
            objLineRenderer.positionCount = vertices.Count;

        Debug.Log("VERTICE LENGTH:" + vertices.Count);
        //}

        objLineRenderer.SetPosition(vertices.Count - 1, transform.position);
    }
}
