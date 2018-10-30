using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class OffscreenArrowManager : ManagerBase<OffscreenArrowManager>
    {
        [SerializeField] private RectTransform arrowPanel;
        [SerializeField] private Image offscreenArrowPrefab;

        private Vector3 screenPos;
        private float maxOffset;

        private readonly Vector2 OFFSET_TRANSFORM = new Vector2(0.5f, 0.5f);

        private Dictionary<OffscreenArrowData, Image> arrows = new Dictionary<OffscreenArrowData, Image>();

        public void OnEnable()
        {
            UniversePlaySpaceManager.OnObjectDestroyed += RemoveObjectFromArrows;
        }

        public void OnDisable()
        {
            UniversePlaySpaceManager.OnObjectDestroyed -= RemoveObjectFromArrows;
        }

        private void Update()
        {
            foreach (SpaceObject currentObj in ObjectTracker.Instance.ObjectsInUniverse)
            {
                screenPos = Camera.main.WorldToViewportPoint(currentObj.transform.position);

                KeyValuePair<OffscreenArrowData, Image> currentArrowPair = default(KeyValuePair<OffscreenArrowData, Image>);
                bool isPairInList = GetArrowPair(currentObj, ref currentArrowPair);

                if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1)
                {
                    if (isPairInList)
                    {
                        RemoveObjectFromArrows(currentObj);
                    }
                
                    continue;
                }
                else if (!isPairInList)
                {
                    currentArrowPair = AddObjectToArrows(currentObj);
                }

                OffscreenArrowData currentArrowData = currentArrowPair.Key;
                Image currentArrowImage = currentArrowPair.Value;
                currentArrowData.SetPosition(screenPos.x - OFFSET_TRANSFORM.x, screenPos.y - OFFSET_TRANSFORM.y);
                currentArrowData.OnScreenPos *= 2.0f;

                maxOffset = Mathf.Max(Mathf.Abs(currentArrowData.OnScreenPos.x), Mathf.Abs(currentArrowData.OnScreenPos.y)); //get largest offset
                currentArrowData.OnScreenPos = (currentArrowData.OnScreenPos / (maxOffset)) /*+ OFFSET_TRANSFORM*/; //undo mapping

                currentArrowImage.rectTransform.localPosition = Camera.main.ViewportToScreenPoint(currentArrowData.OnScreenPos);

                Vector3 diff = currentObj.transform.position - currentArrowImage.rectTransform.localPosition;
                diff.Normalize();
                float rotationZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

                //currentArrowData.UpdateColor();

                currentArrowImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotationZ - 180.0f);
                //currentArrowImage.GetComponent<Image>().color = currentArrowData.ArrowColor;
            }
        }

        private bool GetArrowPair(SpaceObject obj, ref KeyValuePair<OffscreenArrowData, Image> pair)
        {
            foreach (KeyValuePair<OffscreenArrowData, Image> arrowPair in arrows)
            {
                if (arrowPair.Key.ObjSpaceObj == obj)
                {
                    pair = arrowPair;
                    return true;
                }
            }

            return false;
        }

        private KeyValuePair<OffscreenArrowData, Image> AddObjectToArrows(SpaceObject obj)
        {
            OffscreenArrowData arrow = new OffscreenArrowData(obj);
            Image arrowImage = Instantiate(offscreenArrowPrefab) as Image;
            arrowImage.rectTransform.SetParent(arrowPanel);
            Vector3 scale = arrowImage.rectTransform.localScale;
            scale.Normalize();
            scale *= 0.5f;
            scale.x *= 0.7f;
            arrowImage.rectTransform.localScale = scale;
            arrows.Add(arrow, arrowImage);

            KeyValuePair<OffscreenArrowData, Image> pair = new KeyValuePair<OffscreenArrowData, Image>(arrow, arrowImage);
            return pair;
        }

        private void RemoveObjectFromArrows(SpaceObject obj)
        {
            foreach (KeyValuePair<OffscreenArrowData, Image> arrowPair in arrows)
            {
                if (arrowPair.Key.ObjSpaceObj == obj)
                {
                    Debug.Log("DELETE");
                    Destroy(arrowPair.Value.gameObject);
                    arrows.Remove(arrowPair.Key);
                    return;
                }
            }
        }
    }
}
