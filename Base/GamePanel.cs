using System;

namespace IG.Module.UI{
    public class GamePanel : GameView{
        /// <summary>
        /// 上一个页面的类型
        /// </summary>
        public virtual Type LastGamePanelType{ get; set; }

        protected override void OnMoveOutComplete(MoveAnimationState value){
            //当状态等于销毁时
            if (value == MoveAnimationState.EndMoveOutAndDestroy){
                this.LastGamePanelType = null;
                gameObject.SetActive(false);
                IsActive = false;
            }

            //		foreach (GameView item in childViewStack) {
            //			item.gameObject.SetActive (false);
            //		}
            if (_OnMoveOutCompleteCallBack != null){
                _OnMoveOutCompleteCallBack(value);
                _OnMoveOutCompleteCallBack = null;
            }
        }

        protected override void OnBackKey() { PanelController.ClosePanel(this.GetType().BaseType); }
        protected          void ClosePanel(){ PanelController.ClosePanel(this.GetType().BaseType); }
    }
}