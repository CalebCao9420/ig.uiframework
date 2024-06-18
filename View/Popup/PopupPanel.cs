using IG.Runtime.Extensions;

namespace IG.Module.UI{
    /// <summary>
    /// Popup panel.
    /// </summary>
    public class PopupPanel : GamePanel{
        protected PopupData          _PopData;
        protected int                _BtnEventIndex = 0;
        private   PopupViewAnimation _popupViewAnimation;

        protected override void OnAwake(){
            base.OnAwake();
            _popupViewAnimation = this.GetOrAddComponent<PopupViewAnimation>();
        }

        protected override void InitData(){
            base.InitData();
            _PopData = PopupController.Instance.PopData;
        }

        protected override void ShowData(){
            base.ShowData();
            CommonMaskController.Show(this.gameObject, this.transform.parent);
            _PopData = PopupController.Instance.PopData;
        }

        public void CoverData(){ this.ShowData(); }

        public void Close(){
            PanelController.CloseTopPopupPanel();
            //		PopupController.ShowNextPopup ();
        }

        public void OnBtnEvent(int index){
            _BtnEventIndex = index;
            PanelController.CloseTopPopupPanel();
        }

        protected virtual void POnBtnEvent(int index){ }

        protected override void OnMoveOutComplete(MoveAnimationState value){
            base.OnMoveOutComplete(value);
            this.POnBtnEvent(_BtnEventIndex);
            PopupController.ShowNextPopup();
        }

        protected override void PlayMoveInAnimation(){
            if (_popupViewAnimation != null && _PopData.IsAnimation){
                _popupViewAnimation.OnCompleteHandler = () => {
                    base.PlayMoveInAnimation();
                };
                _popupViewAnimation.MoveIn();
            }
            else{
                base.PlayMoveInAnimation();
            }
        }

        protected override void PlayMoveOutAnimation(){
            //		base.PlayMoveOutAnimation ();
            if (_popupViewAnimation != null && _PopData.IsAnimation){
                _popupViewAnimation.OnCompleteHandler = () => {
                    base.PlayMoveOutAnimation();
                };
                _popupViewAnimation.MoveOut();
            }
            else{
                base.PlayMoveOutAnimation();
            }
        }
    }
}