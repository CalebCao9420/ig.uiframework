using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameSlider : Slider{
        [SerializeField]
        protected RectTransform _Gauge;

        [SerializeField]
        protected RectTransform _MaskGauge;

        private Vector2 _maskSize = new Vector2();
        private bool    _isGameSliderGauge{ get{ return !(this._Gauge == null || this._MaskGauge == null); } }

        protected override void Set(float input, bool sendCallback){
            base.Set(input, sendCallback);
            this.MaskSet();
        }

        private void MaskSet(){
            if (!this._isGameSliderGauge){
                // Debug.LogError("Not Add GameObject `Gauge` and `Mask Gauge`.");
                return;
            }

            if (this.direction == Direction.LeftToRight || this.direction == Direction.RightToLeft){
                this._maskSize.x = this._Gauge.sizeDelta.x * (this.value / this.maxValue);
                this._maskSize.y = this._MaskGauge.sizeDelta.y;
            }
            else{
                this._maskSize.x = this._MaskGauge.sizeDelta.x;
                this._maskSize.y = this._Gauge.sizeDelta.y * (this.value / this.maxValue);
            }

            this._MaskGauge.sizeDelta = this._maskSize;
        }
    }
}