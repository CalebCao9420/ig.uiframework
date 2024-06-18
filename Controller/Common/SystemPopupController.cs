using System;
using IG.Module.UI.Controller;

namespace IG.Module.UI{
    public class SystemPopupController : GameControllerBase<SystemPopupController>{
        public string PopupTitle{ get; private set; }
        public string PopupDesc { get; private set; }
        public string OkText    { get; private set; }
        public Action CallBack  { get; private set; }
        public bool   IsError   { get; private set; }
        public SystemPopupController(){ }

        public void Show(string title, string desc, string btnTxt, bool isError, Action action){
            PopupTitle = title;
            PopupDesc  = desc;
            CallBack   = action;
            OkText     = btnTxt;
            IsError    = isError;
            PanelController.ShowPanel<SystemPopupCommonPanel>();
        }
    }
}