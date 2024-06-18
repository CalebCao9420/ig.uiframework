using System.Collections.Generic;
using System.Text.RegularExpressions;
using IG.Runtime.Common.Timer;
using IG.Runtime.Wrap.Center;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class TextImg : MonoBehaviour{
        public  GameText            GameText;
        public  TextMeshProUGUI     TextMeshProUGUI;
        public  List<RectTransform> Items;
        private string              _text = "";
        private Regex               _r    = new Regex("<img.*?/>");

        private void Init(){
            if (Items != null){
                for (int i = 0; i < Items.Count; i++){
                    Destroy(Items[i].gameObject);
                }

                Items.Clear();
            }

            if (string.IsNullOrEmpty(_text)){
                return;
            }

            string[]        texts = _r.Split(_text);
            MatchCollection imgs  = _r.Matches(_text);
            for (int i = 0; i < texts.Length; i++){
                if (GameText != null){
                    CreateGameText(texts[i]);
                }
                else{
                    CreateTextMeshProUGUI(texts[i]);
                }

                if (imgs.Count > i){
                    CreateGameRawImage(imgs[i].Value);
                }
            }

            UnityTimer.SetTimeout(
                                  0.1f,
                                  () => {
                                      this.GetComponent<ContentSizeFitter>().enabled = true;
                                  }
                                 );
        }

        private void CreateGameText(string text){
            if (string.IsNullOrEmpty(text)){
                return;
            }

            GameText temp = GameObject.Instantiate(GameText, this.transform);
            temp.rectTransform.localScale   = Vector3.one;
            temp.transform.localEulerAngles = Vector3.zero;
            temp.text                       = text;
            Items.Add(temp.rectTransform);
            temp.gameObject.SetActive(true);
        }

        private void CreateTextMeshProUGUI(string text){
            if (string.IsNullOrEmpty(text)){
                return;
            }

            TextMeshProUGUI temp = Instantiate(TextMeshProUGUI, this.transform);
            temp.rectTransform.localScale   = Vector3.one;
            temp.transform.localEulerAngles = Vector3.zero;
            temp.text                       = text;
            Items.Add(temp.rectTransform);
            TMPLinkClick tMPLinkClick = temp.GetComponent<TMPLinkClick>();
            if (tMPLinkClick != null){
                tMPLinkClick.OnClickLinkID = TextMeshProUGUI.GetComponent<TMPLinkClick>().OnClickLinkID;
            }

            temp.gameObject.SetActive(true);
        }

        private void CreateGameRawImage(string text){
            GameObject   obj = new GameObject();
            GameRawImage img = obj.AddComponent<GameRawImage>();
            img.transform.parent           = this.transform;
            img.transform.localScale       = Vector3.one;
            img.transform.localEulerAngles = Vector3.zero;
            string   path  = "";
            int      w     = 0;
            int      h     = 0;
            string[] datas = text.Split(new char[]{ ' ' });
            for (int i = 0; i < datas.Length; i++){
                if (datas[i].StartsWith("<img=")){
                    path = datas[i].Replace("/>", "").Replace("<img=", "");
                }
                else if (datas[i].StartsWith("width=")){
                    w = int.Parse(datas[i].Replace("/>", "").Replace("width=", ""));
                }
                else if (datas[i].StartsWith("height=")){
                    h = int.Parse(datas[i].Replace("/>", "").Replace("height=", ""));
                }
            }

            img.rectTransform.sizeDelta = new Vector2(w, h);
            img.SetUrl(path);
            Items.Add(img.rectTransform);
        }

        public void SetText(string value){
            this._text = value;
            this.Init();
        }
    }
}