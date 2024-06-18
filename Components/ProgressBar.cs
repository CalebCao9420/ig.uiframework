using UnityEngine;

namespace IG.Module.UI{
    public class ProgressBar : MonoBehaviour{
        public GameText  ContentTxt;
        public GameText  ProgressTxt;
        public GameImage ProgressImg;

        public void SetTitle(string title){
            if (this.ContentTxt != null){
                this.ContentTxt.text = title;
            }
        }

        public void SetProgress(float value){
            if (this.ProgressTxt != null){
                this.ProgressTxt.text = Mathf.FloorToInt(value * 100) + "%";
            }

            this.ProgressImg.fillAmount = value;
        }
    }
}