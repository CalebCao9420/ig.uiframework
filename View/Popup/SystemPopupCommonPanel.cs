using IG.Runtime.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class SystemPopupCommonPanel : SystemPanel{
        public  TMP_Text           Title;
        public  Image              TitleBg;
        public  GameText           Desc;
        public  Button             OkButton;
        public  GameText           OkText;
        public  Color[]            TitleBgColor;
        private PopupViewAnimation _popupViewAnimation;

        protected override void OnAwake(){
            base.OnAwake();
            this._IsIgnoreLoading = true;
            _popupViewAnimation   = this.GetOrAddComponent<PopupViewAnimation>();
            OkButton.AddClickListener(this.OnOKButton);
        }

        protected override void ShowData(){
            base.ShowData();
            Title.text = SystemPopupController.Instance.PopupTitle;
            Desc.text  = SystemPopupController.Instance.PopupDesc;
            OkButton.gameObject.SetActive(true);
            OkText.text                  = SystemPopupController.Instance.OkText;
            TitleBg.color                = SystemPopupController.Instance.IsError ? TitleBgColor[1] : TitleBgColor[0];
            this.RectTransform.sizeDelta = SystemPopupController.Instance.IsError ? new Vector2(898, 396) : new Vector2(898, 310);
        }

        protected void OnOKButton(){
            this.Close();
            //this.SoundPlay("se018");
            //		if (SystemPopupController.GetInstance ().callBack != null) {
            //			SystemPopupController.GetInstance ().callBack ();
            //		}
        }

        protected override void OnMoveOutComplete(MoveAnimationState value){
            base.OnMoveOutComplete(value);
            if (SystemPopupController.Instance.CallBack != null){
                SystemPopupController.Instance.CallBack();
            }
        }

        protected override void PlayMoveInAnimation(){
            if (_popupViewAnimation != null){
                _popupViewAnimation.OnCompleteHandler = () => {
                    base.PlayMoveInAnimation();
                };
                _popupViewAnimation.MoveIn();
            }
        }

        protected override void PlayMoveOutAnimation(){
            if (_popupViewAnimation != null){
                _popupViewAnimation.OnCompleteHandler = () => {
                    base.PlayMoveOutAnimation();
                };
                _popupViewAnimation.MoveOut();
            }
        }
    }
}