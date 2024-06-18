using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameButtonChangeImage : MonoBehaviour{
        public int         Index;
        public GameImage[] Images;

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start(){
            ShowImage();
            this.GetComponent<Button>().onClick.AddListener(OnClick);
            ;
        }

        private void ShowImage(){
            if (Images != null && Images.Length > Index){
                for (int i = 0; i < Images.Length; i++){
                    Images[i].enabled = i == Index;
                }
            }
        }

        private void OnClick(){
            Index++;
            if (Images == null || Index >= Images.Length){
                Index = 0;
            }

            ShowImage();
        }
    }
}