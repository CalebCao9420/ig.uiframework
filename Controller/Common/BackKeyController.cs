using UnityEngine;

namespace IG.Module.UI{
    /// <summary>
    /// PC上esc键和Android 手机返回键管理类
    /// </summary>
    public class BackKeyController : MonoBehaviour{
        private bool _lockEscape = false;

        // Update is called once per frame
        void Update(){
            if (Input.GetKey(KeyCode.Escape)){
                if (!_lockEscape){
                    _lockEscape = true;
                    PanelController.BackKey();
                }
            }
            else{
                _lockEscape = false;
            }
        }
    }
}