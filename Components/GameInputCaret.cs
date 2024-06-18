using UnityEngine;

namespace IG.Module.UI{
    [RequireComponent(typeof(GameImage))]
    public class GameInputCaret : MonoBehaviour{
        private GameInputField _gameInputField;
        private float          _time = 0;
        private GameImage      _image;
        private Vector3        _cursorXY;
        public  Vector2        OffSetXY = new Vector2(-10, 0);

        private void Awake(){
            this.UIRect().anchoredPosition = new Vector2(-100000, 0);
            Vector2 sizeDelta = this.UIRect().sizeDelta;
            this.UIRect().sizeDelta = new Vector2(sizeDelta.x, 0);
            this.gameObject.SetActive(false);
            this._image = this.GetComponent<GameImage>();
        }

        public void SetGameInputField(GameInputField gameInputField){
            _gameInputField = gameInputField;
            _gameInputField.onValueChanged.AddListener(OnValueChanged);
            _gameInputField.onEndEdit.AddListener(OnCaretEndEdit);
        }

        private void OnValueChanged(string value){ PlayAnimation(); }

        private void OnCaretEndEdit(string value){
            this.UIRect().anchoredPosition = new Vector2(-100000, 0);
            this.gameObject.SetActive(false);
            _time = 0;
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void LateUpdate(){
            if (_gameInputField != null && _gameInputField.CursorVerts != null && _gameInputField.CursorVerts.Length > 0){
                this.UIRect().anchoredPosition = new Vector2(_gameInputField.CursorVerts[0].position.x, 0) + OffSetXY;
                if (_cursorXY != _gameInputField.CursorVerts[0].position){
                    PlayAnimation();
                }

                _cursorXY =  _gameInputField.CursorVerts[0].position;
                _time     += Time.deltaTime;
                if (_time >= _gameInputField.caretBlinkRate && this._image.enabled){
                    _time               = 0;
                    this._image.enabled = false;
                    return;
                }

                if (_time >= _gameInputField.caretBlinkRate / 2 && !this._image.enabled){
                    _time               = 0;
                    this._image.enabled = true;
                }
            }
        }

        public void PlayAnimation(){
            this._time          = 0;
            this._image.enabled = true;
        }
    }
}