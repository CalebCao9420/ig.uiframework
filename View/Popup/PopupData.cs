using System;

namespace IG.Module.UI{
    public abstract class PopupData{
        public Type          PopPanel;
        public bool          IsAnimation = true;
        public PopupItemData PopupItemData;
        public void          Show(){ PopupController.Show(this); }
    }
}