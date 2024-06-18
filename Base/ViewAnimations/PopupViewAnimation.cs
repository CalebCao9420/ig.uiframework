using UnityEngine;

namespace IG.Module.UI{
    [RequireComponent(typeof(Animation))]
    public class PopupViewAnimation : MonoBehaviour, IViewAnimation{
        Animation      Animation = null;
        AnimationState State     = null;
        private void   Awake(){ Animation = this.GetComponent<Animation>(); }

        public IViewAnimation MoveIn(){
            if (Animation != null && Animation.clip != null){
                State       = Animation.PlayQueued(Animation.clip.name);
                State.speed = 1;
                State.time  = 0;
                this.Invoke("MoveInComplete", State.length);
                OnStartHandler?.Invoke();
            }
            else{
                MoveInComplete();
                OnStartHandler?.Invoke();
            }

            return this;
        }

        public IViewAnimation MoveOut(){
            if (Animation != null && Animation.clip != null){
                State       = Animation.PlayQueued(Animation.clip.name);
                State.speed = -1;
                State.time  = Animation.clip.length;
                this.Invoke("MoveOutComplete", State.length);
                OnStartHandler?.Invoke();
            }
            else{
                MoveOutComplete();
                OnStartHandler?.Invoke();
            }

            return this;
        }

        private void MoveInComplete(){
            if (OnCompleteHandler != null){
                OnCompleteHandler();
            }
        }

        private void MoveOutComplete(){
            if (OnCompleteHandler != null){
                OnCompleteHandler();
            }
        }

        public System.Action OnStartHandler   { get; set; }
        public System.Action OnCompleteHandler{ get; set; }
    }
}