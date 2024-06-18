using UnityEngine;

namespace IG.Module.UI{
    /// <summary>
    /// Normal panel 
    /// Only one can be displayed at a time. If you open another Normal Panel, the previous Normal Panel will automatically close.
    /// </summary>
    public class PagePanel : GamePanel{
        public Transform DlgPresent;
    }
}