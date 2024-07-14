using System;
using System.Collections.Generic;
using IG.AssetBundle;
using IG.Runtime.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public static class UIExtension{
    #region Using asset system

        public static void SetSp(this Image img, string path){
            var sprite = AssetSystem.Load<Sprite>(path);
            img.sprite = sprite;
        }

        public static void SetSpAsync(this Image img, string path, Action ok = null){
            AssetSystem.LoadAsync(
                                  (p, pArg) => {
                                      img.sprite = p as Sprite;
                                      if (ok != null){
                                          ok();
                                      }
                                  },
                                  path
                                 );
        }

        public static void SetRawImgAsync(this RawImage img, string path, Action<RawImage> ok = null){
            AssetSystem.LoadAsync(
                                  (p, pArg) => {
                                      if (ok != null){
                                          var tex = p as Texture;
                                          img.texture = tex;
                                          ok(img);
                                      }
                                  },
                                  path
                                 );
        }

        public static void SetRawImg(this RawImage img, string path){
            var loadImg = AssetSystem.Load<Texture>(path);
            img.texture = loadImg;
        }

    #endregion

    #region UI Components
    #region UIImageClip

        public static void SetActive(this UIImageClip clip, bool value){ clip.gameObject.SetActive(value); }

    #endregion

    #region Button

        public static void SetActive(this Button button, bool value){ button.gameObject.SetActive(value); }
        public static void Show(this      Button button){ button.gameObject.SetActive(true); }
        public static void Hide(this      Button button){ button.gameObject.SetActive(false); }

        public static void AddClick(this Button button, Action click){
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(
                                       () => {
                                           if (null != click){
                                               click();
                                           }
                                       }
                                      );
        }

        public static void RemoveClick(this Button button){ button.onClick.RemoveAllListeners(); }

        /// <summary>
        /// 设置按钮文字
        /// TEXT组件必须为Button组件的子组件,不支持多个
        /// </summary>
        /// <param name="button"></param>
        /// <param name="value"></param>
        public static void SetButtonLabel(this Button button, string value){
            Text label = button.GetComponentInChildren<Text>();
            if (null != label){
                label.text = value;
            }
        }

    #endregion

    #region Image

        public static void SetActive(this Image img, bool value){ img.gameObject.SetActive(value); }
        public static void Show(this      Image img){ img.gameObject.SetActive(true); }
        public static void Hide(this      Image img){ img.gameObject.SetActive(false); }

        public static void SetTexture(this Image img, Texture2D texture){
            if (null != texture){
                img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }

        //000000A9
        public static Color32 IMAGE_DISABLED = new Color32(120, 120, 120, 255);

        public static void Disabled(this Image img, bool value){
            if (value){
                img.color = IMAGE_DISABLED;
            }
            else{
                img.color = Color.white;
            }
        }

    #endregion

    #region Text

        public static           void SetActive(this               Text txt, bool value){ txt.gameObject.SetActive(value); }
        public static           void Show(this                    Text txt)                { txt.gameObject.SetActive(true); }
        public static           void Hide(this                    Text txt)                { txt.gameObject.SetActive(false); }
        public static           void SetTextPercent(this          Text label, float value) { label.text = value.ToString("P1"); }
        public static           void SetCurrencyLocalization(this Text label, int   amount){ label.text = amount.ToString("N0"); }
        private static readonly int  s_NUMBER_M = 1000000;

        public static void SetNumber(this Text label, int value, bool setAmountX = false){
            string txt = "";
            if (value < s_NUMBER_M){
                txt        = value.ToString();
                label.text = value.ToString();
            }
            else{
                int m      = value / s_NUMBER_M;
                int remain = value % s_NUMBER_M;
                int suffix = 0;
                if (remain > 0){
                    double v = Math.Round((double)remain / (double)s_NUMBER_M, 1);
                    suffix = (int)(v * 10);
                    if (suffix >= 10){
                        suffix--;
                    }
                }

                txt = $"{m}.{suffix}M";
            }

            if (setAmountX){
                label.text = $"x{txt}";
            }
            else{
                label.text = txt;
            }
        }

    #endregion    

    #endregion

        public static void          SetActive(this Component     behaviour, bool value){ behaviour.gameObject.SetActive(value); }
        public static RectTransform UIRect(this    Transform     transform){ return transform as RectTransform; }
        public static RectTransform UIRect(this    GameObject    go)       { return go.transform as RectTransform; }
        public static RectTransform UIRect(this    MonoBehaviour behaviour){ return behaviour.transform as RectTransform; }

        public static void SetInteractable(this Button button, bool val){
            if (button == null || button.interactable == val){
                return;
            }

            button.interactable = val;
            Color   normalColor   = button.colors.normalColor;
            Color   disabledColor = button.colors.disabledColor;
            Image[] imgs          = button.GetComponentsInChildren<Image>();
            Image   thisImg       = button.image;
            if (val){
                for (int i = 0; i < imgs.Length; i++){
                    if (imgs[i] == thisImg){
                        continue;
                    }

                    imgs[i].color = imgs[i].color.AnMerge(disabledColor);
                }
            }
            else{
                for (int i = 0; i < imgs.Length; i++){
                    if (imgs[i] == thisImg){
                        continue;
                    }

                    imgs[i].color = imgs[i].color.Merge(disabledColor);
                }
            }

            TMP_Text[] tmpTexts = button.GetComponentsInChildren<TMP_Text>();
            if (val){
                for (int i = 0; i < tmpTexts.Length; i++){
                    tmpTexts[i].color = tmpTexts[i].color.AnMerge(disabledColor);
                }
            }
            else{
                for (int i = 0; i < tmpTexts.Length; i++){
                    tmpTexts[i].color = tmpTexts[i].color.Merge(disabledColor);
                }
            }

            GameText[] gameTexts = button.GetComponentsInChildren<GameText>();
            if (val){
                for (int i = 0; i < gameTexts.Length; i++){
                    gameTexts[i].color = gameTexts[i].color.AnMerge(disabledColor);
                }
            }
            else{
                for (int i = 0; i < gameTexts.Length; i++){
                    gameTexts[i].color = gameTexts[i].color.Merge(disabledColor);
                }
            }

            Text[] texts = button.GetComponentsInChildren<Text>();
            if (val){
                for (int i = 0; i < texts.Length; i++){
                    texts[i].color = texts[i].color.AnMerge(disabledColor);
                }
            }
            else{
                for (int i = 0; i < texts.Length; i++){
                    texts[i].color = texts[i].color.Merge(disabledColor);
                }
            }

            Outline[] outlines = button.GetComponentsInChildren<Outline>();
            if (val){
                for (int i = 0; i < outlines.Length; i++){
                    outlines[i].effectColor = outlines[i].effectColor.AnMerge(disabledColor);
                }
            }
            else{
                for (int i = 0; i < outlines.Length; i++){
                    outlines[i].effectColor = outlines[i].effectColor.Merge(disabledColor);
                }
            }
        }

        public static void ChildInteractable(this GameObject self, bool val, Color disabledColor){
            // Button
            List<Button> btns     = new();
            self.GetComponentsInChildren<Button>(btns);
            List<Image> btnsImg = new();
            int         btnCount   = btns?.Count ?? 0;
            for (int i = 0; i < btnCount; ++i){
                var TMBtn = btns[i].GetComponent<GameButtonTextMeshPro>();
                if (TMBtn != null){
                    TMBtn.interactable = val;
                }
                else{
                    btns[i].interactable = val;
                }

                Image img = btns[i].image;
                btnsImg.Add(img);
            }

            // Imgs
            Image[] imgs = self.GetComponentsInChildren<Image>();
            for (int i = 0; i < imgs.Length; i++){
                // Is Button Compnent imgs.
                if (btnsImg.Contains(imgs[i])) continue;
                imgs[i].color = val ? imgs[i].color.AnMerge(disabledColor) : imgs[i].color.Merge(disabledColor);
            }

            // Text, GameText
            Text[] texts = self.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++){
                texts[i].color = val ? texts[i].color.AnMerge(disabledColor) : texts[i].color.Merge(disabledColor);
            }

            // using TMPro
            TMP_Text[] tmpTexts = self.GetComponentsInChildren<TMP_Text>();
            for (int i = 0; i < tmpTexts.Length; i++){
                tmpTexts[i].color = val ? tmpTexts[i].color.AnMerge(disabledColor) : tmpTexts[i].color.Merge(disabledColor);
            }

            Outline[] outlines = self.GetComponentsInChildren<Outline>();
            for (int i = 0; i < outlines.Length; i++){
                outlines[i].effectColor = val ? outlines[i].effectColor.AnMerge(disabledColor) : outlines[i].effectColor.Merge(disabledColor);
            }
        }

        public static void SetInteractable(this GameObject self, bool val, Color disabledColor){
            // Button
            Button btn = self.GetComponent<Button>();
            if (btn != null){
                Image btnImg = btn == null ? null : btn.image;
                var   TMBtn  = btn.GetComponent<GameButtonTextMeshPro>();
                if (TMBtn != null)
                    TMBtn.interactable = val;
                else
                    btn.interactable = val;
            }

            // Imgs
            Image img = self.GetComponent<Image>();
            if (img != null){
                // Is Button Compnent imgs.
                if (btn != null){
                    if (btn.image != img){
                        img.color = val ? img.color.AnMerge(disabledColor) : img.color.Merge(disabledColor);
                    }
                }
                else{
                    img.color = val ? img.color.AnMerge(disabledColor) : img.color.Merge(disabledColor);
                }
            }

            // Text, GameText
            Text txt = self.GetComponent<Text>();
            if (txt != null){
                txt.color = val ? txt.color.AnMerge(disabledColor) : txt.color.Merge(disabledColor);
            }

            // using TMPro
            TMP_Text tmpTxt = self.GetComponent<TMP_Text>();
            if (tmpTxt != null){
                tmpTxt.color = val ? tmpTxt.color.AnMerge(disabledColor) : tmpTxt.color.Merge(disabledColor);
            }

            Outline outline = self.GetComponent<Outline>();
            if (outline != null){
                outline.effectColor = val ? outline.effectColor.AnMerge(disabledColor) : outline.effectColor.Merge(disabledColor);
            }
        }

        public static string ToTString(this int value){
            string str = value.ToString();
            if (value > 0){
                str = value.ToString("###,###");
            }

            return str;
        }

        public static string ToPString(this string value){
            string str = value.Replace("\\n", "\n");
            return str;
        }
    }

    public static class ExtendedUIAssist{
    #region Color Merge Methods.

        public static Color32 Merge(this Color32 color, Color mergeColor){
            Color c = color;
            return Merge(c, mergeColor);
        }

        public static Color Merge(this Color color, Color mergeColor){ return (color * mergeColor); }

        public static Color32 AnMerge(this Color32 color, Color mergeColor){
            Color c = color;
            return AnMerge(c, mergeColor);
        }

        public static Color AnMerge(this Color color, Color mergeColor){
            Color c = color;
            c.r = (color.r / mergeColor.r).RoundCeil(4);
            c.r = c.r >= 1f ? 1f : c.r;
            c.g = (color.g / mergeColor.g).RoundCeil(4);
            c.g = c.g >= 1f ? 1f : c.g;
            c.b = (color.b / mergeColor.b).RoundCeil(4);
            c.b = c.b >= 1f ? 1f : c.b;
            c.a = (color.a / mergeColor.a).RoundCeil(4);
            c.a = c.a >= 1f ? 1f : c.a;
            return c;
        }

    #endregion
    }
}