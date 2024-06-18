using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    [RequireComponent(typeof(ContentSizeFitter))]
    public class ContentImmediate : MonoBehaviour{
        private RectTransform _rectTransform;

        private RectTransform rectTransform{
            get{
                if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private ContentSizeFitter m_ContentSizeFitter;

        private ContentSizeFitter contentSizeFitter{
            get{
                if (m_ContentSizeFitter == null) m_ContentSizeFitter = GetComponent<ContentSizeFitter>();
                return m_ContentSizeFitter;
            }
        }

        //立即获取ContentSizeFitter的区域
        public Vector2 GetPreferredSize(){
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            return new Vector2(HandleSelfFittingAlongAxis(0), HandleSelfFittingAlongAxis(1));
        }

        private float HandleSelfFittingAlongAxis(int axis){
            UnityEngine.UI.ContentSizeFitter.FitMode fitting = (axis == 0 ? contentSizeFitter.horizontalFit : contentSizeFitter.verticalFit);
            if (fitting == UnityEngine.UI.ContentSizeFitter.FitMode.MinSize){
                return LayoutUtility.GetMinSize(rectTransform, axis);
            }
            else{
                return LayoutUtility.GetPreferredSize(rectTransform, axis);
            }
        }
    }
}