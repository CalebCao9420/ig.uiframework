using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class DisplayByLineEffect : MonoBehaviour{
        public  float            CharsPerSecond = 0.2f; //打字时间间隔
        public  bool             Pause          = false;
        public  TMP_Text         TextContent;
        private ScrollRect       _scrollRect;
        private ContentImmediate _contentImmediate;

        void Awake(){
            _scrollRect                                = this.GetComponent<ScrollRect>();
            TextContent                                = _scrollRect.content.GetComponent<TMP_Text>();
            _contentImmediate                          = TextContent.rectTransform.GetComponent<ContentImmediate>();
            TextContent.rectTransform.anchorMin        = new Vector2(0,    1);
            TextContent.rectTransform.anchorMax        = new Vector2(1,    1);
            TextContent.rectTransform.pivot            = new Vector2(0.5f, 1);
            TextContent.rectTransform.anchoredPosition = Vector2.zero;
            TextContent.text                           = "";
            _scrollRect.vertical                       = false;
            _scrollRect.horizontal                     = false;
            _scrollRect.movementType                   = ScrollRect.MovementType.Clamped;
            _scrollRect.inertia                        = false;
        }

        private Action                  _completeCallBack;
        private List<string>            _messages;
        private bool                    _isActive = false;
        private float                   _timer;        //计时器
        private int                     _currentIndex; //当前显示的行
        private Dictionary<int, Action> _inShowLines;  //需要监听的正在显示的行
        private int                     _count = 0;

        public void PlayAnimation(List<string> messages, Dictionary<int, Action> inShowLines, Action complete = null){
            this._inShowLines    = inShowLines;
            _completeCallBack    = complete;
            _scrollRect.vertical = false;
            TextContent.text     = "";
            _isActive            = true;
            this._messages       = messages;
            _currentIndex        = 0;
            _timer               = 0;
            ShowText();
        }

        void Update(){
            if (!Pause){
                if (_isActive){
                    //设置内容
                    _timer += Time.deltaTime;
                    if (_timer > CharsPerSecond){
                        //累计时间足够开始显示下一个字符
                        if (_currentIndex >= _messages.Count){
                            //已经到达最后字符
                            _isActive            = false;
                            _scrollRect.vertical = true;
                            if (_completeCallBack != null){
                                _completeCallBack();
                            }

                            _count = 0;
                        }
                        else{
                            //开始显示下一个字符
                            _currentIndex++;
                            TextContent.text = string.Join("\n", _messages.GetRange(0, _currentIndex).ToArray());
                            ShowText();
                            if (_inShowLines.ContainsKey(_currentIndex)){
                                _inShowLines[_currentIndex]();
                            }
                        }

                        _timer -= CharsPerSecond;
                    }
                }
            }
        }

        void ShowText(){
            _count++; //カウントを追加する
            Vector2 preferredSize = _contentImmediate.GetPreferredSize();
            if (preferredSize.y > _scrollRect.viewport.sizeDelta.y){
                (TextContent.transform as RectTransform).anchoredPosition = new Vector2(0, _scrollRect.viewport.sizeDelta.y);
            }
            else{
                (TextContent.transform as RectTransform).anchoredPosition = Vector2.zero;
            }
        }

        public void StopAnimation(){ _isActive = false; }
        public bool isInAnimation  { get{ return _isActive; } }
    }
}