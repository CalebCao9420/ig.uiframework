using UnityEngine;

namespace IG.Module.UI{
    public class CommonWebViewPanel : DialogPanel{
        protected override void AddEvent() { base.AddEvent(); }
        protected override void OnDestroy(){ base.OnDestroy(); }

        protected override void InitData(){
            base.InitData();
            string url        = WebViewController.Instance.CommonUrl;
            var    margin     = UIUtility.GetAspectBaseMargin();
            var    spaceScale = Mathf.Max(Screen.width / 1920f, Screen.height                   / 1080f);
            webView.OpenUrl(url, (int)(window.x        * spaceScale) + margin.x, (int)(window.y * spaceScale) + margin.y, (int)(window.z * spaceScale) + margin.x, (int)(window.w * spaceScale) + margin.y);
        }

        public Vector4 window;

        protected override void ShowData(){
            base.ShowData();
            webView.SetVisibility(true);
        }

        public WebView webView;

        public void OnCloseBtnEvent(){
            this.Close();
            webView.SetVisibility(false);
        }
    }
}