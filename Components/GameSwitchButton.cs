using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    [RequireComponent(typeof(GameImage))]
    public class GameSwitchButton : Button{
        public  int             PpecialNum;
        public  TextMeshProUGUI LabelOn;
        public  TextMeshProUGUI LabelOff;
        public  Image           OnImg;
        public  Image           OffImg;
        public  Action          ChangeCmpEvent;
        private bool            _isOn;

        public bool IsOn{
            set{
                _isOn = value;
                ChangeState();
                if (ChangeCmpEvent != null){
                    ChangeCmpEvent.Invoke();
                }
            }
            get{ return _isOn; }
        }

        /// <summary>
        /// don't invoke ChangeCmpEvent
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected{
            set{
                _isOn = value;
                ChangeState();
            }
            get{ return _isOn; }
        }

        public string Label{
            set{
                if (LabelOn != null){
                    LabelOn.text = value;
                }

                if (LabelOff != null){
                    LabelOff.text = value;
                }
            }
            get{
                if (LabelOn != null){
                    return LabelOn.text;
                }
                else{
                    return "";
                }
            }
        }

        protected override void Awake(){
            base.Awake();
            this.onClick.AddListener(OnChangeStateEvent);
            if (Application.isPlaying){
                ChangeState();
            }
        }

        protected override void Start(){ base.Start(); }

        private void OnChangeStateEvent(){
            _isOn = !_isOn;
            ChangeState();
            if (ChangeCmpEvent != null){
                ChangeCmpEvent.Invoke();
            }
        }

        private void ChangeState(){
            if (LabelOn != null){
                LabelOn.enabled = _isOn;
            }

            if (LabelOff != null){
                LabelOff.enabled = !_isOn;
            }

            OnImg.enabled  = _isOn;
            OffImg.enabled = !_isOn;
        }
    }
}