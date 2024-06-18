using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    [RequireComponent(typeof(RectMask2D))]
    public class ImageFillAmout : MonoBehaviour{
        private Image      _fill;
        private RectMask2D _rectMask2D;
        private float      _value;

        public float value{
            get => _value;
            set{
                if (value > 1){
                    value = 1;
                }
                else if (value < 0){
                    value = 0;
                }

                _value = value;
                UpdateUI();
            }
        }

        void Awake(){
            _rectMask2D = this.GetComponent<RectMask2D>();
            _fill       = this.GetComponentInChildren<Image>();
            UpdateUI();
        }

        void UpdateUI(){
            if (_rectMask2D != null && _fill != null){
                _rectMask2D.rectTransform.sizeDelta = new Vector2(_fill.rectTransform.sizeDelta.x * value, _fill.rectTransform.sizeDelta.y);
            }
        }
    }
}