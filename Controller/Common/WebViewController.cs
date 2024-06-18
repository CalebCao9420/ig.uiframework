using IG.Module.UI.Controller;

namespace IG.Module.UI{
    public class WebViewController : GameControllerBase<WebViewController>{
        public string CommonUrl{ get; private set; }

        public void ShowCommonWebView(string url){
            CommonUrl = url;
            PanelController.ShowPanel<CommonWebViewPanel>();
        }
    }
}