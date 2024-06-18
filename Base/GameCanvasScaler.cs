using UnityEngine.Device;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameCanvasScaler : CanvasScaler{
        private float _oldMatchWidthOrHeight = 0;

        protected override void Start(){
            base.Start();
            this.matchWidthOrHeight = 1;
#if UNITY_IOS
            		if (UnityEngine.iOS.Device.generation.ToString ().Contains ("iPad")) {
            			this.matchWidthOrHeight = 0;
            		}
#endif
            if (this.referenceResolution.x > Screen.width / (Screen.height / this.referenceResolution.y)){
                this.matchWidthOrHeight = 0;
            }

            _oldMatchWidthOrHeight = this.matchWidthOrHeight;
        }

        public void SetHideLeftAndRightMask(bool value){
            if (value){
                if (_oldMatchWidthOrHeight == 1){
                    matchWidthOrHeight = 0;
                }
            }
            else{
                if (_oldMatchWidthOrHeight == 1){
                    matchWidthOrHeight = 1;
                }
            }
        }
    }
}