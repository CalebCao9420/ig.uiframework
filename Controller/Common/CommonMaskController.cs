using IG.AssetBundle;
using IG.Module.UI.Res;
using UnityEngine;

namespace IG.Module.UI{
    public class CommonMaskController : GameView{
        private static CommonMaskController s_instance;
        private static GameObject           s_caller;

        public static void Show(GameObject caller, Transform parent){
            if (s_instance == null){
                PrefabInfo prefabInfo = GamePrefabPaths.GetPath(typeof(CommonMaskController));
                var        prefab     = AssetSystem.Load<GameObject>(prefabInfo.path);
                var        obj        = GameObject.Instantiate(prefab);
                obj.name   = prefab.name;
                s_instance = obj.GetComponent<CommonMaskController>();
            }

            if (s_instance == null){
                return;
            }

            if (s_instance.gameObject.activeSelf){
                return;
            }

            s_instance.gameObject.SetActive(true);
            s_instance.transform.SetParent(parent);
            s_instance.transform.SetAsFirstSibling();
            s_caller = caller;
        }

        public static void Hide(GameObject caller){
            if (s_instance == null){
                return;
            }

            if (s_caller != caller){
                return;
            }

            s_instance.gameObject.SetActive(false);
            s_caller = null;
        }
    }
}