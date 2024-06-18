using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IG.Module.UI{
    public class GameRefreshScrollView : GameScrollView{
        private enum EGameRefreshStatus{ Nomal, RefreshStay, RefreshEnd }

        [Header("Refresh Param")]
        // 動作軽量化用
        public bool IsUseRefreshControl = false;

        // 上部で検知する距離
        public float TopRefreshOffSet = 200f;

        // 下部で検知する距離
        public float BottomRefreshOffset = 200f;

        // 強制的に上下にスクロールを固定する
        public bool IsForceScrollPointReset = false;

        // Action Delegate
        public event Action OnTopRefresh = delegate{ }, OnBottomRefresh = delegate{ };
        public  Action      OnAction     = null;
        private Vector2     _scrollPoint;

        // 更新可能かどうかのフラグ
        private bool               _isScrollRefreshFlg = true;
        // private EGameRefreshStatus _status             = EGameRefreshStatus.Nomal;

        public override void OnValueChanged(Vector2 v){
            base.OnValueChanged(v);
            _scrollPoint = this.normalizedPosition;
            InitRefreshFlg();
        }

        public override void OnEndDrag(PointerEventData eventData){
            base.OnEndDrag(eventData);
            OnEndDragTopRefresh();
            OnEndDragBottomRefresh();
        }

        private void OnEndDragTopRefresh(){
            if (!IsUseRefreshControl) return;
            if (_isScrollRefreshFlg){
                if (TopRefreshOffSet <= (GetScrollPoint - 1f) * GetScrollContentSize){
                    _isScrollRefreshFlg = false;
                    if (IsForceScrollPointReset) ForceScrollPosition(true);
                    DOVirtual.DelayedCall(0.3f, () => OnTopRefresh());
                }
            }
        }

        private void OnEndDragBottomRefresh(){
            if (!IsUseRefreshControl) return;
            if (_isScrollRefreshFlg){
                if (BottomRefreshOffset < -GetScrollPoint * GetScrollContentSize){
                    _isScrollRefreshFlg = false;
                    if (IsForceScrollPointReset) ForceScrollPosition(false);
                    DOVirtual.DelayedCall(0.3f, () => OnBottomRefresh());
                }
            }
        }

        private void InitRefreshFlg(){
            if (!IsUseRefreshControl) return;
            if (!_isScrollRefreshFlg){
                float y = Mathf.Floor(_scrollPoint.y * 1000f) / 1000f;
                if (0 <= y && y <= 1.0f){
                    // initialize
                    _isScrollRefreshFlg = true;
                }
            }
        }

        private void ForceScrollPosition(bool istop){
            if (this.horizontal && this.vertical){
                return; // 非対応
            }
            else if (this.horizontal){
                //水平
                horizontalNormalizedPosition = istop ? 1 : 0;
            }
            else if (this.vertical){
                // 垂直
                verticalNormalizedPosition = istop ? 1 : 0;
            }
        }

        private float GetScrollContentSize{
            get{
                if (this.horizontal && this.vertical){
                    // 非対応
                }
                else if (this.horizontal){
                    //水平
                    return this.content.sizeDelta.x;
                }
                else if (this.vertical){
                    // 垂直
                    return this.content.sizeDelta.y;
                }

                return 0;
            }
        }

        private float GetScrollPoint{
            get{
                if (this.horizontal && this.vertical){
                    // 非対応
                }
                else if (this.horizontal){
                    //水平
                    return this._scrollPoint.x;
                }
                else if (this.vertical){
                    // 垂直
                    return this._scrollPoint.y;
                }

                return 0;
            }
        }
    }
}