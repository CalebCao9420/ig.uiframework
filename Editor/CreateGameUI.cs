#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace IG.Module.UI.Editor{
    public static class CreateGameUI{
        private const string PACKAGE_NAME = "com.ig.uiframework";
        private const string kUILayerName = "UI";

    #region Text

        [MenuItem("GameObject/UI工具/UI组件/多语言Text", false, 10)]
        public static void CreateText(MenuCommand command){
            GameObject go = new GameObject("Text");
            GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
            var text = go.AddComponent<UIText>();
            // var outline = go.AddComponent<OutLineEx>();
            // text.font                    = AssetDatabase.LoadAssetAtPath<Font>("Assets/Game/Res/LocalRes/Fonts/jf-openhuninn-2.0.ttf");
            text.font                    = UGUIExtensionConfig.Instance.DefaultTextFont;
            text.fontSize                = 20;
            text.alignment               = TextAnchor.MiddleLeft;
            text.text                    = "Text it is using UIText now";
            text.rectTransform.sizeDelta = new Vector2(160, 36);
            text.enableOutline           = true;
            text.outlineCol              = Color.black;
            text.outlineWidth            = 1;
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

    #endregion

    #region Scroll view

        [MenuItem("GameObject/UI工具/UI组件/无限列表")]
        public static void CreateGameScrollView(MenuCommand menuCommand){
            string packagesPath = $"Packages/{PACKAGE_NAME}/Res/EditorRes/UGUI/GameScrollView.prefab";
            string localPath    = "Assets/ig.uiFramework/Res/EditorRes/UGUI/GameScrollView.prefab";
            string loadPath     = String.Empty;
            if (File.Exists(packagesPath)){
                loadPath = packagesPath;
            }
            else{
                loadPath = localPath;
            }

            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(loadPath);
            gameObject      = GameObject.Instantiate(gameObject, Selection.activeTransform);
            gameObject.name = "GameScrollView";
            PlaceUIElementRoot(gameObject, menuCommand);
        }

    #endregion

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand){
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null){
                parent = GetOrCreateCanvasGameObject();
            }

            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
            element.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(element, "Create "                      + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, parent);
            Selection.activeGameObject = element;
        }

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        static public GameObject GetOrCreateCanvasGameObject(){
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy) return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy) return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }

        static public GameObject CreateNewUI(){
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer(kUILayerName);
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            CreateEventSystem(false);
            return root;
        }

        private static void CreateEventSystem(bool select){ CreateEventSystem(select, null); }

        private static void CreateEventSystem(bool select, GameObject parent){
            var esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null){
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null){
                Selection.activeGameObject = esys.gameObject;
            }
        }
    }
}
#endif