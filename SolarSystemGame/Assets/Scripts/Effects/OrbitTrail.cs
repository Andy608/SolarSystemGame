using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitTrail : MonoBehaviour
{
    private LineRenderer objLineRenderer;

    private List<Vector2> vertices = new List<Vector2>();

    public float vertexSpacing = 0.1f;
    public int maxTailLength = 30;

    private void Start()
    {
        objLineRenderer = GetComponent<LineRenderer>();
        AddVertex();
    }

    private void Update()
    {
        if (Vector3.Distance(vertices.Last(), transform.position) > vertexSpacing)
        {
            AddVertex();
        }
    }

    private void AddVertex()
    {
        if (vertices.Count == maxTailLength)
        {
            vertices.RemoveAt(0);
            vertices.Add(transform.position);

            objLineRenderer.positionCount = vertices.Count;

            for (int i = 0; i < vertices.Count - 1; ++i)
            {
                objLineRenderer.SetPosition(i, vertices[i + 1]);
            }
        }
        else
        {
            vertices.Add(transform.position);
            objLineRenderer.positionCount = vertices.Count;
        }

        objLineRenderer.SetPosition(vertices.Count - 1, transform.position);
    }
}
