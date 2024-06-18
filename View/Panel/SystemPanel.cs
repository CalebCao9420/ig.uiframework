namespace IG.Module.UI{
    /// <summary>
    /// SystemPanel 
    /// Only one can be displayed at a time. The current SystemPanel must be closed to open the next SystemPanel.
    /// </summary>
    public class SystemPanel : GamePanel{
        protected override void ShowData(){
            base.ShowData();
            CommonMaskController.Show(this.gameObject, this.transform.parent);
        }

        public void Close(){ PanelController.CloseTopSytemPopupPanel(); }
    }
}