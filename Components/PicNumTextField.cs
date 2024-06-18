using System;
using System.Collections.Generic;
using IG.AssetBundle;
using IG.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class PicNumTextField : MonoBehaviour{
        void                Awake(){ InitParam(); }
        public List<string> NumPathList;

        /// <summary>
        /// Display container
        /// </summary>
        [HideInInspector]
        public GameObject ShowCont;

        /// <summary>
        /// Hidden container
        /// </summary>
        [HideInInspector]
        public GameObject HideCont;

        /// <summary>
        /// layout
        /// </summary>
        protected GridLayoutGroup _GLG;

        /// <summary>
        /// Digital image size
        /// </summary>
        public Vector2 ImageSize;

        /// <summary>
        /// spacing
        /// </summary>
        public Vector2 Gap;

        /// <summary>
        ///Create a good GameObject
        /// </summary>
        protected Dictionary<int, Image> _NumObjAry;

        protected int _Num;

        public int Num{
            get => _Num;
            set{
                if (_Num == value){
                    return;
                }

                _Num = value;
                InitFarmat();
            }
        }

        /// <summary>
        /// Initialize the default image path parameters
        /// </summary>
        protected virtual void InitParam(){
            if (NumPathList == null || NumPathList.Count == 0){
                NumPathList = new List<string>();
            }

            if (ImageSize == Vector2.zero){
                ImageSize = new Vector2(50, 38);
            }

            if (Gap == Vector2.zero){
                Gap = new Vector2(-19, 0);
            }

            ShowCont = new GameObject();
            ShowCont.transform.SetParent(this.transform);
            ShowCont.name                    = "showCont";
            ShowCont.transform.localPosition = Vector3.zero;
            ShowCont.transform.localScale    = Vector3.one;
            _GLG                             = ShowCont.gameObject.AddComponent<GridLayoutGroup>();
            _GLG.cellSize                    = ImageSize;
            _GLG.spacing                     = Gap;
            _GLG.childAlignment              = TextAnchor.MiddleCenter;
            RectTransform showRT = ShowCont.UIRect();
            showRT.sizeDelta = this.RectTransform().sizeDelta;
            HideCont         = new GameObject();
            HideCont.transform.SetParent(this.transform);
            HideCont.name                    = "hideCont";
            HideCont.transform.localPosition = Vector3.zero;
            HideCont.transform.localScale    = Vector3.one;
            HideCont.SetActive(false);
            _NumObjAry = new Dictionary<int, Image>();
        }

        protected void InitFarmat(){
            string numStr = _Num.ToString();
            int    nowNum = 0;
            for (int i = _NumObjAry.Count; i < numStr.Length; i++){
                nowNum = Convert.ToInt32(numStr.Substring(i, 1));
                GameObject newGameObject = new GameObject();
                newGameObject.name = "numItem";
                newGameObject.transform.SetParent(this.HideCont.transform);
                newGameObject.transform.localScale       = Vector3.one;
                newGameObject.transform.localEulerAngles = Vector3.zero;
                Image newImage = newGameObject.AddComponent<Image>();
                _NumObjAry.Add(i, newImage);
            }

            int index = 0;
            foreach (var item in _NumObjAry){
                if (index < numStr.Length){
                    nowNum = Convert.ToInt32(numStr.Substring(index, 1));
                    item.Value.transform.SetParent(this.ShowCont.transform);
                    // GameAssetManager.LoadSprite (numPathList [nowNum], item.Value);
                    var tmpSprite = AssetSystem.Load<Sprite>(NumPathList[nowNum]);
                    item.Value.sprite = tmpSprite;
                }
                else{
                    item.Value.transform.SetParent(this.HideCont.transform);
                }

                index++;
            }
        }
    }
}