using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class UIListCardView<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : class{
        public virtual bool          ForceBlock   { get{ return true; } }
        public virtual EHorzOrVert   ScrollDir    { get{ return EHorzOrVert.Horizontal; } }
        public virtual int           InShowNum    { get{ return 3; } }
        public virtual float         ItemSizeDelta{ get{ return 420f; } }
        public virtual float         Space        { get{ return -60f; } }
        public virtual RectOffset    Padding      { get{ return new RectOffset(0, 0, 20, 0); } }
        public virtual float         MinScale     { get{ return 0.65f; } }
        public virtual float         PageSpeed    { get{ return 5f; } }
        private        Tweener       _inAnimationTweener;
        private        Vector2       _beginDragPointPos;
        private        Vector2       _beginDragContainerPos;
        private        RectTransform _container;

        public virtual void OnBeginDrag(PointerEventData eventData){
            if (_inAnimationTweener != null && _inAnimationTweener.IsPlaying()){
                _inAnimationTweener.Kill();
            }

            _beginDragPointPos     = eventData.position;
            _beginDragContainerPos = _container.anchoredPosition;
        }

        private float _pageSizeDelta{ get{ return (this.transform as RectTransform).sizeDelta.x; } }

        public virtual void OnDrag(PointerEventData eventData){
            if (_inAnimationTweener != null && _inAnimationTweener.IsPlaying()){
                _inAnimationTweener.Kill();
            }

            Vector2 pointPos = eventData.position;
            //如果是非无限循环的判断是否已经到了边界范围，当超过边界范围的时候设置弹簧属性
            Vector2 targetPos = (_beginDragContainerPos + new Vector2(pointPos.x - _beginDragPointPos.x, 0));
            float   mixPos    = -1 * (Items.Count * (Space                       + ItemSizeDelta) - _pageSizeDelta);
            float   maxPos    = 0f;
            if (targetPos.x < mixPos){
                //已经超出了最大边界范围
                _container.anchoredPosition = new Vector2(mixPos + RubberDelta(targetPos.x - mixPos, _pageSizeDelta), _beginDragContainerPos.y);
            }
            else if (targetPos.x > maxPos){
                //已经超出了最小范围
                _container.anchoredPosition = new Vector2(maxPos + RubberDelta(targetPos.x, _pageSizeDelta), _beginDragContainerPos.y);
            }
            else{
                _container.anchoredPosition = targetPos;
            }

            RefreshItem();
        }

        /// <summary>
        /// 通过Item模板的prefab路径创建GameObject
        /// </summary>
        /// <returns>The template item.</returns>
        /// <param name="tplPath">Tpl path.</param>
        /// GameAssetManager.InstantiatePrefab<IUIListCardViewItemTemplate<T>> (tplPath,parent);
        public virtual IUIListCardViewItemTemplate<T> CreateTemplateItem(string tplPath, Transform parent){ return null; }

        /// <summary>
        /// 分页的索引
        /// </summary>
        /// <value>The index of the page.</value>
        public float PageIndex{
            get{ return -1 * (_container.anchoredPosition.x - Space / 2) / (_pageSizeDelta); }
            set{
                _container.anchoredPosition = new Vector2(-1 * (_pageSizeDelta) * value + Space / 2, _container.anchoredPosition.y);
                RefreshItem();
            }
        }

        /// <summary>
        /// 当前分页的相邻的较大的的整块page的分页值
        /// </summary>
        /// <returns>The nearly block page big.</returns>
        /// <param name="page">Page.</param>
        private float GetNearlyBlockPageBig(float page){
            int i = 0;
            while (true){
                if (page >= 0){
                    if (page == i * (1f / InShowNum)){
                        return page;
                    }

                    if (i * (1f / InShowNum) < page && (i + 1) * (1f / InShowNum) > page){
                        return (i + 1) * (1f / InShowNum);
                    }
                }
                else{
                    if (page == -1 * i * (1f / InShowNum)){
                        return page;
                    }

                    if (-1 * i * (1f / InShowNum) > page && -1 * (i + 1) * (1f / InShowNum) < page){
                        return -1 * (i) * (1f / InShowNum);
                    }
                }

                i++;
            }
        }

        /// <summary>
        /// 当前分页的相邻的较小的的整块page的分页值
        /// </summary>
        /// <returns>The nearly block page small.</returns>
        /// <param name="page">Page.</param>
        private float GetNearlyBlockPageSmall(float page){
            int i = 0;
            while (true){
                if (page >= 0){
                    if (page == i * (1f / InShowNum)){
                        return page;
                    }

                    if (i * (1f / InShowNum) < page && (i + 1) * (1f / InShowNum) > page){
                        return i * (1f / InShowNum);
                    }
                }
                else{
                    if (page == -1 * i * (1f / InShowNum)){
                        return page;
                    }

                    if (-1 * i * (1f / InShowNum) > page && -1 * (i + 1) * (1f / InShowNum) < page){
                        return -1 * (i + 1) * (1f / InShowNum);
                    }
                }

                i++;
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData){
            float endPage = PageIndex;
            if (ForceBlock){
                //开始居中动画
                Vector2 pointPos = eventData.position;
                if ((pointPos.x - _beginDragPointPos.x) > 0){
                    //往下拖动了，分页增加值到整页
                    endPage = GetNearlyBlockPageSmall(endPage);
                }
                else{
                    //往上拖动了，分页减小值到整页
                    endPage = GetNearlyBlockPageBig(endPage);
                }
            }

            OnEndDragWithNoInertiaTween(endPage);
        }

        /// <summary>
        /// 分页总数
        /// </summary>
        /// <value>The page total.</value>
        public float PageTotal{ get{ return Items.Count * 1.0f / InShowNum; } }

        /// <summary>
        /// 拖拽结束时的动画启用
        /// </summary>
        /// <param name="endPage">End page.</param>
        private void OnEndDragWithNoInertiaTween(float endPage){
            if (endPage > (PageTotal - 1)){
                //开始动画回弹
                springBackTween(PageIndex, PageTotal - 1);
            }
            else if (endPage < 0){
                //开始动画回弹
                springBackTween(PageIndex, 0);
            }
            else if (ForceBlock){
                //开始居中动画
                forceBlockTween(PageIndex, endPage);
            }
        }

        //回弹动画，当是不是无限循环的时候才会调用,传递开始的分页index和结束的分页index给动画函数
        public virtual void springBackTween(float start, float end){
            //回弹应该是相隔的距离越远回弹速度越快,就是说回弹时间固定
            _inAnimationTweener = DOTween.To(
                                             () => {
                                                 return start;
                                             },
                                             (x) => {
                                                 PageIndex = x;
                                             },
                                             end,
                                             0.3f
                                            ).
                                          SetEase(Ease.OutQuad);
        }

        /// <summary>
        /// 播放强制整块滑动动画的速度，决定整块滑动播放时间
        /// </summary>
        /// <value>The force block speed.</value>
        public virtual float forceBlockSpeed{ get{ return 10f; } }

        //强制整块滑动动画,传递开始的分页index和结束的分页index给动画函数
        public virtual void forceBlockTween(float start, float end){
            float persent = Math.Abs((end - start) * _pageSizeDelta / 100f / forceBlockSpeed);
            _inAnimationTweener = DOTween.To(
                                             () => {
                                                 return start;
                                             },
                                             (x) => {
                                                 PageIndex = x;
                                             },
                                             end,
                                             persent
                                            ).
                                          SetEase(Ease.InOutQuad).
                                          OnComplete(
                                                     () => {
                                                         if (OnTweenComplete != null){
                                                             OnTweenComplete();
                                                         }
                                                     }
                                                    );
        }

        public Action OnTweenComplete;

        /// <summary>
        /// 弹簧函数和UGUIScrollRect一致
        /// </summary>
        /// <returns>The delta.</returns>
        /// <param name="overStretching">Over stretching.</param>
        /// <param name="viewSize">View size.</param>
        private static float RubberDelta(float overStretching, float viewSize){ return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching); }

        private List<T>                              _data = new List<T>();
        public  List<IUIListCardViewItemTemplate<T>> Items = new List<IUIListCardViewItemTemplate<T>>();

        public void InitData(List<T> datas, string tplPath){
            for (int i = 0; i < this.transform.childCount; i++){
                Destroy(this.transform.GetChild(i).gameObject);
            }

            this._data = datas;
            Items      = new List<IUIListCardViewItemTemplate<T>>();
            LeftButton.onClick.AddListener(
                                           delegate(){
                                               OnLeftButton();
                                           }
                                          );
            rightButton.onClick.AddListener(
                                            delegate(){
                                                OnRightButton();
                                            }
                                           );
            (this.transform as RectTransform).sizeDelta = new Vector2((ItemSizeDelta + Space) * InShowNum, (this.transform as RectTransform).sizeDelta.y);
            GameObject go = new GameObject();
            _container = go.AddComponent<RectTransform>();
            go.transform.SetParent(this.transform);
            go.name                    = "Container";
            go.transform.localScale    = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale    = Vector3.one;
            _container.pivot           = new Vector2(0, 0.5f);
            _container.anchorMin       = new Vector2(0, 0.5f);
            _container.anchorMax       = new Vector2(0, 0.5f);
            if (ScrollDir == EHorzOrVert.Horizontal){
                HorizontalLayoutGroup layout = go.AddComponent<HorizontalLayoutGroup>();
                layout.childControlHeight = false;
                layout.childControlWidth  = false;
                layout.spacing            = Space;
                layout.padding            = Padding;
                _container.sizeDelta      = new Vector2(datas.Count * ItemSizeDelta + (datas.Count - 1) * Space, (this.transform as RectTransform).sizeDelta.y);
            }
            else if (ScrollDir == EHorzOrVert.Vertical){
                VerticalLayoutGroup layout = go.AddComponent<VerticalLayoutGroup>();
                layout.childControlHeight = false;
                layout.childControlWidth  = false;
                layout.spacing            = Space;
                layout.padding            = Padding;
                _container.sizeDelta      = new Vector2((this.transform as RectTransform).sizeDelta.y, datas.Count * ItemSizeDelta + (datas.Count - 1) * Space);
            }

            for (int i = 0; i < datas.Count; i++){
                IUIListCardViewItemTemplate<T> view = CreateTemplateItem(tplPath, go.transform);
                view.UIRect().sizeDelta = new Vector2(this.ItemSizeDelta, view.UIRect().sizeDelta.y);
                view.name               = (typeof(T).Name + "_" + i);
                view.SetData(datas[i]);
                Items.Add(view);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_container);
            RefreshItem();
        }

        private int _centerIndex = 1;

        public void RefreshItem(){
            float centerAnchoredPositionX = -1 * _container.anchoredPosition.x + _pageSizeDelta / 2;
            //重新设置item的大小
            for (int i = 0; i < Items.Count; i++){
                int distance = (int)Mathf.Abs((Items[i].transform as RectTransform).anchoredPosition.x - centerAnchoredPositionX);
                Debug.LogError(distance + "__" + ((_pageSizeDelta                                      - ItemSizeDelta) / 2));
                if (distance > ((_pageSizeDelta - ItemSizeDelta) / 2)){
                    Items[i].transform.localScale = new Vector3(MinScale, MinScale, 1f);
                }
                else{
                    Items[i].transform.localScale = new Vector3(1 - (1 - MinScale) * distance / ((_pageSizeDelta - ItemSizeDelta) / 2), 1 - (1 - MinScale) * distance / (_pageSizeDelta / 2 - ItemSizeDelta / 2), 1f);
                }

                Items[i].InCenter(distance == 0);
                if (distance == 0){
                    _centerIndex = i;
                }
            }

            RefreshPage();
        }

        public Button LeftButton;
        public Button rightButton;

        public void OnLeftButton(){
            float pageEnd = PageIndex - (1.0f / InShowNum);
            if (pageEnd < 0){
                pageEnd = 0;
            }

            pageTween(PageIndex, pageEnd);
        }

        public void OnRightButton(){
            float pageEnd = PageIndex + (1.0f / InShowNum);
            if (pageEnd > (PageTotal - 1)){
                pageEnd = PageTotal - 1;
            }

            pageTween(PageIndex, pageEnd);
        }

        public virtual void pageTween(float start, float end){
            _inAnimationTweener = DOTween.To(
                                             () => {
                                                 return start;
                                             },
                                             (x) => {
                                                 PageIndex = x;
                                             },
                                             end,
                                             Math.Abs((end - start) / PageSpeed)
                                            ).
                                          SetEase(Ease.Linear).
                                          OnComplete(RefreshPage);
        }

        void RefreshPage(){
            float pageNum = PageIndex;
            LeftButton.gameObject.SetActive(_centerIndex  > 1);
            rightButton.gameObject.SetActive(_centerIndex < Items.Count - 2);
        }
    }
}