using DG.Tweening;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class AlphaViewAnimation : ViewAnimation{
        Graphic[] graphicList;

        // Use this for initialization
        void Start(){
            // graphicList = this.gameObject.GetComponentsInChildren<Graphic>();
            // Tweener tweener = null;
            // if (m_isMoveIn){
            //     tweener = graphicList[0].DOFlip().DOFade(0, moveInTime).From().SetEase(Ease.InQuad);
            //     for (int i = 1; i < graphicList.Length; i++){
            //         graphicList[i].DOFade(0, moveInTime).From().SetEase(Ease.InQuad);
            //     }
            // }
            // else{
            //     tweener = graphicList[0].DOFade(0, moveOutTime).SetEase(Ease.InQuad);
            //     //			this.InitEvent (tweener);
            //     for (int i = 1; i < graphicList.Length; i++){
            //         graphicList[i].DOFade(0, moveOutTime).SetEase(Ease.InQuad);
            //     }
            // }
            //
            // this.InitEvent(tweener);
        }
    }
}