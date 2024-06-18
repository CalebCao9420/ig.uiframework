using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IG.Module.UI{
    public class TMPLinkClick : MonoBehaviour, IPointerClickHandler{
        private TextMeshProUGUI _textMeshPro;
        private Camera          _camera;
        public  Action<string>  OnClickLinkID;

        void Awake(){
            _textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
            Canvas m_Canvas = gameObject.GetComponentInParent<Canvas>();
            if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                _camera = null;
            else
                _camera = m_Canvas.worldCamera;
        }

        public void OnPointerClick(PointerEventData eventData){
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textMeshPro, Input.mousePosition, _camera);
            if (linkIndex != -1){
                TMP_LinkInfo linkInfo = _textMeshPro.textInfo.linkInfo[linkIndex];
                if (OnClickLinkID != null){
                    OnClickLinkID(linkInfo.GetLinkID());
                }
            }
        }
    }
}