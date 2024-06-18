using UnityEngine;

namespace IG.Module.UI{
    public class ShortLoadingMaskController : MonoBehaviour{
        private bool _isActive = false;

        // Start is called before the first frame update
        void Start(){
            //GameObject go = Resources.Load(GamePrefabPaths.GetPath<ShortLoadingPanel>().LocalPath) as GameObject;
            //go = Instantiate(go);
            //go.transform.parent = this.transform;
            //go.transform.localScale = Vector3.one;
            //go.transform.localPosition = Vector3.zero;
            //go.UIRect().anchoredPosition = Vector2.zero;
            //go.SetActive(false);
        }

        // Update is called once per frame
        void Update(){
            if (IsCheckShow()){
                if (_isActive == true){
                    return;
                }

                for (int i = 0; i < this.transform.childCount; i++){
                    this.transform.GetChild(i).gameObject.SetActive(true);
                }

                _isActive = true;
            }
            else{
                if (!_isActive){
                    return;
                }

                for (int i = 0; i < this.transform.childCount; i++){
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }

                _isActive = false;
            }
        }

        private bool IsCheckShow(){
            //TODO:联机检测
            // if (APIManager.GetInstance().IsNoApiPerform)
            // {
            //     return false;
            // }
            return true;
        }
    }
}