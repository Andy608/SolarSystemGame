﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    //Responsible for holding a list of all the objects in the universe.
    public class ObjectTracker : ManagerBase<ObjectTracker>
    {
        public delegate void SelectedObjectChangeAction();
        public static event SelectedObjectChangeAction OnSelectedObjectChanged;

        public delegate void SpawnObjectAction();
        public static event SpawnObjectAction OnObjectSpawned;

        //Plane that all objects are on.
        private const float OBJECT_Z_PLANE = 0;

        //The list of all the objects that are affected by gravity.
        private List<SpaceObject> objectsInUniverse = new List<SpaceObject>();
        private List<SpaceObject> activeObjectsInUniverse = new List<SpaceObject>();
        private List<SpaceObject> pausedObjectsInUniverse = new List<SpaceObject>();

        //The object in the process of being created right now.
        private SpaceObject ghostObject = null;

        //Holds the current active object.
        private SpaceObject selectedObj = null;

        private static Vector3 spawnPosition = new Vector3();
        private static Vector3 touchPosition = new Vector3();
        private static Vector3 dragPosition = new Vector3();

        public GameObject planetPrefab;

        public SpaceObject SelectedObj
        {
            get
            {
                return selectedObj;
            }

            set
            {
                selectedObj = value;

                if (OnSelectedObjectChanged != null)
                {
                    OnSelectedObjectChanged();
                }
            }
        }

        private void OnEnable()
        {
            Managers.GameState.OnGamePaused += HandlePause;
            Managers.GameState.OnGameUnPaused += HandleUnPause;

            Managers.InputHandler.OnTap += HandleTap;

            Managers.InputHandler.OnDragBegan += HandleDragBegan;
            Managers.InputHandler.OnDragHeld += HandleDragHeld;
            Managers.InputHandler.OnDragEnded += HandleDragEnded;
        }

        private void OnDisable()
        {
            Managers.GameState.OnGamePaused -= HandlePause;
            Managers.GameState.OnGameUnPaused -= HandleUnPause;

            Managers.InputHandler.OnTap -= HandleTap;

            Managers.InputHandler.OnDragBegan -= HandleDragBegan;
            Managers.InputHandler.OnDragHeld -= HandleDragHeld;
            Managers.InputHandler.OnDragEnded -= HandleDragEnded;
        }

        public void RegisterObject(SpaceObject spaceObj)
        {
            objectsInUniverse.Add(spaceObj);
            activeObjectsInUniverse.Add(spaceObj);

            if (Managers.GameState.Instance.IsState(GameState.EnumGameState.PAUSED))
            {
                PauseObj(spaceObj);
            }
        }

        public void UnRegisterObject(SpaceObject spaceObj)
        {
            if (objectsInUniverse != null)
            {
                objectsInUniverse.Remove(spaceObj);
            }

            if (activeObjectsInUniverse != null)
            {
                activeObjectsInUniverse.Remove(spaceObj);
            }
        }

        private void FixedUpdate()
        {
            if (Managers.GameState.Instance.IsState(GameState.EnumGameState.RUNNING))
            {
                for (int i = 0; i < activeObjectsInUniverse.Count; ++i)
                {
                    Gravitate gravitator = activeObjectsInUniverse[i].objGravitate;

                    for (int j = 0; j < activeObjectsInUniverse.Count; ++j)
                    {
                        Gravitate objToGravitate = activeObjectsInUniverse[j].objGravitate;

                        if (gravitator != objToGravitate)
                        {
                            gravitator.ApplyGravity(objToGravitate);
                        }
                    }

                    gravitator.objPhysicsProperties.UpdatePhysicsProperties();
                }
            }
        }

        private void HandlePause()
        {
            for (int i = 0; i < activeObjectsInUniverse.Count; ++i)
            {
                PauseObj(activeObjectsInUniverse[i]);
                --i;
            }
        }

        private void HandleUnPause()
        {
            for (int i = 0; i < pausedObjectsInUniverse.Count; ++i)
            {
                UnPauseObj(pausedObjectsInUniverse[i]);
                --i;
            }
        }

        private void PauseObj(SpaceObject obj)
        {
            if (pausedObjectsInUniverse == null)
            {
                pausedObjectsInUniverse = new List<SpaceObject>();
            }

            if (!obj.IsPaused)
            {
                obj.objRigidbody.simulated = false;

                pausedObjectsInUniverse.Add(obj);
                activeObjectsInUniverse.Remove(obj);

                obj.IsPaused = true;
            }
        }

        private void UnPauseObj(SpaceObject obj)
        {
            if (obj.GetComponent<SpaceObject>().IsPaused)
            {
                obj.objRigidbody.simulated = true;

                pausedObjectsInUniverse.Remove(obj);
                activeObjectsInUniverse.Add(obj);

                obj.IsPaused = false;
            }
        }

        private void GhostObj(SpaceObject obj)
        {
            if (!obj)
            {
                return;
            }
            else if (ghostObject)
            {
                UnGhostObj();
            }

            ghostObject = obj;
            ghostObject.GetComponent<SpriteRenderer>().color = Color.blue;

            PauseObj(ghostObject);
        }

        private void UnGhostObj()
        {
            if (!ghostObject)
            {
                return;
            }
            else if (ghostObject == selectedObj)
            {
                ghostObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else
            {
                ghostObject.GetComponent<SpriteRenderer>().color = Color.white;
            }

            if (Managers.GameState.Instance.IsState(Managers.GameState.EnumGameState.RUNNING))
            {
                UnPauseObj(ghostObject);
            }

            ghostObject = null;
        }

        private void HandleTap(Touch touch)
        {
            TouchPositionToWorldVector3(touch, ref touchPosition);
            SpaceObject target = GetObjectAtPosition(touchPosition);

            //Tries to set the target. If it can't, the target is set to null.
            bool setTarget = SetSelectedObject(target);

            if (!setTarget)
            {
                SpawnAndSelectObject(touch);
                selectedObj.JustSpawned = false;
            }
        }

        private void HandleDragBegan(Touch touch)
        {
            //if (selectedObj)
            //{
            //    SetSelectedObject(null);
            //}

            SpawnAndSelectObject(touch);

            //Make a ghost method that pauses it in it and changes the image to be slightly transparent
            GhostObj(selectedObj);
            
            //else if (selectedObj.GetComponent<SpaceObject>().JustSpawned)
            //{
            //    //Get distance from selected object to touch position
            //    TouchPositionToWorldVector3(touch, ref dragPosition);

            //    Vector3 distance = dragPosition - selectedObj.transform.position;

            //    //Update the arrow image

            //    Debug.Log("DRAG BEGAN: " + distance);
            //}
        }

        private void HandleDragHeld(Touch touch)
        {
            if (selectedObj && selectedObj.JustSpawned)
            {
                //Get distance from selected object to touch position
                TouchPositionToWorldVector3(touch, ref dragPosition);

                //Vector3 distance = dragPosition - selectedObj.transform.position;

                //Update the arrow image

                //Debug.Log("DRAG HELD: " + distance);
            }
        }

        private void HandleDragEnded(Touch touch)
        {
            //Set the velocity to the vector created in the other drag events
            if (selectedObj && selectedObj.JustSpawned)
            {
                //Get distance from selected object to touch position
                TouchPositionToWorldVector3(touch, ref dragPosition);

                Vector3 distance = dragPosition - selectedObj.transform.position;

                //Remove the arrow image.

                //Set the velocity to the distance times a scale factor that works for the game.
                selectedObj.objRigidbody.velocity = distance;

                selectedObj.JustSpawned = false;

                UnGhostObj();

                //Debug.Log("DRAG ENDED: " + distance);
            }
        }

        private bool SetSelectedObject(SpaceObject target)
        {
            bool success = false;

            if (selectedObj)
            {
                selectedObj.GetComponent<SpriteRenderer>().color = Color.white;
            }

            if (target)
            {
                target.GetComponent<SpriteRenderer>().color = Color.yellow;
                SelectedObj = target;
                
                success = true;
            }
            else
            {
                SelectedObj = null;
            }

            return success;
        }

        private void SpawnAndSelectObject(Touch touch)
        {
            //If the tap is on an object, select the object (For now turn it yellow).
            TouchPositionToWorldVector3(touch, ref touchPosition);

            SpaceObject target = GetObjectAtPosition(touchPosition);

            //Tries to set the target. If it can't, the target is set to null.
            bool setTarget = SetSelectedObject(target);

            //If we couldn't set the target, spawn a new obj and set that as the target.
            if (!setTarget)
            {
                SpaceObject obj = SpawnObject(touch);
                SetSelectedObject(obj);
            }
        }

        private SpaceObject SpawnObject(Touch touch)
        {
            TouchPositionToWorldVector3(touch, ref spawnPosition);

            //Spawn the selected GUI object in the future.
            GameObject obj = Instantiate(planetPrefab/*selectedGUIObj*/, spawnPosition, Quaternion.identity);

            if (obj)
            {
                if (OnObjectSpawned != null)
                {
                    OnObjectSpawned();
                }

                return obj.GetComponent<SpaceObject>();
            }
            else
            {
                return null;
            }
        }

        private SpaceObject GetObjectAtPosition(Vector3 position)
        {
            SpaceObject target = null;
            Vector2 distance = Vector3.zero;

            foreach (SpaceObject obj in objectsInUniverse)
            {
                distance = position - obj.transform.position;

                if (distance.sqrMagnitude < obj.objPhysicsProperties.SqrRadius)
                {
                    target = obj;
                }
            }

            return target;
        }

        private void TouchPositionToWorldVector3(Touch touch, ref Vector3 position)
        {
            position = Camera.main.ScreenToWorldPoint(touch.position);
            position.z = OBJECT_Z_PLANE;
        }

        public SpaceObject GetMostMassiveObject()
        {
            SpaceObject mostMassive = null;

            if (objectsInUniverse.Count > 0)
            {
                mostMassive = objectsInUniverse[0];
            }

            //You could also merge sort this and then take the last element,
            //if you are worried about speed in the future. Although that might
            //be a lot of memory usage on a phone.
            foreach (SpaceObject obj in objectsInUniverse)
            {
                if (obj.objRigidbody.mass > mostMassive.objRigidbody.mass)
                {
                    mostMassive = obj;
                }
            }

            return mostMassive;
        }
    }
}
