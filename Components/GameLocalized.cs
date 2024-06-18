using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IG.Module.UI{
    public class GameLocalized : MonoBehaviour{
        //public bool isResetStyles = false;
        public  string          LocalizedKey;
        private TextMeshProUGUI _textMesh;
        private Text            _uguiText;
        private bool            _isUguiText;

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start(){ }

        public void Init(){
            InitText();
            InitMesh();
        }

        private void InitText(){
            if (_uguiText == null){
                _uguiText = this.GetComponent<Text>();
            }

            if (string.IsNullOrEmpty(LocalizedKey) || _uguiText == null){
                return;
            }

            _isUguiText = true;
            // text = localizedKey.Localized();
            //TODO:
            text = LocalizedKey;
        }

        private void InitMesh(){
            if (_textMesh == null){
                _textMesh = this.GetComponent<TextMeshProUGUI>();
            }

            if (string.IsNullOrEmpty(LocalizedKey) || _textMesh == null){
                return;
            }

            _isUguiText = false;
            //TODO:
            // text = localizedKey.Localized();
            text = LocalizedKey;
            //if (isResetStyles)
            //textMesh.fontStyle = FontStyles.Normal;
        }

        public string text{
            get{
                if (_isUguiText){
                    return _uguiText.text;
                }

                return _textMesh.text;
            }
            set{
                if (string.IsNullOrEmpty(value)){
                    if (_isUguiText){
                        _uguiText.text = value;
                    }
                    else{
                        _textMesh.text = value;
                    }
                }
                else{
                    if (_isUguiText){
                        _uguiText.text = value.Replace(
                                                       "\\n",
                                                       @"
"
                                                      );
                    }
                    else{
                        _textMesh.text = value.Replace(
                                                       "\\n",
                                                       @"
"
                                                      );
                    }
                }
            }
        }
    }
}