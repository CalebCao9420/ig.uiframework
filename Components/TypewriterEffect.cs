using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    [RequireComponent(typeof(ScrollRect))]
    public class TypewriterEffect : MonoBehaviour{
        public  float            CharsPerSecond = 0.2f; //打字时间间隔
        public  bool             Pause          = false;
        private ScrollRect       _scrollRect;
        private ContentImmediate _contentImmediate;
        private TMP_Text         _textContent;

        void Awake(){
            _scrollRect                                 = this.GetComponent<ScrollRect>();
            _textContent                                = _scrollRect.content.GetComponent<TMP_Text>();
            _contentImmediate                           = _textContent.rectTransform.GetComponent<ContentImmediate>();
            _textContent.rectTransform.anchorMin        = new Vector2(0,    1);
            _textContent.rectTransform.anchorMax        = new Vector2(1,    1);
            _textContent.rectTransform.pivot            = new Vector2(0.5f, 1);
            _textContent.rectTransform.anchoredPosition = Vector2.zero;
            _textContent.text                           = "";
            _scrollRect.vertical                        = false;
            _scrollRect.horizontal                      = false;
            _scrollRect.movementType                    = ScrollRect.MovementType.Clamped;
            _scrollRect.inertia                         = false;
        }

        private Action _completeCallBack;
        private string _message;
        private bool   _isActive = false;
        private float  _timer;        //计时器
        private int    _currentIndex; //当前打字位置
        private int    _maxLength;    //最大的行列数

        public void PlayAnimation(string message, Action complete = null){
            _completeCallBack     = complete;
            _scrollRect.vertical = false;
            _textContent.text    = "";
            _isActive             = true;
            this._message         = message;
            _currentIndex         = 0;
            _maxLength            = (RichTextHelper.clearLength(message) + 2); //多延迟2秒
            _timer                = 0;
            ShowText();
        }

        void Update(){
            if (!Pause){
                if (_isActive){
                    //设置内容
                    _timer += Time.deltaTime;
                    if (_timer > CharsPerSecond){
                        //累计时间足够开始显示下一个字符
                        if (_currentIndex >= _maxLength){
                            //已经到达最后字符
                            _isActive             = false;
                            _scrollRect.vertical = true;
                            if (_completeCallBack != null){
                                _completeCallBack();
                            }
                        }
                        else{
                            //开始显示下一个字符
                            _currentIndex++;
                            _textContent.text = RichTextHelper.clearSubString(_message, _currentIndex);
                            ShowText();
                        }

                        _timer -= CharsPerSecond;
                    }
                }
            }
        }

        void ShowText(){
            Vector2 preferredSize = _contentImmediate.GetPreferredSize();
            if (preferredSize.y > _scrollRect.viewport.sizeDelta.y){
                (_textContent.transform as RectTransform).anchoredPosition = new Vector2(0, _scrollRect.viewport.sizeDelta.y);
            }
            else{
                (_textContent.transform as RectTransform).anchoredPosition = Vector2.zero;
            }
        }

        public void StopAnimation(){ _isActive = false; }
        public bool isInAnimation  { get{ return _isActive; } }
    }

    public class RichTextHelper{
        static List<string> s_compositeTags{
            get{
                List<string> list = new List<string>();
                list.Add("color");
                list.Add("link");
                list.Add("size");
                return list;
            }
        }

        static List<string> s_singleTags{
            get{
                List<string> list = new List<string>();
                list.Add("i");
                list.Add("u");
                list.Add("b");
                list.Add("sub");
                list.Add("sup");
                return list;
            }
        }

        //获取长度
        public static string clearSubString(string richText, int clearLength){
            StringBuilder clearText       = new StringBuilder("");
            List<string>  myEndTags       = new List<string>();
            int           clearTextLength = 0;
            for (int i = 0; i < richText.Length; i++){
                if (clearTextLength < clearLength){
                    if (richText[i] == '<'){
                        //如果是标签形式开头的获取到结尾的标签位置，判断中间的内容是否是指定的标签
                        int tagLength = GetTagLength(i, richText);
                        if (tagLength > 0){
                            //是完整的标签
                            string tagInfo = richText.Substring(i + 1, tagLength);
                            //Debug.Log ("tagInfo:"+tagInfo+"   tagLength:"+tagLength);
                            bool matchingRichTag = false;
                            if (tagInfo[0] == '/'){
                                //是结束标签
                                for (int j = 0; j < s_compositeTags.Count; j++){
                                    if (("/" + s_compositeTags[j]) == tagInfo){
                                        //是对应的结束tag
                                        matchingRichTag = true;
                                        //Debug.Log ("移除标签结束标志："+compositeTags [j]);
                                        if (myEndTags.FindIndex((s) => s == s_compositeTags[j]) > -1){
                                            myEndTags.RemoveAt(myEndTags.FindIndex((s) => s == s_compositeTags[j]));
                                        }

                                        break;
                                    }
                                }

                                for (int j = 0; j < s_singleTags.Count; j++){
                                    if (("/" + s_singleTags[j]) == tagInfo){
                                        //是对应的结束tag
                                        matchingRichTag = true;
                                        //Debug.Log ("移除标签结束标志："+singleTags [j]);
                                        if (myEndTags.FindIndex((s) => s == s_singleTags[j]) > -1){
                                            myEndTags.RemoveAt(myEndTags.FindIndex((s) => s == s_singleTags[j]));
                                        }

                                        break;
                                    }
                                }
                            }
                            else if (tagInfo.IndexOf('=') > -1){
                                //compositeTags标签类型
                                for (int j = 0; j < s_compositeTags.Count; j++){
                                    if (tagLength > (s_compositeTags[j].Length + 1) && (s_compositeTags[j] + "=") == tagInfo.Substring(0, s_compositeTags[j].Length + 1)){
                                        //是对应的tag
                                        matchingRichTag = true;
                                        //Debug.Log ("添加标签结束标志："+compositeTags [j]);
                                        myEndTags.Add(s_compositeTags[j]);
                                        break;
                                    }
                                }
                            }
                            else{
                                //singleTags标签类型
                                for (int j = 0; j < s_singleTags.Count; j++){
                                    if (s_singleTags[j] == tagInfo){
                                        //是对应的tag
                                        matchingRichTag = true;
                                        //Debug.Log ("添加标签结束标志："+tagInfo);
                                        myEndTags.Add(tagInfo);
                                        break;
                                    }
                                }
                            }

                            if (matchingRichTag){
                                i += (tagLength + 1);
                                clearText.Append("<" + tagInfo + ">");
                            }
                            else{
                                //Debug.Log("不能匹配的标签,当普通字符处理:"+tagInfo);
                                clearText.Append(richText[i]);
                                clearTextLength++;
                            }
                        }
                        else{
                            //非标签,当普通字符处理
                            //Debug.Log("非标签,当普通字符处理:+"+richText [i]);
                            clearText.Append(richText[i]);
                            clearTextLength++;
                        }
                    }
                    else{
                        //普通字符
                        clearText.Append(richText[i]);
                        clearTextLength++;
                    }
                }
            }

            for (int i = myEndTags.Count; i > 0; i--){
                clearText.Append("</" + myEndTags[i - 1] + ">");
            }

            return clearText.ToString();
        }

        public static int clearLength(string richText){
            int clearTextLength = 0;
            for (int i = 0; i < richText.Length - 1; i++){
                if (richText[i] == '<'){
                    //如果是标签形式开头的获取到结尾的标签位置，判断中间的内容是否是指定的标签
                    int tagLength = GetTagLength(i, richText);
                    if (tagLength > 0){
                        //是完整的标签
                        string tagInfo = richText.Substring(i + 1, tagLength);
                        //Debug.Log ("tagInfo:"+tagInfo+"   tagLength:"+tagLength);
                        bool matchingRichTag = false;
                        if (tagInfo[0] == '/'){
                            //是结束标签
                            for (int j = 0; j < s_compositeTags.Count; j++){
                                if (("/" + s_compositeTags[j]) == tagInfo){
                                    //是对应的结束tag
                                    matchingRichTag = true;
                                    break;
                                }
                            }

                            for (int j = 0; j < s_singleTags.Count; j++){
                                if (("/" + s_singleTags[j]) == tagInfo){
                                    //是对应的结束tag
                                    matchingRichTag = true;
                                    break;
                                }
                            }
                        }
                        else if (tagInfo.IndexOf('=') > -1){
                            //compositeTags标签类型
                            for (int j = 0; j < s_compositeTags.Count; j++){
                                if (tagLength > (s_compositeTags[j].Length + 1) && (s_compositeTags[j] + "=") == tagInfo.Substring(0, s_compositeTags[j].Length + 1)){
                                    //是对应的tag
                                    matchingRichTag = true;
                                    break;
                                }
                            }
                        }
                        else{
                            //singleTags标签类型
                            for (int j = 0; j < s_singleTags.Count; j++){
                                if (s_singleTags[j] == tagInfo){
                                    //是对应的tag
                                    matchingRichTag = true;
                                    break;
                                }
                            }
                        }

                        if (matchingRichTag){
                            i += (tagLength + 1);
                        }
                        else{
                            clearTextLength++;
                        }
                    }
                    else{
                        clearTextLength++;
                    }
                }
                else{
                    //普通字符
                    clearTextLength++;
                }
            }

            return clearTextLength;
        }

        static int GetTagLength(int tagStartIndex, string richText){
            int tagLength = 0;
            for (int i = tagStartIndex + 1; i < richText.Length; i++){
                if (richText[i] == '>'){
                    //已经是结束标签了
                    return tagLength;
                }

                tagLength++;
            }

            return 0;
        }
    }
}