using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenArrowData
{
    private SpaceObject objSpaceObj;
    private Color arrowColor;

    private Vector2 onScreenPos;

    public Color ArrowColor { get { return arrowColor; } set { arrowColor = value; } }
    public SpaceObject ObjSpaceObj { get { return objSpaceObj; } }
    public Vector2 OnScreenPos { get { return onScreenPos; } set { onScreenPos = value; } }

    public OffscreenArrowData(SpaceObject spaceObj)
    {
        objSpaceObj = spaceObj;
    }

    public void SetPosition(float x, float y)
    {
        onScreenPos.x = x;
        onScreenPos.y = y;
    }

    private void UpdateColor()
    {
        //Change color based on 
        arrowColor = Color.white;
    }
}
