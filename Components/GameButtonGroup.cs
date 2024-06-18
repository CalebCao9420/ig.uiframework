using System;
using UnityEngine;

namespace IG.Module.UI{
    public class GameButtonGroup : MonoBehaviour{
        private GameButton[] _gameButtons = null;
        public  Action<int>  OnChangeTab;

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start(){
            _gameButtons = this.GetComponentsInChildren<GameButton>();
            for (int i = 0; i < _gameButtons.Length; i++){
                int tag = i;
                _gameButtons[i].
                    onClick.AddListener(
                                        () => {
                                            this.OnChangeTabFunc(tag);
                                        }
                                       );
            }

            RefreshState(0);
        }

        private void OnChangeTabFunc(int index){
            if (this.OnChangeTab != null){
                this.OnChangeTab(index);
            }

            RefreshState(index);
        }

        private void RefreshState(int index){
            for (int i = 0; i < _gameButtons.Length; i++){
                _gameButtons[i].interactable = i != index;
            }
        }
    }
}