using System;

namespace IG.Module.UI{
    public class PopupItemData{
        public Type   PopupItemView;
        public string Title;
        public void   Show(){ PopupController.Show(this); }
    }
}