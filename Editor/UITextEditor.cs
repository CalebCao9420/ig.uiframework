#if UNITY_EDITOR
using IG.Module.Language;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace IG.Module.UI.Editor{
    [CustomEditor(typeof(UIText), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the Text Component.
    ///   Extend this class to write a custom editor for an Text-derived component.
    /// </summary>
    public class UITextEditor : GraphicEditor{
        SerializedProperty m_LanguageKey;
        SerializedProperty m_Text;
        SerializedProperty m_Language;
        SerializedProperty m_FontData;
        SerializedProperty m_OutlineCol;
        SerializedProperty m_EnableOutline;
        SerializedProperty m_OutlineWidth;
        SerializedProperty m_EnableGradient;
        SerializedProperty m_GradientColLeft;
        SerializedProperty m_GradientColRight;
        SerializedProperty m_GradientColTop;
        SerializedProperty m_GradientColBottom;
        SerializedProperty m_GradientRatio;
        SerializedProperty m_EnableShadow;
        SerializedProperty m_ShadowScale;
        SerializedProperty m_ShadowOffset;
        SerializedProperty m_ShadowCol;
        SerializedProperty m_SupportMultipleLines;
        GUIContent         m_LanguageKeyContent;
        GUIContent         m_TextContent;
        GUIContent         m_LanguageContent;
        GUIContent         m_OutlineColContent;
        GUIContent         m_EnableOutlineContent;
        GUIContent         m_OutlineWidthContent;
        GUIContent         m_EnableGradientContent;
        GUIContent         m_GradientColLeftContent;
        GUIContent         m_GradientColRightContent;
        GUIContent         m_GradientColTopContent;
        GUIContent         m_GradientColBottomContent;
        GUIContent         m_GradientRatioContent;
        GUIContent         m_EnableShadowContent;
        GUIContent         m_ShadowScaleContent;
        GUIContent         m_ShadowOffsetContent;
        GUIContent         m_ShadowColContent;
        GUIContent         m_SupportMultipleLinesContent;

        protected override void OnEnable(){
            base.OnEnable();
            m_Text                        = serializedObject.FindProperty("m_Text");
            m_Language                    = serializedObject.FindProperty("CurLanguage");
            m_FontData                    = serializedObject.FindProperty("m_FontData");
            m_LanguageKey                 = serializedObject.FindProperty("CurLanguageKey");
            m_EnableOutline               = serializedObject.FindProperty("m_EnableOutline");
            m_OutlineCol                  = serializedObject.FindProperty("m_OutlineCol");
            m_OutlineWidth                = serializedObject.FindProperty("m_OutlineWidth");
            m_EnableGradient              = serializedObject.FindProperty("m_EnableGradient");
            m_GradientColLeft             = serializedObject.FindProperty("m_GradientColLeft");
            m_GradientColRight            = serializedObject.FindProperty("m_GradientColRight");
            m_GradientColTop              = serializedObject.FindProperty("m_GradientColTop");
            m_GradientColBottom           = serializedObject.FindProperty("m_GradientColBottom");
            m_GradientRatio               = serializedObject.FindProperty("m_GradientRatio");
            m_EnableShadow                = serializedObject.FindProperty("m_EnableShadow");
            m_ShadowScale                 = serializedObject.FindProperty("m_ShadowScale");
            m_ShadowOffset                = serializedObject.FindProperty("m_ShadowOffset");
            m_ShadowCol                   = serializedObject.FindProperty("m_ShadowCol");
            m_SupportMultipleLines        = serializedObject.FindProperty("m_SupportMultipleLines");
            m_LanguageKeyContent          = new GUIContent("多语言Key");
            m_TextContent                 = new GUIContent("翻译");
            m_LanguageContent             = new GUIContent("选择预览的多语言");
            m_EnableOutlineContent        = new GUIContent("描边");
            m_OutlineColContent           = new GUIContent("描边颜色");
            m_OutlineWidthContent         = new GUIContent("描边粗细");
            m_EnableGradientContent       = new GUIContent("渐变色");
            m_GradientColLeftContent      = new GUIContent("渐变色左边");
            m_GradientColRightContent     = new GUIContent("渐变色右边");
            m_GradientColTopContent       = new GUIContent("渐变色上边");
            m_GradientColBottomContent    = new GUIContent("渐变色下边");
            m_GradientRatioContent        = new GUIContent("渐变色水平，垂直权重");
            m_SupportMultipleLinesContent = new GUIContent("渐变色支持换行");
            m_EnableShadowContent         = new GUIContent("阴影");
            m_ShadowScaleContent          = new GUIContent("阴影缩放");
            m_ShadowOffsetContent         = new GUIContent("阴影偏移");
            m_ShadowColContent            = new GUIContent("阴影颜色");
        }

        public override void OnInspectorGUI(){
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Language,    m_LanguageContent);
            EditorGUILayout.PropertyField(m_LanguageKey, m_LanguageKeyContent);
            EditorGUILayout.PropertyField(m_Text,        m_TextContent);
            var text = LanguageManagerWrap.EditorGetLangText(m_LanguageKey.stringValue, (SystemLanguage)m_Language.enumValueFlag);
            m_Text.stringValue = text;
            EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();
            EditorGUILayout.PropertyField(m_EnableOutline, m_EnableOutlineContent);
            if (m_EnableOutline.boolValue){
                EditorGUILayout.PropertyField(m_OutlineCol,   m_OutlineColContent);
                EditorGUILayout.PropertyField(m_OutlineWidth, m_OutlineWidthContent);
            }

            EditorGUILayout.PropertyField(m_EnableGradient, m_EnableGradientContent);
            if (m_EnableGradient.boolValue){
                EditorGUILayout.PropertyField(m_GradientColLeft,      m_GradientColLeftContent);
                EditorGUILayout.PropertyField(m_GradientColRight,     m_GradientColRightContent);
                EditorGUILayout.PropertyField(m_GradientColTop,       m_GradientColTopContent);
                EditorGUILayout.PropertyField(m_GradientColBottom,    m_GradientColBottomContent);
                EditorGUILayout.PropertyField(m_GradientRatio,        m_GradientRatioContent);
                EditorGUILayout.PropertyField(m_SupportMultipleLines, m_SupportMultipleLinesContent);
            }

            EditorGUILayout.PropertyField(m_EnableShadow, m_EnableShadowContent);
            if (m_EnableShadow.boolValue){
                EditorGUILayout.PropertyField(m_ShadowScale,  m_ShadowScaleContent);
                EditorGUILayout.PropertyField(m_ShadowOffset, m_ShadowOffsetContent);
                EditorGUILayout.PropertyField(m_ShadowCol,    m_ShadowColContent);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif