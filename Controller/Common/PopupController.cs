using System;
using System.Collections.Generic;
using IG.Module.UI.Controller;
using UnityEngine;

namespace IG.Module.UI{
    public class PopupController : GameControllerBase<PopupController>{
        private readonly List<PopupData>  _popupDataLis = new();
        private          PopupCommonPanel _currentPopup;
        public           PopupData        PopData{ get; private set; }

        private void PShow(PopupData messageData, int index = -1){
            if (messageData != null){
                if (index < 0 || _popupDataLis.Count <= index){
                    _popupDataLis.Add(messageData);
                }
                else{
                    _popupDataLis.Insert(index, messageData);
                }
            }

            if (this.PopData == null){
                PShowNextPopup();
            }
        }

        private void PCoverLastData(PopupData messageData){
            if (_currentPopup == null){
                PShow(messageData);
            }
            else{
                this.PopData = messageData;
                _currentPopup.CoverData();
            }
        }

        private void PShowNextPopup(){
            this.PopData       = null;
            this._currentPopup = null;
            if (_popupDataLis.Count > 0){
                this.PopData = _popupDataLis[0];
                _popupDataLis.RemoveAt(0);
            }

            if (this.PopData != null){
                PanelController.ShowPanel(this.PopData.PopPanel, c => { _currentPopup = c.GetComponent<PopupCommonPanel>(); });
            }
        }

        public static void ClearAllPopup(){ PopupController.Instance._popupDataLis.Clear(); }
        public static void ShowNextPopup(){ PopupController.Instance.PShowNextPopup(); }

        public static void Show(PopupData messageData, int index = -1){ PopupController.Instance.PShow(messageData, index); }
        // public static void Show(PopupItemData messageData, int     index           = -1){ Show(messageData, "OK".Localized(), "Cancel".Localized(), true,                 index); }
        // public static void Show(PopupItemData messageData, Vector2 size, int index = -1){ Show(messageData, size,             "OK".Localized(),     "Cancel".Localized(), index); }

        //TODO:注意多语言
        public static void Show(PopupItemData messageData, int     index           = -1){ Show(messageData, "OK", "Cancel", true,     index); }
        public static void Show(PopupItemData messageData, Vector2 size, int index = -1){ Show(messageData, size, "OK",     "Cancel", index); }

        public static void Show(PopupItemData popupItemData, Vector2 size, string enterLabel, string cancelLabel, int index = -1){
            PopupCommonData popupCommonData = new PopupCommonData();
            popupCommonData.PopupItemData = popupItemData;
            popupCommonData.BtnSprites[0] = EPopupBtnSprite.ORANGE;
            popupCommonData.Labels[0]     = enterLabel;
            popupCommonData.Labels[1]     = cancelLabel;
            popupCommonData.Size          = size;
            PopupController.Instance.PShow(popupCommonData, index);
        }

        public static void Show(PopupItemData popupItemData, string enterLabel, string cancelLabel, bool isAnimation = true, int index = -1){
            PopupCommonData popupCommonData = new PopupCommonData();
            popupCommonData.PopupItemData = popupItemData;
            popupCommonData.BtnSprites[0] = EPopupBtnSprite.ORANGE;
            popupCommonData.Labels[0]     = enterLabel;
            popupCommonData.Labels[1]     = cancelLabel;
            popupCommonData.IsAnimation   = isAnimation;
            PopupController.Instance.PShow(popupCommonData, index);
        }

        public static void Show(string title, string msg, string enterLabel, string cancelLabel, Action okCallBack, Action closeCallBack = null){
            PopupCommonData popupCommonData = new PopupCommonData(title, msg, 2);
            popupCommonData.BtnSprites[0] = EPopupBtnSprite.ORANGE;
            popupCommonData.Labels[0]     = enterLabel;
            popupCommonData.Labels[1]     = cancelLabel;
            popupCommonData.CallBacks[0]  = okCallBack;
            popupCommonData.CallBacks[1]  = closeCallBack;
            PopupController.Instance.PShow(popupCommonData);
        }

        public static void Show(string title, string msg, Action okCallBack, Action closeCallBack = null, bool isAnimation = true){
            PopupCommonData popupCommonData = new PopupCommonData(title, msg, 2);
            popupCommonData.BtnSprites[0] = EPopupBtnSprite.ORANGE;
            // popupCommonData.labels[0]     = "OK".Localized();
            // popupCommonData.labels[1]     = "Cancel".Localized();
            //TODO:注意多语言处理
            popupCommonData.Labels[0]    = "OK";
            popupCommonData.Labels[1]    = "Cancel";
            popupCommonData.CallBacks[0] = okCallBack;
            popupCommonData.CallBacks[1] = closeCallBack;
            popupCommonData.IsAnimation  = isAnimation;
            Show(popupCommonData);
        }

        public static void ShowSingleBtn(string title, string msg){
            PopupCommonData popupCommonData = new PopupCommonData(title, msg, 1);
            // popupCommonData.labels [0] = "OK".Localized ();
            //TODO:注意多语言处理
            popupCommonData.Labels[0] = "OK";
            Show(popupCommonData);
        }

        public static void ShowSingleBtn(string title, string msg, string enterLabel, Action callBack, bool isCoverLast = false){
            PopupCommonData popupCommonData = new PopupCommonData(title, msg, 1);
            popupCommonData.Labels[0]     = enterLabel;
            popupCommonData.CallBacks[0]  = callBack;
            popupCommonData.BtnSprites[0] = EPopupBtnSprite.ORANGE;
            popupCommonData.IsCoverLast   = isCoverLast;
            if (popupCommonData.IsCoverLast){
                PopupController.Instance.PCoverLastData(popupCommonData);
            }
            else{
                Show(popupCommonData);
            }
        }

        public static void ShowSingleBtn(string title, string msg, Action callBack, bool isCoverLast = false){
            // ShowSingleBtn(title, msg, "OK".Localized(), callBack, isCoverLast);
            //TODO:多语言处理
            ShowSingleBtn(title, msg, "OK", callBack, isCoverLast);
        }

        public static void ShowNoBtn(string title, string msg){
            PopupCommonData popupCommonData = new PopupCommonData(title, msg, 0);
            Show(popupCommonData);
        }
    }

    public enum EPopupSize{
        SMALL = 1,
        LARGE = 2,
        MINI  = 3
    }
}