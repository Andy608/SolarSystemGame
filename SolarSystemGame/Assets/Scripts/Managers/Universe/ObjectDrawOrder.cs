using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ObjectDrawOrder : ManagerBase<MonoBehaviour>
    {
        private const string SORTING_LAYER = "SpaceObject";

        private void OnEnable()
        {
            ObjectTracker.OnObjectSpawned += UpdateDrawOrder;
        }

        private void OnDisable()
        {
            ObjectTracker.OnObjectSpawned -= UpdateDrawOrder;
        }

        private void UpdateDrawOrder(SpaceObject newObj)
        {
            //Set the new objects draw layer to the space object draw layer
            newObj.GetComponent<SpriteRenderer>().sortingLayerName = SORTING_LAYER;

            //Get list of active objects in universe.
            List<SpaceObject> activeObjects = ObjectTracker.Instance.ActiveObjectsInUniverse;

            //Sort by max mass first
            SortObjectsByMass(ref activeObjects);

            //Testing sort
            printArray(activeObjects);

            //set the draw order to the index of the list
            int currentIndex = 0;
            for (; currentIndex < activeObjects.Count; ++currentIndex)
            {
                activeObjects[currentIndex].GetComponent<SpriteRenderer>().sortingOrder = currentIndex;
            }
        }

        private void SortObjectsByMass(ref List<SpaceObject> objects)
        {
            //If we run into performance issues, then we'll look to optimize this algorithm.

            if (objects.Count > 1)
            {
                //Debug.Log("SORTING");
                int currentIndex;
                int otherIndex;
                int maxIndex;

                for (currentIndex = 0; currentIndex < objects.Count - 1; ++currentIndex)
                {
                    maxIndex = currentIndex;

                    for (otherIndex = currentIndex + 1; otherIndex < objects.Count; ++otherIndex)
                    {
                        if (objects[otherIndex].objRigidbody.mass < objects[currentIndex].objRigidbody.mass)
                        {
                            maxIndex = otherIndex;
                        }
                    }

                    //Swap
                    //Debug.Log("SWAPPING");
                    SpaceObject tempIndex = objects[maxIndex];
                    objects[maxIndex] = objects[currentIndex];
                    objects[currentIndex] = tempIndex;
                }
            }
        }

        // Prints the array 
        static void printArray(List<SpaceObject> arr)
        {
            string s = "";
            int n = arr.Count;
            for (int i = 0; i < n; ++i)
                s += ("[" + arr[i].objRigidbody.mass.ToString() + "] ");

            //Debug.Log(s);
        }
    }
}

