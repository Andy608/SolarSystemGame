using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenArrowData
{
    private SpaceObject objSpaceObj;
    private Color arrowColor;

    private Color startColor = Color.white;
    private Color endColor = Color.red;

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

    public void UpdateColor()
    {
        //Change color based on distance to end
        float maxDistance = Managers.UniversePlaySpaceManager.UNIVERSE_BOUNDS;
        float diff = Camera.main.ViewportToWorldPoint((OnScreenPos)).magnitude;

        Debug.Log("Diff: " + diff + " Dist: " + maxDistance);

        arrowColor = Color.Lerp(startColor, endColor, diff / maxDistance);
    }
}
