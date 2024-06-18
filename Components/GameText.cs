using UnityEngine.UI;

namespace IG.Module.UI{
    /// <summary>
    /// 游戏UI中的Text
    /// 
    /// n
    /// </summary>
    public class GameText : Text{
        /// <summary>
        /// 是否自动获取文字
        /// </summary>
        public bool IsStatic = true;

        /// <summary>
        /// 根据语言key format语言.
        /// </summary>
        /// <param name="key">语言key.</param>
        /// <param name="arg">参数.</param>
        public void FormatTextByKey(string key, params object[] arg){
            // FormatText (key.Localized(), arg);
            //TODO:
            FormatText(key, arg);
        }

        /// <summary>
        /// format语言.
        /// </summary>
        /// <param name="format">format.</param>
        /// <param name="arg">参数.</param>
        public void FormatText(string format, params object[] arg){ this.text = string.Format(format, arg); }

        /// <summary>
        /// 通过key赋值
        /// </summary>
        /// <param name="key">Key.</param>
        public void TextByKey(string key){
            // this.text = key.Localized();
            //TODO:
            this.text = key;
        }

        public override string text{
            get{ return base.text; }
            set{
                if (string.IsNullOrEmpty(value)){
                    base.text = value;
                }
                else{
                    base.text = value.Replace(
                                              "\\n",
                                              @"
"
                                             );
                }
            }
        }

        protected override void Start(){
            base.Start();
            if (this.IsStatic){
                // text = base.text.Localized();
                //TODO:
                text = base.text;
            }
        }
    }
}