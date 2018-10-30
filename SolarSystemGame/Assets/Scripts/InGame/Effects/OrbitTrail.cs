using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class OrbitTrail : MonoBehaviour
{
    private LineRenderer objLineRenderer;
    private TrailRenderer objTrailRenderer;
    private List<Vector2> vertices = new List<Vector2>();
    [SerializeField] private float vertexSpacing = 0.1f;
    [SerializeField] private float maxTrailTime = 1.0f;
    private int maxVertices = 50;
    private float timeTilSpawn = 0.01f;

    private float timeCounter = 0.0f;

    private SpaceObject objSpaceObj;

    public TrailRenderer ObjTrailRenderer { get { return objTrailRenderer; } }

    private void Start()
    {
        objSpaceObj = transform.parent.GetComponent<SpaceObject>();
        objLineRenderer = GetComponent<LineRenderer>();
        //objTrailRenderer = GetComponent<TrailRenderer>();
        UpdateTrailProperties();
        AddVertex();
    }

    private void OnEnable()
    {
        Managers.UniversePlaySpaceManager.OnAbsorbed += UpdateFromAbsorb;
    }

    private void OnDisable()
    {
        Managers.UniversePlaySpaceManager.OnAbsorbed -= UpdateFromAbsorb;
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;

        //What do we want
        //For x amount of seconds, I want a trail to show.
        //After x seconds I want the back of the trail to start disappearing.
        if (timeCounter > timeTilSpawn)
        {
            if (Vector3.Distance(vertices.Last(), transform.position) > vertexSpacing)
            {
                timeCounter = 0.0f;
                AddVertex();
            }

            if (vertices.Count == maxVertices)
            {
                ShiftVertices();
            }
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

            objLineRenderer.SetPosition(vertices.Count - 1, transform.position);
        }
    }

    private void AddVertex()
    {
        vertices.Add(transform.position);
        objLineRenderer.positionCount = vertices.Count;
        objLineRenderer.SetPosition(vertices.Count - 1, transform.position);
    }

    private void UpdateFromAbsorb(SpaceObject absorber, SpaceObject absorbed)
    {
        if (objSpaceObj == absorber)
        {
            UpdateTrailProperties();
        }
    }

    private void UpdateTrailProperties()
    {
        //Update trail color
        Color32 current = objSpaceObj.ObjectColor;
        Color32 alpha = current;
        alpha.a = 0;
        objLineRenderer.endColor = current * Color.white * 0.8f;
        objLineRenderer.startColor = alpha;

        //Update trail width
        float width = transform.parent.GetComponent<SpaceObject>().objPhysicsProperties.Circumference;
        objLineRenderer.endWidth = width;
        objLineRenderer.startWidth = 0.0f;
    }
}