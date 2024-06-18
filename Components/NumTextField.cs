using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class NumTextField : MonoBehaviour{
        // Use this for initialization
        void Start(){ }

        // Update is called once per frame
        void Update(){
            if (Glg.enabled == true && _step < 1){
                _step++;
            }
            else{
                Glg.enabled = false;
                foreach (var item in _imgList){
                    item.SetNativeSize();
                }
            }
        }

        public  GameImage       Img;
        public  Transform       Cont;
        public  GridLayoutGroup Glg;
        public  int             Type        = 1;
        private string          _typePath01 = "Images/CommonUI/unit_text_Select_{0}.png";
        private string          _typePath02 = "Images/CommonUI/unit_text_Select_{0}.png";
        private List<GameImage> _imgList;
        private int             _step;
        private int             _textNum;

        public int TextNum{
            get => _textNum;
            set{
                if (_textNum == value){
                    return;
                }

                _textNum = value;
                ShowNum();
            }
        }

        private void ShowNum(){
            while (Cont.childCount > 0){
                DestroyImmediate(Cont.GetChild(0).gameObject);
            }

            _step       = 0;
            _imgList    = new List<GameImage>();
            Glg.enabled = true;
            string numStr = _textNum.ToString();
            for (int i = 0; i < numStr.Length; i++){
                string numPath = "";
                if (Type == 1){
                    numPath = string.Format(_typePath01, numStr.Substring(i, 1));
                }
                else{
                    numPath = string.Format(_typePath02, numStr.Substring(i, 1));
                }

                GameImage newNumImg = Instantiate(Img);
                newNumImg.transform.SetParent(Cont);
                newNumImg.transform.localPosition = Vector3.zero;
                newNumImg.transform.localScale    = Vector3.one;
                newNumImg.SetSpAsyn(numPath);
                _imgList.Add(newNumImg);
            }
        }
    }
}