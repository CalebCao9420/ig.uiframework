using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IG.Module.UI{
    public class GameButtonTextMeshPro : Button{
        private TextMeshProUGUI _labelTxt = null;
        private GameImage       _textImg;
        public  TextMeshProUGUI LabelTxt{ get{ return _labelTxt ??= this.GetComponentInChildren<TextMeshProUGUI>(); } }
        public  GameImage       TextImg { get{ return _textImg ??= this.Find<GameImage>("LabelSp"); } }

        protected override void Awake(){
            base.Awake();
            bool interactableTemp = this.Interactable;
            this.Interactable = !interactableTemp;
            this.Interactable = interactableTemp;
        }

        public string Label{
            set{
                if (this.LabelTxt == null){
                    Debug.Log("Not Find `TMText`.");
                    return;
                }

                if (string.IsNullOrEmpty(value)){
                    LabelTxt.text = value;
                    if (TextImg != null){
                        TextImg.SetActive(false);
                    }
                }
                else{
                    if (TextImg != null){
                        if (value.EndsWith(".png")){
                            TextImg.AsyncLoadImage(
                                                   value,
                                                   c => {
                                                       if (c != null){
                                                           c.SetNativeSize();
                                                           c.SetActive(true);
                                                       }
                                                   }
                                                  );
                            LabelTxt.SetActive(false);
                        }
                        else{
                            TextImg.SetActive(false);
                            LabelTxt.SetActive(true);
                        }
                    }
                    else{
                        LabelTxt.SetActive(true);
                    }

                    LabelTxt.text = value.Replace(
                                                  "\\n",
                                                  @"
"
                                                 );
                }
            }
            get{ return LabelTxt.text; }
        }

        public Material FontSharedMaterial{ get{ return LabelTxt.fontSharedMaterial; } set{ LabelTxt.fontSharedMaterial = value; } }
        public bool     Interactable      { get{ return this.interactable; }           set{ this.SetInteractable(value); } }

        [ContextMenu("Debug - Interactable")]
        private void DebugInteractable(){ this.gameObject.ChildInteractable(this.interactable, this.colors.disabledColor); }

        private void InteractableText(bool val){
            Color disabledColor = this.colors.disabledColor;
            if (Label.EndsWith(".png") && this.TextImg != null){
                if (val){
                    this.TextImg.color = this.TextImg.color.AnMerge(disabledColor);
                }
                else{
                    this.TextImg.color = this.TextImg.color.Merge(disabledColor);
                }

                return;
            }

            if (val){
                // SetCacheColor(); // Delete Color
                this.LabelTxt.color        = this.LabelTxt.color.AnMerge(disabledColor);
                this.LabelTxt.outlineColor = this.LabelTxt.outlineColor.AnMerge(disabledColor);
                //this.labelTxt.faceColor = labelTxt.faceColor.AnMerge(disabledColor);
            }
            else{
                // CacheColor(); // Delete Color
                this.LabelTxt.color        = this.LabelTxt.color.Merge(disabledColor);
                this.LabelTxt.outlineColor = this.LabelTxt.outlineColor.Merge(disabledColor);
                //this.labelTxt.faceColor = labelTxt.faceColor.Merge(disabledColor);
            }
        }

        // Colorコードが正常に戻らない場合のもしもの時のメソッド
        private Color fontColor, outLineColor;

        private void CacheColor(){
            this.fontColor    = this.LabelTxt.color;
            this.outLineColor = this.LabelTxt.outlineColor;
        }

        private void SetCacheColor(){
            this.LabelTxt.color        = this.fontColor;
            this.LabelTxt.outlineColor = this.outLineColor;
        }
    }
}