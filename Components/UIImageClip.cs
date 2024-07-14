using System;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class UIImageClip : Selectable, ICanvasElement{
        [SerializeField]
        public Sprite[] Images;

        public  bool           SetNativeSize = true;
        private Image          Clip          = null;
        private int            _clipIndex    = -1;
        private UISelectable   _uiSeltable;
        private Action         _callback       = null;
        private Action<object> _callback_param = null;
        private object         _param          = null;

        protected override void Awake(){
            base.Start();
            Clip = gameObject.GetComponent<Image>();
            if (null != _callback){
                _uiSeltable = Clip.AddClick(_callback);
            }
            else if (null != _callback_param){
                _uiSeltable = Clip.AddClick(_callback_param, _param);
            }

            if (_clipIndex >= 0){
                if (null != Clip && null != Images && _clipIndex < Images.Length){
                    Clip.sprite = Images[_clipIndex];
                    if (SetNativeSize){
                        Clip.SetNativeSize();
                    }
                }
            }
        }

        public void AddClick(Action callback){
            if (null != Clip){
                _uiSeltable = Clip.AddClick(callback);
            }
            else{
                this._callback = callback;
            }
        }

        public void AddClick(Action<object> callback, object data){
            if (null != Clip){
                _uiSeltable = Clip.AddClick(callback, data);
            }
            else{
                _callback_param = callback;
                _param          = data;
            }
        }

        public void RemoveClick(bool removeSelect = true){
            if (null != Clip){
                Clip.RemoveClick(removeSelect);
            }
            else{
                this._callback = null;
            }
        }

        public virtual void Rebuild(CanvasUpdate executing){ }
        public         void GraphicUpdateComplete()        { }
        public         void LayoutComplete()               { }

        public void ChangeClipByIndex(int index){
            _clipIndex = index;
            if (_clipIndex < 0){
                _clipIndex = 0;
            }

            if (null != Clip && null != Images && _clipIndex < Images.Length){
                Clip.sprite = Images[_clipIndex];
                if (SetNativeSize){
                    Clip.SetNativeSize();
                }
            }
        }

        public int ClipIndex{ get{ return _clipIndex; } }
    }
}