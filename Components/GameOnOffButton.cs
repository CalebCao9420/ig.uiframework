using System;
using UnityEngine;

namespace IG.Module.UI{
    [RequireComponent(typeof(GameButton))]
    public class GameOnOffButton : MonoBehaviour{
        public  Sprite              OnSprite;
        public  Sprite              OffSprite;
        public  Action<bool> OnClick    = null;
        private bool                _isOn      = true;
        private GameImage           _gameImage = null;
        private GameButton          _gameBtn   = null;

        private void OnStart(){
            _gameImage = this.GetComponent<GameImage>();
            Refresh();
            this._gameBtn = this.GetComponent<GameButton>();
            this._gameBtn.onClick.AddListener(this.OnClickFunc);
        }

        public bool IsOn{
            get{ return _isOn; }
            set{
                _isOn = value;
                Refresh();
            }
        }

        private void Refresh(){
            if (_isOn){
                _gameImage.sprite = OnSprite;
            }
            else{
                _gameImage.sprite = OffSprite;
            }
        }

        private void OnClickFunc(){
            if (this.OnClick != null){
                this.OnClick(this._isOn);
            }

            this.IsOn = !this.IsOn;
        }
    }
}