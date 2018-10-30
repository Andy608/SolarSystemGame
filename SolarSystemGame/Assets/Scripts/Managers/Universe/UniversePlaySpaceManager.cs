using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UniversePlaySpaceManager : ManagerBase<UniversePlaySpaceManager>
    {
        public delegate void AbsorbedAction(SpaceObject absorber, SpaceObject absorbed);
        public static event AbsorbedAction OnAbsorbed;

        public delegate void ObjectDestroyedEvent(SpaceObject destroyedObj);
        public static event ObjectDestroyedEvent OnObjectDestroyed;

        //If objects get outside this area, they despawn.
        public static float UNIVERSE_BOUNDS = 200.0f;

        [SerializeField] private GameObject centerObjPrefab;

        private Vector3 prevPosition;
        private Vector2 universeVelocity;

        private GameObject centerOfUniverse = null;

        private List<SpaceObject> objectsInUniverse;

        public GameObject CenterOfUniverse { get { return centerOfUniverse; } }
        public Vector3 UniverseVelocity { get { return universeVelocity; } }

        private void Start()
        {
            centerOfUniverse = Instantiate(centerObjPrefab);
            centerOfUniverse.name = "CenterOfUniverse";
            prevPosition = centerOfUniverse.transform.position;
            universeVelocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            objectsInUniverse = ObjectTracker.Instance.ObjectsInUniverse;

            //Update the universe and get the velocity.
            UpdateCenterOfSolarSystem();

            //Wrap the universe around if it gets out of range
            //RemapUniversePositions();
            DestroyOuterObjects();
        }

        private void UpdateCenterOfSolarSystem()
        {
            if (objectsInUniverse.Count == 0)
            {
                //centerOfSolarSystem.transform.position = Vector3.zero;
                return;
            }
            else
            {
                Vector3 position = Vector3.zero;
                float massTotal = 0.0f;
                float objCount = objectsInUniverse.Count;

                foreach (SpaceObject obj in objectsInUniverse)
                {
                    position += obj.transform.position * obj.objRigidbody.mass;
                    massTotal += obj.objRigidbody.mass;
                }

                //Get the average position of the universe
                position /= (objCount + massTotal);

                //Set the previous position.
                prevPosition = centerOfUniverse.transform.position;

                //Set the current position.
                centerOfUniverse.transform.position = position;

                //Get the velocity of the universe as a whole.
                universeVelocity = position - prevPosition;
            }
        }

        private void DestroyOuterObjects()
        {
            List<SpaceObject> objectsInUniverse = ObjectTracker.Instance.ObjectsInUniverse;
            SpaceObject currentObj;

            int currentIndex = 0;
            for (; currentIndex < objectsInUniverse.Count; ++currentIndex)
            {
                currentObj = objectsInUniverse[currentIndex];

                if (currentObj.transform.position.sqrMagnitude > UNIVERSE_BOUNDS * UNIVERSE_BOUNDS)
                {
                    if (OnObjectDestroyed != null)
                    {
                        OnObjectDestroyed(currentObj);
                    }

                    Destroy(currentObj.gameObject);
                    --currentIndex;
                }
            }
        }

        public static void AbsorbObject(SpaceObject obj1, SpaceObject obj2)
        {
            SpaceObject absorber;
            SpaceObject absorbed;

            if (obj1.objRigidbody.mass >= obj2.objRigidbody.mass)
            {
                absorber = obj1;
                absorbed = obj2;
            }
            else
            {
                absorber = obj2;
                absorbed = obj1;
            }

            //Add the impact force.
            //We need to experiment with this. This is wrong.
            absorber.objRigidbody.AddRelativeForce(absorbed.objRigidbody.velocity * absorbed.objPhysicsProperties.CurrentMass, ForceMode2D.Impulse);

            absorber.objPhysicsProperties.CurrentMass += absorbed.objPhysicsProperties.CurrentMass;
            absorber.objRigidbody.mass = absorber.objPhysicsProperties.CurrentMass;
            absorber.objPhysicsProperties.UpdateRadiusAndScale();

            if (OnObjectDestroyed != null)
            {
                OnObjectDestroyed(absorbed);
            }

            if (OnAbsorbed != null)
            {
                OnAbsorbed(absorber, absorbed);
            }

            Destroy(absorbed.gameObject);
        }

        //private void RemapUniversePositions()
        //{
        //    //Debug.Log("Current: " + currentPosition + " Prev: " + prevPosition + " Transform: " + centerOfSolarSystem.transform.position);

        //    if (centerOfUniverse.transform.position.magnitude > UNIVERSE_BOUNDS)
        //    {
        //        List<SpaceObject> objectsInUniverse = ObjectTracker.Instance.ObjectsInUniverse;
        //        Vector2 offest = -2.0f * centerOfUniverse.transform.position;

        //        foreach (SpaceObject obj in objectsInUniverse)
        //        {
        //            obj.transform.Translate(offest);
        //        }

        //        CameraState.Instance.TranslateCamera(offest);
        //        centerOfUniverse.transform.Translate(offest);
        //    }
        //}
    }
}