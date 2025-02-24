﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //The maximum pixels a tap can be dragged to count as a tap.
        public float maxTapMovement = 50.0f;
        private float sqrMaxTapMovement;

        private Vector2 dragMovement;

        public float dragMinTime = 0.1f;
        private float startTime;

        //If movement is greated than maxTapMovement, the tap failed.
        private bool tapFailed = false;
        private bool dragRecognized = false;

        private void Start()
        {
            sqrMaxTapMovement = maxTapMovement * maxTapMovement;
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                //This only takes into account the first touch that happens. 
                //We can change this in the future to a loop if we want to handle multiple taps.
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
                        tapFailed = true;

                        if (OnDragBegan != null)
                        {
                            OnDragBegan(touch);
                        }
                    }
                    else if (dragRecognized)
                    {
                        if (OnDragHeld != null)
                        {
                            OnDragHeld(touch);
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
                        }
                    }
                    else if (!tapFailed)
                    {
                        if (OnTap != null)
                        {
                            OnTap(touch);
                        }
                    }

                    tapFailed = false;
                    dragRecognized = false;
                }
            }
        }
    }
}