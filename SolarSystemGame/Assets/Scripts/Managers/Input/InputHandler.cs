using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managers
{
    public class InputHandler : ManagerBase<InputHandler>
    {
        public delegate void TapAction(Touch t);
        public static event TapAction OnTap;

        public delegate void DragBegan(Touch t);
        public static event DragBegan OnDragBegan;

        public delegate void DragHeld(Touch t);
        public static event DragHeld OnDragHeld;

        public delegate void DragEnd(Touch t);
        public static event DragEnd OnDragEnded;

        public delegate void PinchBegan(Touch first, Touch second);
        public static event PinchBegan OnPinchBegan;

        public delegate void PinchHeld(Touch first, Touch second);
        public static event PinchHeld OnPinchHeld;

        public delegate void PinchEnd(Touch first, Touch second);
        public static event PinchEnd OnPinchEnded;

        //The maximum pixels a tap can be dragged to count as a tap.
        public float maxTapMovement = 50.0f;
        private float sqrMaxTapMovement;

        private Vector2 dragMovement;

        public float dragMinTime = 0.1f;
        public float pinchMinTime = 0.1f;
        private float startTime;

        //If movement is greated than maxTapMovement, the tap failed.
        private bool tapFailed = false;
        private bool dragRecognized = false;
        private bool pinchRecognized = false;

        private void Start()
        {
            sqrMaxTapMovement = maxTapMovement * maxTapMovement;
        }

        private void Update()
        {
            if (Input.touchCount == 2)
            {
                CheckForPinch();
            }
            else if (Input.touchCount > 0)
            {
                if (!pinchRecognized)
                {
                    CheckForDrag();
                }
            }
            else
            {
                dragRecognized = false;
                pinchRecognized = false;
                tapFailed = false;
            }
        }

        //First Priority
        private void CheckForPinch()
        {
            Touch firstTouch = Input.touches[0];
            Touch secondTouch = Input.touches[1];

            if (firstTouch.phase == TouchPhase.Began && secondTouch.phase == TouchPhase.Began)
            {
                dragMovement = Vector2.zero;
                startTime = Time.time;
            }
            else if ((firstTouch.phase == TouchPhase.Moved || firstTouch.phase == TouchPhase.Stationary) && (secondTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Stationary))
            {
                if (!pinchRecognized && Time.time - startTime > pinchMinTime)
                {
                    pinchRecognized = true;
                    dragRecognized = false;
                    tapFailed = true;

                    Debug.Log("PINCH BEGAN");
                    if (OnPinchBegan != null)
                    {
                        OnPinchBegan(firstTouch, secondTouch);
                    }
                }
                else if (pinchRecognized)
                {
                    Debug.Log("PINCH HELD");
                    if (OnPinchHeld != null)
                    {
                        OnPinchHeld(firstTouch, secondTouch);
                    }
                }
                else if (dragMovement.sqrMagnitude > sqrMaxTapMovement)
                {
                    tapFailed = true;
                }
            }
            else
            {
                if (pinchRecognized)
                {
                    Debug.Log("PINCH ENDED");
                    if (OnPinchEnded != null)
                    {
                        OnPinchEnded(firstTouch, secondTouch);
                    }
                }
            }
        }

        //Second Priority
        private void CheckForDrag()
        {
            Touch touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                dragMovement = Vector2.zero;
                startTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                dragMovement += touch.deltaPosition;

                if (!dragRecognized && Time.time - startTime > dragMinTime)
                {
                    dragRecognized = true;
                    pinchRecognized = false;
                    tapFailed = true;

                    if (OnDragBegan != null)
                    {
                        OnDragBegan(touch);
                        Debug.Log("DRAG BEGAN");
                    }
                }
                else if (dragRecognized)
                {
                    if (OnDragHeld != null)
                    {
                        OnDragHeld(touch);
                        Debug.Log("DRAG HELD");
                    }
                }
                else if (dragMovement.sqrMagnitude > sqrMaxTapMovement)
                {
                    tapFailed = true;
                }
            }
            else
            {
                if (dragRecognized)
                {
                    if (OnDragEnded != null)
                    {
                        OnDragEnded(touch);
                        Debug.Log("DRAG ENDED");
                    }
                }
                else
                {
                    CheckForTap();
                }

                //dragRecognized = false;
                //pinchRecognized = false;
                //tapFailed = false;
            }
        }

        //Last Priority
        private void CheckForTap()
        {
            if (!tapFailed)
            {
                if (OnTap != null)
                {
                    OnTap(Input.touches[0]);
                    Debug.Log("TAP HAPPENED");
                }
            }
        }

        //Found on https://answers.unity.com/questions/1073979/android-touches-pass-through-ui-elements.html
        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}