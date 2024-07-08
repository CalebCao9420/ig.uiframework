using UnityEngine;

namespace IG.Module.Language{
    public class LanguageManagerWrap : WrapBase<ILanguageManager>{
        public static SystemLanguage CurrenLang{
            get{
                JudgeExist();
                return S_Ins.CurrenLang;
            }
            set{
                JudgeExist();
                S_Ins.CurrenLang = value;
            }
        }

        public static string GetLangText(string key){
            JudgeExist();
            return S_Ins.GetLangText(key);
        }

        public static string EditorGetLangText(string key, SystemLanguage language = SystemLanguage.English){
            if (string.IsNullOrWhiteSpace(key)){
                return string.Empty;
            }

            //TODO:加了配置后修改
            // var map = EditorLoadFile(language);
            // if (map != null)
            // {
            //     if (map.TryGetValue(key, out string value))
            //     {
            //         return value;
            //     }
            // }
            return key;
        }
    }
}