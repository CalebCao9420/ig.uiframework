using System;
using DG.Tweening;
using UnityEngine;

namespace IG.Module.UI{
    public class ViewAnimation : MonoBehaviour, IViewAnimation{
        public    Vector3 StartVector3 = Vector3.one;
        public    Vector3 EndVector3   = Vector3.one;
        public    float   MoveInTime   = 0.2f;
        public    float   MoveOutTime  = 0.2f;
        protected bool    _IsMoveIn;

        public IViewAnimation MoveIn(){
            _IsMoveIn = true;
            return this;
        }

        public IViewAnimation MoveOut(){
            _IsMoveIn = false;
            return this;
        }

        public Action OnStartHandler   { get; set; }
        public Action OnCompleteHandler{ get; set; }

        protected void InitEvent(Tweener tweener){
            // If TRUE the tween will ignore Unity's Time.timeScale. 
            tweener.SetUpdate(true);
            tweener.OnStart(
                            () => {
                                if (OnStartHandler != null){
                                    OnStartHandler();
                                    OnStartHandler = null;
                                }
                            }
                           ).
                    OnComplete(
                               () => {
                                   Destroy(this);
                                   if (OnCompleteHandler != null){
                                       OnCompleteHandler();
                                       OnCompleteHandler = null;
                                   }
                               }
                              );
        }

        private void OnDestroy(){
            if (OnStartHandler != null){
                OnStartHandler();
                OnStartHandler = null;
            }

            if (OnCompleteHandler != null){
                OnCompleteHandler();
                OnCompleteHandler = null;
            }
        }
    }
}