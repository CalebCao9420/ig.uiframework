using System;
using IG.AssetBundle;
using IG.Module.UI.Res;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    /// <summary>
    /// Com PropUp
    /// </summary>
    public class PopupCommonPanel : PopupPanel{
        public  TextMeshProUGUI TitleTxt;
        public  GameText        DesText;
        private GridLayoutGroup Grid;
        public  GameButton[]    Btns;
        public  Sprite[]        BtnSprites;

        //public Material[] btnMaterials;
        public  float           BtnSpacing = 100;
        public  PopupCommonData PopupCommonData{ get; private set; }
        private PopupItemView   _popupItemView;

        protected override void OnAwake(){
            base.OnAwake();
            Grid = transform.Find<GridLayoutGroup>("Btns");
        }

        protected override void ShowData(){
            base.ShowData();
            PopupCommonData = (_PopData as PopupCommonData);
            this.ShowPopupItem();
            if (TitleTxt != null){
                TitleTxt.text = PopupCommonData.Title;
            }

            DesText.text = PopupCommonData.Msg;
            if (PopupCommonData.BtnCount == 2){
                Grid.spacing = new Vector2(BtnSpacing, 0f);
            }
            else{
                Grid.spacing = new Vector2(0f, 0f);
            }

            for (int i = 0; i < Btns.Length; i++){
                Btns[i].gameObject.SetActive(i < PopupCommonData.BtnCount);
                if (i < PopupCommonData.BtnCount){
                    Btns[i].Text.text    = PopupCommonData.Labels[i];
                    Btns[i].image.sprite = BtnSprites[(int)PopupCommonData.BtnSprites[i]];
                    //btns [i].FontSharedMaterial = btnMaterials [(int)popupCommonData.btnSprites [i]];
                }
            }

            _BtnEventIndex = PopupCommonData.BtnCount - 1;
            //this.rectTransform.sizeDelta = popupCommonData.size;
        }

        /// <summary>
        /// Shows the popup item.
        /// </summary>
        private void ShowPopupItem(){
            if (_popupItemView != null){
                Destroy(_popupItemView.gameObject);
            }

            if (_PopData.PopupItemData == null){
                return;
            }

            PopupCommonData.Title = _PopData.PopupItemData.Title;
            Transform  popupItemTransform = this.Find("PopupItem");
            PrefabInfo prefabInfo         = GamePrefabPaths.GetPath(_PopData.PopupItemData.PopupItemView);
            AssetsSystem.LoadAsync(
                                  (o, oArg) => {
                                      var        prefab = o as GameObject;
                                      GameObject obj    = Instantiate(prefab);
                                      _popupItemView = obj.GetComponent<PopupItemView>();
                                      obj.transform.SetParent(popupItemTransform);
                                      _popupItemView.SetParentView(this);
                                      _popupItemView.SetPopupItemData(_PopData.PopupItemData);
                                  },
                                  prefabInfo.path,
                                  typeof(GameObject)
                                 );
        }

        protected override void POnBtnEvent(int index){
            PopupCommonData popupCommonDataTemp = this.PopupCommonData;
            PopupItemView   popupItemViewTemp   = this._popupItemView;
            if (popupCommonDataTemp.CallBacks != null && popupCommonDataTemp.CallBacks[index] != null){
                popupCommonDataTemp.CallBacks[index]();
            }

            if (popupItemViewTemp != null && popupItemViewTemp.CallBacks != null && popupItemViewTemp.CallBacks[index] != null){
                popupItemViewTemp.CallBacks[index]();
            }
        }
    }

    /// <summary>
    /// Generic PropUp data
    /// </summary>
    public class PopupCommonData : PopupData{
        public string            Title      = "";
        public string            Msg        = "";
        public string[]          Labels     = null;
        public Action[]          CallBacks  = null;
        public EPopupBtnSprite[] BtnSprites = null;
        public int               BtnCount{ get; protected set; }
        public bool              IsCoverLast = false;

        // not used
        public Vector2 Size = new Vector2(1096, 700);

        public PopupCommonData(int btnCount = 2){
            this.BtnCount = btnCount;
            PopPanel      = typeof(PopupCommonPanel);
            CallBacks     = new Action[this.BtnCount];
            Labels        = new string[this.BtnCount];
            BtnSprites    = new EPopupBtnSprite[this.BtnCount];
        }

        public PopupCommonData(string title, string msg, int btnCount){
            this.Title    = title;
            this.Msg      = msg;
            this.BtnCount = btnCount;
            PopPanel      = typeof(PopupCommonPanel);
            CallBacks     = new Action[this.BtnCount];
            Labels        = new string[this.BtnCount];
            BtnSprites    = new EPopupBtnSprite[this.BtnCount];
        }
    }

    public enum EPopupBtnSprite{
        BLUE   = 0,
        ORANGE = 1,
        RED    = 2,
    }
}