using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameTabButton : Toggle{
        public Image           OffImg;
        public Image           OnImg;
        public TextMeshProUGUI LabelOn;
        public TextMeshProUGUI LabelOff;

        [HideInInspector]
        public int Index;

        public Action ChangeCmpEvent;

        public string Label{
            set{
                LabelOn.text  = value;
                LabelOff.text = value;
            }
            get{ return LabelOn.text; }
        }

        protected override void Start(){
            base.Start();
            LabelOn.enabled  = isOn;
            LabelOff.enabled = !isOn;
            LabelOn.gameObject.SetActive(isOn);
            LabelOff.gameObject.SetActive(!isOn);
            if (OnImg != null){
                OnImg.gameObject.SetActive(isOn);
            }

            if (OffImg != null){
                OffImg.gameObject.SetActive(!isOn);
            }

            this.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDestroy(){
            this.onValueChanged.RemoveListener(OnValueChanged);
            base.OnDestroy();
        }

        private void OnValueChanged(bool value){
            if (Application.isPlaying){
                LabelOn.enabled  = isOn;
                LabelOff.enabled = !isOn;
                LabelOn.gameObject.SetActive(value);
                LabelOff.gameObject.SetActive(!value);
                if (OnImg != null){
                    OnImg.gameObject.SetActive(isOn);
                }

                if (OffImg != null){
                    OffImg.gameObject.SetActive(!isOn);
                }

                if (ChangeCmpEvent != null){
                    ChangeCmpEvent.Invoke();
                }
            }
        }
    }
}