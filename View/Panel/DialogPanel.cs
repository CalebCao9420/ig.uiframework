namespace IG.Module.UI{
    /// <summary>
    /// Dialog panel.
    /// </summary>
    public class DialogPanel : GamePanel{
        /// <summary>
        /// The is hide last dialog panel.
        /// </summary>
        public bool IsHideLastDialogPanel = true;

        protected override void ShowData(){
            base.ShowData();
            CommonMaskController.Show(this.gameObject, this.transform.parent);
        }

        /// <summary>
        /// Close this instance.
        /// </summary>
        protected void Close(){ PanelController.CloseTopDialogPanel(); }

        public virtual void OnCloseBtn(){ Close(); }
    }
}