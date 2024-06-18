using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    [RequireComponent(typeof(GameImage))]
    public class GameButton : Button{
        private GameText _textUI;

        public GameText Text{
            get{
                if (_textUI == null){
                    _textUI = this.GetComponentInChildren<GameText>();
                }

                return _textUI;
            }
        }

        private GameImage _imgUI;

        public GameImage Image{
            get{
                if (_imgUI == null){
                    _imgUI = this.image.GetComponent<GameImage>();
                }

                return _imgUI;
            }
        }
    }
}