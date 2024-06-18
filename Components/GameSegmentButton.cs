using System;
using UnityEngine;

namespace IG.Module.UI{
    public class GameSegmentButton : MonoBehaviour{
        [SerializeField]
        protected GameButton[] _GameBtns = null;

        public Sprite OnSprite, OffSprite;

        // 同じボタンを押した場合でも強制的にリフレッシュを行うか
        // whether to forcibly refresh even if pressing the same btn.
        public    bool               IsForceChangeTab = false;
        public    Action<int>        OnChangeTab      = null;
        protected Action<GameButton> OnActionOn       = null;
        protected Action<GameButton> OnActionOff      = null;
        protected int                SelectedIdx      = 0;

        private void Awake(){
            Init();
            OnAwake();
            Refresh();
        }

        protected void Init(){
            for (int i = 0; i < _GameBtns.Length; i++){
                int tag = i;
                this._GameBtns[i].onClick.AddListener(() => this.OnChangeTab(tag));
                SetSpriteBtn(i, false);
            }
        }

        protected virtual void OnAwake(){ }
        public virtual    void Refresh(){ OnChangeTabFunc(SelectedIdx, true); }

        // Btn Add Listener
        public void OnChangeTabFunc(int  Idx){ OnChangeTabFunc(Idx, this.IsForceChangeTab); }
        public void OnChangeTabForce(int Idx){ OnChangeTabFunc(Idx, true); }

        public virtual void OnChangeTabFunc(int Idx, bool isForce = false){
            if (Idx == SelectedIdx && !isForce) return;
            SetSpriteBtn(SelectedIdx, false);
            SetSpriteBtn(Idx,         true);
            this.SelectedIdx = Idx;
            if (this.OnChangeTab != null){
                this.OnChangeTab(Idx);
            }
        }

        protected virtual void SetSpriteBtn(int Idx, bool isOn){ _GameBtns[Idx].image.sprite = isOn ? OnSprite : OffSprite; }
    }
}