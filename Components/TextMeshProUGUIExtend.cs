using TMPro;

namespace IG.Module.UI{
    public static class TextMeshProUGUIExtend{
        /// <summary>
        /// csv读取的文字是被转义过的文字，如果需要显示特殊的\n请调用此方法
        /// </summary>
        /// <returns>The escape text.</returns>
        /// <param name="val">Value.</param>
        /// <param name="text">Text.</param>
        public static void SetEscapeText(this TextMeshProUGUI val, string text){
            if (!string.IsNullOrEmpty(text)){
                text = text.Replace(
                                    "\\n",
                                    @"
"
                                   );
            }

            val.text = text;
        }
    }
}