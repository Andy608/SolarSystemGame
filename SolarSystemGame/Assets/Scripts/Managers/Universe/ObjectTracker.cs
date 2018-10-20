using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    //Responsible for holding a list of all the objects in the universe.
    //Responsible for tracking current selected obj
    //Responsible for tracking current ghost obj
    public class ObjectTracker : ManagerBase<ObjectTracker>
    {
        public delegate void SelectedObjectChangeAction();
        public static event SelectedObjectChangeAction OnSelectedObjectChanged;

        public delegate void SpawnObjectAction(SpaceObject spawnedObj);
        public static event SpawnObjectAction OnObjectSpawned;

        //Plane that all objects are on.
        private const float OBJECT_Z_PLANE = 0;

        //The list of all the objects that are affected by gravity.
        private List<SpaceObject> objectsInUniverse = new List<SpaceObject>();
        private List<SpaceObject> activeObjectsInUniverse = new List<SpaceObject>();
        private List<SpaceObject> pausedObjectsInUniverse = new List<SpaceObject>();

        private Color tempGhostColor;
        private Color tempSelectedColor;

        //The object in the process of being created right now.
        private SpaceObject ghostObject = null;

        //Holds the most massive object in the scene.
        private SpaceObject mostMassiveObj = null;
        private SpaceObject selectedObj = null;

        private static Vector3 spawnPosition = new Vector3();
        private static Vector3 touchPosition = new Vector3();
        private static Vector3 dragPosition = new Vector3();

        private SpaceObject spawnObjPrefab;

        public List<SpaceObject> ObjectsInUniverse { get { return objectsInUniverse; } }
        public List<SpaceObject> ActiveObjectsInUniverse { get { return activeObjectsInUniverse; } }

        public SpaceObject SelectedObj
        {
            get { return selectedObj; }

            set
            {
                selectedObj = value;

                if (selectedObj)
                {
                    tempSelectedColor = selectedObj.GetComponent<SpriteRenderer>().color;
                }

                if (OnSelectedObjectChanged != null)
                {
                    OnSelectedObjectChanged();
                }
            }
        }

        public SpaceObject MostMassiveObj { get { return mostMassiveObj; } }

        private void OnEnable()
        {
            GameState.OnGamePaused += HandlePause;
            GameState.OnGameUnPaused += HandleUnPause;

            InputHandler.OnTap += HandleTap;

            InputHandler.OnDragBegan += HandleDragBegan;
            InputHandler.OnDragHeld += HandleDragHeld;
            InputHandler.OnDragEnded += HandleDragEnded;

            UniversePlaySpaceManager.OnObjectDestroyed += HandleObjectDestroyed;

            SpaceObjectUI.OnUIObjectSelected += UpdateObjectToSpawn;
        }

        private void OnDisable()
        {
            GameState.OnGamePaused -= HandlePause;
            GameState.OnGameUnPaused -= HandleUnPause;

            InputHandler.OnTap -= HandleTap;

            InputHandler.OnDragBegan -= HandleDragBegan;
            InputHandler.OnDragHeld -= HandleDragHeld;
            InputHandler.OnDragEnded -= HandleDragEnded;

            UniversePlaySpaceManager.OnObjectDestroyed -= HandleObjectDestroyed;

            SpaceObjectUI.OnUIObjectSelected -= UpdateObjectToSpawn;
        }

        private void Start()
        {
            InitDefaultObjectToSpawn();
        }

        public void RegisterObject(SpaceObject spaceObj)
        {
            objectsInUniverse.Add(spaceObj);
            activeObjectsInUniverse.Add(spaceObj);

            if (GameState.Instance.IsState(GameState.EnumGameState.PAUSED))
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
            if (GameState.Instance.IsState(GameState.EnumGameState.RUNNING))
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
            tempGhostColor = ghostObject.GetComponent<SpriteRenderer>().color;
            ghostObject.GetComponent<SpriteRenderer>().color = Color.blue;
            ghostObject.GetComponent<OrbitTrajectory>().Show();

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
                ghostObject.GetComponent<SpriteRenderer>().color = tempGhostColor;
            }

            ghostObject.GetComponent<OrbitTrajectory>().Hide();

            if (GameState.Instance.IsState(GameState.EnumGameState.RUNNING))
            {
                UnPauseObj(ghostObject);
            }

            ghostObject = null;
        }

        private void HandleTap(Touch touch)
        {
            if (InputHandler.IsPointerOverUIObject()) return;

            TouchPositionToWorldVector3(touch, ref touchPosition);
            SpaceObject target = GetObjectAtPosition(touchPosition);

            if (target)
            {
                SetSelectedObject(target);
            }
            else
            {
                SpaceObject obj = SpawnObject(touch);
                if (obj)
                {
                    obj.IsJustSpawned = true;
                }

                SetSelectedObject(null);
            }
        }

        private void HandleDragBegan(Touch touch)
        {
            if (InputHandler.IsPointerOverUIObject()) return;

            TouchPositionToWorldVector3(touch, ref touchPosition);
            SpaceObject target = GetObjectAtPosition(touchPosition);

            if (!target)
            {
                GhostObj(SpawnObject(touch));
            }
        }

        private void HandleDragHeld(Touch touch)
        {
            if (selectedObj && selectedObj.IsJustSpawned)
            {
                //Get distance from selected object to touch position
                TouchPositionToWorldVector3(touch, ref dragPosition);
            }
        }

        private void HandleDragEnded(Touch touch)
        {
            //Set the velocity to the vector created in the other drag events
            if (ghostObject && ghostObject.IsJustSpawned)
            {
                //Get distance from selected object to touch position
                TouchPositionToWorldVector3(touch, ref dragPosition);

                Vector2 distance = ghostObject.transform.position - dragPosition;

                //Remove the arrow image.

                ghostObject.IsJustSpawned = false;

                //Set the velocity to the distance times a scale factor that works for the game.
                ghostObject.objRigidbody.velocity += distance;

                UnGhostObj();
            }
        }

        private bool SetSelectedObject(SpaceObject target)
        {
            bool success = false;

            if (selectedObj)
            {
                selectedObj.GetComponent<SpriteRenderer>().color = tempSelectedColor;
            }

            if (target)
            {
                SelectedObj = target;
                target.GetComponent<SpriteRenderer>().color = Color.yellow;

                success = true;
            }
            else
            {
                SelectedObj = null;
            }

            return success;
        }

        //private void SpawnAndSelectObject(Touch touch)
        //{
        //    //If the tap is on an object, select the object (For now turn it yellow).
        //    TouchPositionToWorldVector3(touch, ref touchPosition);

        //    SpaceObject target = GetObjectAtPosition(touchPosition);

        //    //Tries to set the target. If it can't, the target is set to null.
        //    bool setTarget = SetSelectedObject(target);

        //    //If we couldn't set the target, spawn a new obj and set that as the target.
        //    if (!setTarget)
        //    {
        //        SpaceObject obj = SpawnObject(touch);
        //        if (obj)
        //        {
        //            SetSelectedObject(obj);
        //        }
        //    }
        //}

        private SpaceObject SpawnObject(Touch touch)
        {
            TouchPositionToWorldVector3(touch, ref spawnPosition);

            if (!spawnObjPrefab)
            {
                Debug.Log("There is no selected object to spawn!");
                return null;
            }

            if (MoneyManager.Instance.Funds <= InventoryManager.Instance.GetCost(spawnObjPrefab.objSpaceObjectType.Type))
            {
                Debug.Log("Not enough money to spawn in object!");
                return null;
            }

            GameObject obj = Instantiate(spawnObjPrefab.gameObject, spawnPosition, Quaternion.identity);

            if (obj)
            {
                SpaceObject objSpaceObj = obj.GetComponent<SpaceObject>();
                objSpaceObj.objPhysicsProperties.UpdateMass(InventoryManager.Instance.GetMass(objSpaceObj.objSpaceObjectType.Type));

                UpdateMostMassiveObject();

                if (OnObjectSpawned != null)
                {
                    OnObjectSpawned(objSpaceObj);
                }

                return objSpaceObj;
            }
            else
            {
                return null;
            }
        }

        private void UpdateObjectToSpawn(EnumObjectType type, float cost, float mass)
        {
            Debug.Log("UPDATE OBJECT TO SPAWN: " + type);
            spawnObjPrefab = ObjectStore.Instance.GetSpaceObjectPrefab(type);
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

        public void TouchPositionToWorldVector3(Touch touch, ref Vector3 position)
        {
            position = Camera.main.ScreenToWorldPoint(touch.position);
            position.z = OBJECT_Z_PLANE;
        }

        public void UpdateMostMassiveObject()
        {
            if (objectsInUniverse.Count == 0)
            {
                mostMassiveObj = null;
                return;
            }
            else
            {
                mostMassiveObj = objectsInUniverse[0];

                //You could also merge sort this and then take the last element,
                //if you are worried about speed in the future. Although that might
                //be a lot of memory usage on a phone.
                foreach (SpaceObject obj in objectsInUniverse)
                {
                    if (mostMassiveObj != obj)
                    {
                        if (mostMassiveObj.objRigidbody.mass < obj.objRigidbody.mass)
                        {
                            mostMassiveObj = obj;
                        }
                    }
                }
            }
        }

        private void HandleObjectDestroyed(SpaceObject destroyedObj)
        {
            UpdateMostMassiveObject();
        }

        private void InitDefaultObjectToSpawn()
        {
            List<EnumObjectType> unlockedObjects = UnlockablesManager.Instance.UnlockedSpaceObjects;

            if (unlockedObjects.Count > 0)
            {
                spawnObjPrefab = ObjectStore.Instance.GetSpaceObjectPrefab(unlockedObjects[0]);
            }
            else
            {
                Debug.Log("UH OH The list is empty so we can't set a default spawn object.");
                spawnObjPrefab = null;
            }
        }
    }
}
