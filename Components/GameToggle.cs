using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameToggle : Toggle{
        protected override void Start(){
            base.Start();
            this.interactable = !isOn;
            this.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn){ this.interactable = !isOn; }
    }
}