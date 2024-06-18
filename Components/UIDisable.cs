using System.Collections.Generic;
using UnityEngine;

namespace IG.Module.UI{
    public class UIDisable : MonoBehaviour{
        [Header("Settings Init Color #999999FF"), SerializeField]
        private Color _disableColor = new Color(.6f, .6f, .6f, 1f);

        [Header("Disable GameObjects"), SerializeField]
        private List<GameObject> _interactableGo;

        [SerializeField]
        private bool _interactable = true;

        public bool Interactable{ get{ return this._interactable; } set{ this.SetInteractable(value); } }

        [SerializeField, Header("Only Boolen")]
        private bool _isMyChildOnly;

        [SerializeField]
        private bool _isMyOnly;

        void Awake(){
            if (this._isMyChildOnly || this._isMyOnly){
                this._interactableGo = new List<GameObject>();
                this._interactableGo.Add(this.gameObject);
            }

            this.OnAwakeDisableColor();
        }

        private void OnAwakeDisableColor(){
            if (this._interactable) return;
            this.SetInteractable(false, true);
        }

        public void SetInteractable(bool val, bool isForce = false){
            if (this._interactable == val && !isForce) return;
            this._interactable = val;
            if (_isMyOnly){
                for (int i = 0; i < this._interactableGo.Count; i++){
                    this._interactableGo[i].SetInteractable(val, this._disableColor);
                }
            }
            else{
                for (int i = 0; i < this._interactableGo.Count; i++){
                    this._interactableGo[i].ChildInteractable(val, this._disableColor);
                }
            }
        }
    }
}