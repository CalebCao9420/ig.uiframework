using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IG.Module.UI{
    public class UIUtility{
        private static PointerEventData    eventDataCurrentPosition;
        private static List<RaycastResult> results;

        public static bool IsPointerOverUIObject(){
            eventDataCurrentPosition          = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            results                           = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return (results.Count > 0);
        }

        /// <summary>
        /// Get display margin.
        /// </summary>
        public static Vector2Int GetAspectBaseMargin(){
            const float SCREEN_RATIO_X = 16f;
            const float SCREEN_RATIO_Y = 9f;
            var         margin         = new Vector2Int(0, 0);
            var         screenWidth    = (float)Screen.width;
            var         screenHeight   = (float)Screen.height;
            if (screenWidth / screenHeight > SCREEN_RATIO_X / SCREEN_RATIO_Y){
                margin.x = (int)(screenWidth - (screenHeight / SCREEN_RATIO_Y * SCREEN_RATIO_X)) / 2;
            }
            else{
                margin.y = (int)(screenHeight - (screenWidth / SCREEN_RATIO_X * SCREEN_RATIO_Y)) / 2;
            }

            return margin;
        }
    }
}