using System.IO;
using UnityEditor;
using UnityEngine;

namespace IG.Module.UI{
    public class UGUIExtensionConfig : ScriptableObject{
        public Font DefaultTextFont;

#if UNITY_EDITOR
        
        /// <summary>
        /// 设置数据基本路径
        /// </summary>
        private const string SettingDataBasePath = "Assets/Editor/Res";

        /// <summary>
        /// 储存路径
        /// </summary>
        public const string PATH = SettingDataBasePath + "/UGUIExtensionConfig.asset";
        private static UGUIExtensionConfig s_instance;

        public static UGUIExtensionConfig Instance{
            get{
                if (s_instance == null){
                    s_instance = AssetDatabase.LoadAssetAtPath<UGUIExtensionConfig>(PATH);
                    if (s_instance == null){
                        s_instance = new UGUIExtensionConfig();
                        FileInfo fileInfo = new FileInfo(PATH);
                        if (!fileInfo.Directory.Exists){
                            fileInfo.Directory.Create();
                        }

                        AssetDatabase.CreateAsset(s_instance, PATH);
                    }
                }

                return s_instance;
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public void Save(){
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}