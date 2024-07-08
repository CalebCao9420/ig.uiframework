using UnityEngine;

namespace IG.Module.Language{
    public interface ILanguageManager{
        SystemLanguage CurrenLang{ get; set; }
        string         GetLangText(string key);
    }
}