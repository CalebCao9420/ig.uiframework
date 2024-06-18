using System;
using IG.AssetBundle;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameImage : Image{
        public void SetFillAmount(float min, float max){ this.fillAmount = min / max; }

        public void AsyncLoadImage(string path, Action<Image> action = null, bool isForceLocal = false){
            AssetSystem.LoadAsync(
                                  (o, oArg) => {
                                      var sprite = o as Sprite;
                                      this.sprite = sprite;
                                      action?.Invoke(this);
                                  },
                                  path
                                 );
        }

        public GameImage LoadImage(string path, bool isForceLocal = false){
            var sprite = AssetSystem.Load<Sprite>(path);
            this.sprite = sprite;
            return this;
        }

        private Color _color;

        public float Alpha{
            get => this.color.a;
            set{
                this._color   = this.color;
                this._color.a = value;
                this.color    = this._color;
            }
        }
    }
}