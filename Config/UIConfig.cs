using UnityEditor;
using UnityEngine;

namespace IG.Module.UI{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Config/UIConfig")]
    public class UIConfig : ScriptableObject{
        const  string CONFIG_PATH = "UIConfig";
        public string PrefabPathScriptFullPath;

    #region Instance

        private static UIConfig m_Instance;

        public static UIConfig Instance{
            get{
                if (m_Instance == null){
                    m_Instance = LoadInstance();
                }

                return m_Instance;
            }
        }

        private static UIConfig LoadInstance(){
            UIConfig rel = Resources.Load<UIConfig>(CONFIG_PATH);
            return rel;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 保存设置
        /// </summary>
        public void Save(){
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif

    #endregion
    }
}