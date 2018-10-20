using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Tomorrow: Make this a manager class that holds a copy of the universe.
//Move the positions around in this copy of the universe.
//Then each object can ask about the state of the universe at x seconds.
//We can use this manager class for trails as well as future trajectories.
public class OrbitTrajectory : MonoBehaviour
{
    [SerializeField] private GameObject objTrail;

    [SerializeField] private float offsetTime = 0.5f;

    private LineRenderer objLineRenderer;
    private List<Vector2> vertices = new List<Vector2>();

    private SpaceObject objSpaceObj;

    [SerializeField] private float vertexSpacing = 0.01f;

    private void OnEnable()
    {
        Managers.InputHandler.OnDragBegan += UpdateStartingVelocity;
        Managers.InputHandler.OnDragHeld += UpdateStartingVelocity;
        Managers.InputHandler.OnDragEnded += UpdateStartingVelocity;

        Debug.Log("START");
        objSpaceObj = GetComponent<SpaceObject>();
        objLineRenderer = objTrail.GetComponent<LineRenderer>();
    }

    private void OnDisable()
    {
        Managers.InputHandler.OnDragBegan -= UpdateStartingVelocity;
        Managers.InputHandler.OnDragHeld -= UpdateStartingVelocity;
        Managers.InputHandler.OnDragEnded -= UpdateStartingVelocity;
    }

    public void Show()
    {
        Managers.GameState.Instance.RequestPause();
        Debug.Log("SHOW");
        //Initialize the trajectory.
        objLineRenderer.gameObject.SetActive(true);
    }

    public void Hide()
    {
        Managers.GameState.Instance.RequestUnpause();
        //Clear the trajectory.
        ClearTrajectory();
        objLineRenderer.gameObject.SetActive(false);
    }

    private void UpdateStartingVelocity(Touch touch)
    {
        Vector3 worldTouchPos = Vector3.zero;
        Managers.ObjectTracker.Instance.TouchPositionToWorldVector3(touch, ref worldTouchPos);

        Vector2 worldPos = worldTouchPos;

        ClearTrajectory();
        InitTrajectory(objSpaceObj.objRigidbody.position - worldPos);
    }

    private void InitTrajectory(Vector2 startingVelocity)
    {
        List<SpaceObject> activeObjects = Managers.ObjectTracker.Instance.ObjectsInUniverse;

        CreateTrajectory(activeObjects, startingVelocity, offsetTime);
    }

    private void ClearTrajectory()
    {
        vertices.Clear();
        objLineRenderer.positionCount = 0;
    }

    private void CreateTrajectory(List<SpaceObject> objectList, Vector2 startingVelocity, float offsetTime)
    {
        float iterTime = 0;
        float offsetTimeFactor = 1.0f;
        Vector2 position = objSpaceObj.objRigidbody.position;
        Vector2 velocity = startingVelocity;
        Vector2 frameAcceleration = Vector2.zero;

        if (offsetTime == 0)
        {
            return;
        }
        else if (offsetTime < 0)
        {
            offsetTimeFactor = -1.0f;
            offsetTime *= -1.0f;
        }

        if (vertices.Count == 0)
        {
            AddVertex(position);
        }

        while (iterTime < offsetTime)
        {
            foreach (SpaceObject otherObj in objectList)
            {
                if (otherObj == objSpaceObj) continue;

                Rigidbody2D rbToGravitate = otherObj.objRigidbody;

                Vector2 direction = rbToGravitate.position - position;
                float distanceSquared = direction.sqrMagnitude;

                float distanceFromCenter = (objSpaceObj.objPhysicsProperties.Radius + otherObj.objPhysicsProperties.Radius);

                if (direction.sqrMagnitude > distanceFromCenter * distanceFromCenter)
                {
                    float gravitationMag = Gravitate.G * rbToGravitate.mass / distanceSquared;
                    Vector2 gravitationalForce = direction.normalized * gravitationMag;
                    frameAcceleration += gravitationalForce;
                }
                else
                {
                    //We got absorbed, so we should stop drawing the line.
                    return;
                }
            }

            velocity = startingVelocity * offsetTimeFactor + (frameAcceleration * Time.fixedDeltaTime);
            position = position + velocity * Time.fixedDeltaTime;

            //Check to add vertex in list.

            if (Vector2.Distance(vertices.Last(), position) > vertexSpacing)
            {
                AddVertex(position);
            }

            iterTime += Time.fixedDeltaTime;
        }
    }

    private void AddVertex(Vector2 position)
    {
        vertices.Add(position);
        objLineRenderer.positionCount = vertices.Count;

        objLineRenderer.SetPosition(vertices.Count - 1, position);
    }
}
