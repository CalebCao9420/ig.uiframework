using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace IG.Module.UI{
    public class ScrollViewPoint : MonoBehaviour, IPointerClickHandler{
        public  GameObject  SelectedGO;
        public  Vector2     Size;
        private Action<int> _onClick;
        private int         _index;

        public void Init(int index, Action<int> onClick){
            this._onClick = onClick;
            this._index   = index;
        }

        public void OnPointerClick(PointerEventData eventData){
            OnSelected(this._index);
            if (this._onClick != null){
                this._onClick(this._index);
            }
        }

        public void OnSelected(int index){ SelectedGO.SetActive(this._index == index); }
    }
}