using TMPro;
using UnityEngine;

namespace IG.Module.UI{
    public class TMDropdownTogle : MonoBehaviour{
        [SerializeField, Header("TM_Label")]
        private TextMeshProUGUI _tMLabel;

        public TextMeshProUGUI TMLabel{ get{ return this._tMLabel ?? (this._tMLabel = this.gameObject.GetComponentInChildren<TextMeshProUGUI>()); } }

        [Header("On")]
        public Color ON_FontColor;

        public Material ON_TMMaterial;

        [Header("Off")]
        public Color OFF_FontColor;

        public Material OFF_TMMaterial;

        public void OnToggle(bool IsOn){
            if (IsOn){
                this.TMLabel.color              = this.ON_FontColor;
                this.TMLabel.fontSharedMaterial = this.ON_TMMaterial;
            }
            else{
                this.TMLabel.color              = this.OFF_FontColor;
                this.TMLabel.fontSharedMaterial = this.OFF_TMMaterial;
            }
        }
    }
}