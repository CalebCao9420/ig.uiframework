using UnityEngine;
using DG.Tweening;

namespace IG.Module.UI{
    public class ScaleViewAtTargetAnimation : ViewAnimation{
        public Transform Target;

        // Use this for initialization
        private void Start(){
            Target.localScale = StartVector3;
            if (_IsMoveIn){
                this.InitEvent(Target.DOScale(EndVector3, MoveInTime));
            }
            else{
                this.InitEvent(Target.DOScale(EndVector3, MoveOutTime));
            }
        }
    }
}