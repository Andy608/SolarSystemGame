using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class InputHandler : ManagerBase<InputHandler>
    {
        public delegate void TapAction(Touch t);
        public static event TapAction OnTap;

        //The maximum pixels a tap can be dragged to count as a tap.
        public float maxTapMovement = 50.0f;
        private Vector2 dragMovement;

        private float sqrMaxTapMovement;

        //If movement is greated than maxTapMovement, the tap failed.
        private bool tapFailed = false;

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
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    dragMovement += touch.deltaPosition;

                    if (dragMovement.sqrMagnitude > sqrMaxTapMovement)
                    {
                        tapFailed = true;
                    }
                }
                else
                {
                    if (!tapFailed)
                    {
                        if (OnTap != null)
                        {
                            OnTap(touch);
                        }

                        tapFailed = false;
                    }
                }
            }
        }
    }
}