using DG.Tweening;

namespace IG.Module.UI{
    public class ScaleViewAnimation : ViewAnimation{
        // Use this for initialization
        void Start(){
            this.transform.localScale = StartVector3;
            if (_IsMoveIn){
                this.InitEvent(this.transform.DOScale(EndVector3, MoveInTime));
            }
            else{
                this.InitEvent(this.transform.DOScale(EndVector3, MoveOutTime));
            }
        }
    }
}