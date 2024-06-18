using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IG.Module.UI{
    public class AgeDateControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
        public enum EAgeDateType{
            Year,
            Month,
            Day
        }

        // Use this for initialization
        void Start(){
            switch (Type){
                case EAgeDateType.Year:
                    _nowNum = DateTime.Now.Year; //GameEngine.GameTime.Now.Year;
                    _maxNum = _nowNum;
                    _minNum = 0;
                    break;
                case EAgeDateType.Month:
                    _nowNum = DateTime.Now.Month; //GameEngine.GameTime.Now.Month;
                    _maxNum = 12;
                    _minNum = 1;
                    break;
                case EAgeDateType.Day:
                    _nowNum = DateTime.Now.Day; //GameEngine.GameTime.Now.Day;
                    _maxNum = 31;
                    _minNum = 1;
                    break;
                default: break;
            }

            ShowDate();
        }

        // Update is called once per frame
        void                    Update(){ }
        public  TextMeshProUGUI TopTxt;
        public  TextMeshProUGUI CenterTxt;
        public  TextMeshProUGUI BottomTxt;
        public  EAgeDateType    Type;
        private float           _step = 33f;
        private Vector3         _oldPos;

        /// <summary>
        /// start num
        /// </summary>
        private int _nowNum;

        private int _maxNum;
        private int _minNum;

        public void ShowDate(){
            string topNumStr    = _nowNum - 1 < _minNum ? "" : (_nowNum - 1).ToString("D2");
            string bottomNumStr = _nowNum + 1 > _maxNum ? "" : (_nowNum + 1).ToString("D2");
            TopTxt.text = topNumStr;
            switch (Type){
                case EAgeDateType.Year:
                    CenterTxt.text = _nowNum.ToString("D4");
                    break;
                case EAgeDateType.Month:
                    CenterTxt.text = _nowNum.ToString("D2");
                    break;
                case EAgeDateType.Day:
                    CenterTxt.text = _nowNum.ToString("D2");
                    break;
            }

            BottomTxt.text = bottomNumStr;
        }

        /// <summary>
        /// set max value
        /// </summary>
        /// <param name="value">Value.</param>
        public void SetMax(int value){
            _maxNum = value;
            if (_nowNum > _maxNum){
                _nowNum = _maxNum;
            }

            ShowDate();
        }

        /// <summary>
        /// new date num
        /// </summary>
        /// <returns>The date number.</returns>
        public string GetDateNumStr(){
            switch (Type){
                case EAgeDateType.Year:
                    return _nowNum.ToString("D4");
                case EAgeDateType.Month:
                    return _nowNum.ToString("D2");
                case EAgeDateType.Day:
                    return _nowNum.ToString("D2");
                default:
                    return _nowNum.ToString();
            }
        }

        public int  GetDateNum()                           { return _nowNum; }
        public void OnBeginDrag(PointerEventData eventData){ _oldPos = Input.mousePosition; }

        public void OnDrag(PointerEventData eventData){
            Vector3 nowPos = Input.mousePosition;
            if (Mathf.Abs(nowPos.y - _oldPos.y) > _step){
                if (nowPos.y > _oldPos.y){
                    if (_nowNum + 1 > _maxNum){
                        return;
                    }

                    _nowNum++;
                }
                else{
                    if (_nowNum - 1 < _minNum){
                        return;
                    }

                    _nowNum--;
                }

                _oldPos = nowPos;
                ShowDate();
            }
        }

        public void OnEndDrag(PointerEventData eventData){ }
    }
}