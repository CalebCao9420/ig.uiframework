using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class SmallPopup : PopupPanel{
        public SmallPopupData PopupCommonData{ get; private set; }

        protected override void InitData(){
            base.InitData();
            this.transform.Find<Button>("OKButton").AddClick(this.Close);
        }

        protected override void ShowData(){
            base.ShowData();
            PopupCommonData = (_PopData as SmallPopupData);
            PopupCommonData.Title.BuildText("Title/Text (TMP)", this);
            PopupCommonData.Msg.BuildText("Text", this);
            PopupCommonData.Label.BuildText("OKButton/Text", this);
            this.RectTransform.sizeDelta = PopupCommonData.Size;
        }

        protected override bool OnClose(){
            PopupCommonData.CallBack?.Invoke();
            return base.OnClose();
        }
    }

    /// <summary>
    /// Generic PropUp data
    /// </summary>
    public class SmallPopupData : PopupData{
        public string        Title = "";
        public string        Msg   = "";
        public string        Label = null;
        public System.Action CallBack;
        public Vector2       Size = new Vector2(898, 310);
        public SmallPopupData(){ PopPanel = typeof(SmallPopup); }

        public SmallPopupData(string title, string msg, string label){
            PopPanel   = typeof(SmallPopup);
            this.Title = title;
            this.Msg   = msg;
            this.Label = label;
        }
    }
}