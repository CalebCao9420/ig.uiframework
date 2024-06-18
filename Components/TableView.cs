using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IG.Module.UI{
    [SelectionBase]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class TableView : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup{
        /// <summary>
        /// 支持的滑动方向，横向或者纵向中的一种
        /// </summary>
        /// <value>The scroll direction.</value>
        [Tooltip("支持的滑动方向，横向或者纵向中的一种")]
        [SerializeField]
        public EHorzOrVert ScrollDirection = EHorzOrVert.Vertical;

        /// <summary>
        /// 是否支持无限循环
        /// </summary>
        /// <value><c>true</c> if loop; otherwise, <c>false</c>.</value>
        [Tooltip("当不是无限循环的时候可以设置回弹动画")]
        [SerializeField]
        public bool Loop = true;

        /// <summary>
        /// 是否支持强制整块滑动
        /// </summary>
        /// <value><c>true</c> if force block; otherwise, <c>false</c>.</value>
        [Tooltip("当强行整块滑动的时候需要可以设置动画的速度和DowTwenn的Ease")]
        [SerializeField]
        public bool ForceBlock = false;

        /// <summary>
        /// 是否支持回弹动画 只要在loop是false的时候生效
        /// </summary>
        [Tooltip("是否支持回弹动画 只要在loop是false的时候生效")]
        [SerializeField]
        public bool Elastic = false;

        /// <summary>
        /// 播放强制整块滑动动画的速度，决定整块滑动播放时间
        /// </summary>
        /// <value>The force block speed.</value>
        [Tooltip("播放强制整块滑动动画的速度，决定整块滑动播放时间")]
        [SerializeField]
        public float ForceBlockSpeed = 100f;

        [Tooltip("强制整块滑动动画的DotweenEase")]
        [SerializeField]
        public Ease ForceBlockEase = Ease.InOutQuad;

        /// <summary>
        /// 是否自动获取TPL的SizeDelta
        /// </summary>
        [Tooltip("如果是自动获取则获取CellTemplate的SizeDelta")]
        [SerializeField]
        private bool _autoCellSize = true;

        /// <summary>
        /// 单元格的尺寸
        /// </summary>
        [Tooltip("一个cell的尺寸")]
        [SerializeField]
        public Vector2 CellSize = new Vector2(100, 100);

        /// <summary>
        /// x行间隔
        /// y列间隔
        /// </summary>
        [Tooltip("x行间隔,y列间隔")]
        [SerializeField]
        public Vector2 Space = new Vector2(10, 10);

        /// <summary>
        /// 每页显示的行数
        /// </summary>
        /// <value>The row number.</value>
        [Tooltip("每页显示的行数")]
        [SerializeField]
        public int RowNum = 1;

        /// <summary>
        /// 每页显示的列数
        /// </summary>
        /// <value>The column number.</value>
        [Tooltip("每页显示的列数")]
        [SerializeField]
        public int ColumnNum = 1;

        private RectTransform _content;
        public  RectTransform Content     => _content;
        private bool          _horizontal => (ScrollDirection != EHorzOrVert.Vertical);
        private bool          _vertical   => (ScrollDirection == EHorzOrVert.Vertical);

        private ScrollRect.MovementType _movementType{
            get{
                if (Loop){
                    return ScrollRect.MovementType.Unrestricted;
                }
                else{
                    if (Elastic){
                        return ScrollRect.MovementType.Elastic;
                    }
                    else{
                        return ScrollRect.MovementType.Clamped;
                    }
                }
            }
        }

        /// <summary>
        /// 弹性
        /// </summary>
        [Tooltip("同ScrollRect的弹力")]
        [SerializeField]
        private float _elasticity = 0.1f; // Only used for MovementType.Elastic

        public float Elasticity{ get{ return _elasticity; } set{ _elasticity = value; } }

        /// <summary>
        /// 惯性
        /// </summary>
        [Tooltip("同ScrollRect当支持惯性的时候需要给一个阻力")]
        [SerializeField]
        private bool _inertia = false;

        public bool Inertia{ get{ return _inertia; } set{ _inertia = value; } }

        /// <summary>
        /// 惯性的阻力
        /// </summary>
        [Tooltip("惯性的阻力")]
        [SerializeField]
        private float _decelerationRate = 0.135f; // Only used when inertia is enabled

        public float DecelerationRate{ get => _decelerationRate; set => _decelerationRate = value; }

        /// <summary>
        /// scroll的敏感度
        /// </summary>
        [Tooltip("scroll的敏感度")]
        [SerializeField]
        private float _scrollSensitivity = 1.0f;

        public float ScrollSensitivity{ get => _scrollSensitivity; set => _scrollSensitivity = value; }

        /// <summary>
        /// 模板Item
        /// </summary>
        [Tooltip("可复用的模板Item")]
        [SerializeField]
        public GameObject CellTemplate;

        /// <summary>
        /// 滚动事件
        /// </summary>
        [Tooltip("滚动事件")]
        [SerializeField]
        private ScrollRect.ScrollRectEvent _onValueChanged = new();

        public  ScrollRect.ScrollRectEvent OnValueChanged{ get => _onValueChanged; set => _onValueChanged = value; }
        private RectTransform              _viewport     => this._rectTransform;

        // The offset from handle position to mouse down position
        private   Vector2       _pointerStartLocalCursor = Vector2.zero;
        protected Vector2       _ContentStartPosition    = Vector2.zero;
        private   RectTransform _viewRect => this._rectTransform;
        protected Bounds        _ContentBounds;
        private   Bounds        _viewBounds;
        private   Vector2       _velocity;
        public    Vector2       Velocity{ get => _velocity; set => _velocity = value; }
        private   bool          _dragging;
        private   Vector2       _prevPosition = Vector2.zero;
        private   Bounds        _prevContentBounds;
        private   Bounds        _prevViewBounds;

        [NonSerialized]
        private bool _hasRebuiltLayout = false;

        [NonSerialized] private RectTransform _rect;

        private RectTransform _rectTransform{
            get{
                if (_rect == null) _rect = GetComponent<RectTransform>();
                return _rect;
            }
        }

        private DrivenRectTransformTracker _tracker;

        public virtual void Rebuild(CanvasUpdate executing){
            if (executing == CanvasUpdate.Prelayout){
                UpdateCachedData();
            }

            if (executing == CanvasUpdate.PostLayout){
                UpdateBounds();
                UpdatePrevData();
                _hasRebuiltLayout = true;
            }
        }

        public virtual void LayoutComplete()       { }
        public virtual void GraphicUpdateComplete(){ }

        void UpdateCachedData(){
            Transform transform = this.transform;
        }

        /// <summary>
        /// 用于记录滑动状态变为了禁止
        /// </summary>
        private bool _inSliding = false;

        /// <summary>
        /// 用于记录是否在回弹动画状态.
        /// </summary>
        private bool _inElasticing;

        /// <summary>
        /// 用于记录拖动完成，这样可以保证只播放一次拖动完成动画
        /// </summary>
        private bool _needPlayMoveEnd = false;

        /// <summary>
        /// content容器
        /// </summary>
        private RectTransform _container;

        /// <summary>
        /// 是否初始化完毕
        /// </summary>
        private bool _isInitOk = false;

        /// <summary>
        /// 数据源.
        /// </summary>
        public object[] Data{ get; private set; }

        /// <summary>
        /// 复用的Cell
        /// </summary>
        public List<ITableViewCell> Items{ get; private set; }

        /// <summary>
        /// 第一个cell的开始坐标
        /// </summary>
        private float _startX = 0;

        /// <summary>
        /// 第一个cell的开始坐标
        /// </summary>
        private float _startY = 0;

        /// <summary>
        /// The force block tweener.
        /// </summary>
        private Tweener _forceBlockTweener;

        /// <summary>
        /// 记录item对应的数据，当数据变化的时候调用OnWillShow
        /// </summary>
        private Dictionary<ITableViewCell, object> _cellsTab;

        //		void Awake(){
        //			if (cellTemplate != null) {
        //				Init ();
        //			}
        //		}

        //如果没有赋值可以通过
        /// <summary>
        /// 如果没有给cellTemplate赋值可以通过init方法初始化
        /// </summary>
        /// <param name="cellTemplate">Cell template.</param>
        public void Init(GameObject cellTemplate){
            this.CellTemplate = cellTemplate;
            Init();
        }

        /// <summary>
        /// 初始化UITableView
        /// </summary>
        public void Init(){
            if (!_isInitOk){
                Data      = null;
                Items     = new List<ITableViewCell>();
                _cellsTab = new Dictionary<ITableViewCell, object>();
                if (CellTemplate.GetComponent<ITableViewCell>() == null){
                    Debug.LogError("cellTemplate上找不到模板文件UITableViewCell");
                    return;
                }

                if (_autoCellSize){
                    CellSize = (CellTemplate.transform as RectTransform).sizeDelta;
                }

                GameObject go = new GameObject();
                go.transform.SetParent(this.transform);
                _container               = go.AddComponent<RectTransform>();
                _container.name          = "Container";
                _container.localScale    = Vector3.one;
                _container.localRotation = Quaternion.identity;
                //container.gameObject.AddComponent<Image> ().color = new Color (1f, 0f, 0f, 0.5f);
                CellTemplate.transform.SetParent(_container);
                ITableViewCell tpl = CellTemplate.GetComponent<ITableViewCell>();
                Items.Add(tpl);
                _cellsTab.Add(tpl, null);
                for (int i = 1; i < RowNum * ColumnNum; i++){
                    ITableViewCell item = Instantiate(CellTemplate, _container as Transform).GetComponent<ITableViewCell>();
                    item.gameObject.name = (CellTemplate.gameObject.name + "_" + i);
                    Items.Add(item);
                    _cellsTab.Add(item, null);
                }

                _isInitOk = true;
                _content  = _container;
            }
        }

        /// <summary>
        /// 设置数据成功之后的回调
        /// </summary>
        /// <value>The on set datas complete.</value>
        public Action onSetDataComplete{ get; set; }

        //slideValue 0-1之间
        public void SetData(object[] dataList){
            if (_isInitOk){
                this.Data = dataList;
#if UNITY_EDITOR
                for (int i = 0; i < Data.Length; i++){
                    if (Data[i] == null){
                        Debug.LogError("索引：" + i + "的数据为null " + this.GetType().FullName + "不能设置空数据否则会出现ReuseCell不能正常刷新");
                    }
                }
#endif
                //设置容器的宽高
                if (ScrollDirection == EHorzOrVert.Vertical){
                    _container.sizeDelta = new Vector2((CellSize.x + Space.x) * ColumnNum - Space.x, (CellSize.y + Space.y) * _dataGroupNum - Space.y);
                    //设置container的布局方式
                    _container.pivot            = new Vector2(0.5f, 1);
                    _container.anchorMin        = new Vector2(0.5f, 1);
                    _container.anchorMax        = new Vector2(0.5f, 1);
                    _container.anchoredPosition = Vector2.zero;
                    //获取startItem的anchoredPosition
                    _startX = (_container.sizeDelta.x / 2f - CellSize.x / 2) * -1;
                    _startY = (_container.sizeDelta.y / 2f - CellSize.y / 2);
                }
                else{
                    _container.sizeDelta        = new Vector2((CellSize.x + Space.x) * _dataGroupNum - Space.x, (CellSize.y + Space.y) * RowNum - Space.y);
                    _container.pivot            = new Vector2(0,                                                0.5f);
                    _container.anchorMin        = new Vector2(0,                                                0.5f);
                    _container.anchorMax        = new Vector2(0,                                                0.5f);
                    _container.anchoredPosition = Vector2.zero;
                    //获取startItem的anchoredPosition
                    _startX = (_container.sizeDelta.x / 2f - CellSize.x / 2) * -1;
                    _startY = (_container.sizeDelta.y / 2f - CellSize.y / 2);
                }

                RefreshCells();
                if (onSetDataComplete != null){
                    onSetDataComplete();
                }
            }
            else{
                Debug.LogError(this.GetType().FullName + "需要先初始化");
            }
        }

        /// <summary>
        /// 滚动的slider值，只有在loop是false的时候有效
        /// </summary>
        /// <value>The slider value.</value>
        public float SliderValue{
            get{
                if (!Loop){
                    if (ScrollDirection == EHorzOrVert.Vertical){
                        return _container.anchoredPosition.y / (_container.sizeDelta.y - this._rectTransform.sizeDelta.y);
                    }
                    else{
                        return _container.anchoredPosition.x / (_container.sizeDelta.x - this._rectTransform.sizeDelta.x);
                    }
                }
                else{
                    Debug.LogError("sliderValue只有在loop是false的时候有效");
                    return 0;
                }
            }
            set{
                if (!Loop){
                    if (ScrollDirection == EHorzOrVert.Vertical){
                        float containerOffset = (_container.sizeDelta.y - this._rectTransform.sizeDelta.y) * value;
                        _container.anchoredPosition = new Vector2(0, containerOffset);
                    }
                    else{
                        float containerOffset = (_container.sizeDelta.x - this._rectTransform.sizeDelta.x) * value;
                        _container.anchoredPosition = new Vector2(containerOffset, 0);
                    }
                }
                else{
                    Debug.LogError("sliderValue只有在loop是false的时候有效");
                }
            }
        }

        /// <summary>
        /// 获取第一个cell显示的数据的索引,可能不在datas的索引范围内,可以通过GetInDatasRangeRelativeIndex获取到datas范围内的相对索引
        /// 通过startCellDataIndex可以做banner的Index
        /// </summary>
        /// <value>The start index of the data.</value>
        public int StartCellDataIndex{
            get{
                if (ScrollDirection == EHorzOrVert.Vertical){
                    int stepNum = Mathf.FloorToInt(this._container.anchoredPosition.y / StepSize);
                    return stepNum * ColumnNum;
                }
                else{
                    int stepNum = Mathf.FloorToInt((-1) * this._container.anchoredPosition.x / StepSize);
                    return stepNum * RowNum;
                }
            }
        }

        /// <summary>
        /// 把超过datas数组长度的index转换到数组之内的Index
        /// </summary>
        /// <returns>The relative index.</returns>
        /// <param name="index">Index.</param>
        public int GetInDatasRangeRelativeIndex(int index){
            if (index >= 0 && index < Data.Length){
                return index;
            }
            else{
                if (index > 0){
                    index -= Data.Length;
                }
                else{
                    index += Data.Length;
                }

                return GetInDatasRangeRelativeIndex(index);
            }
        }

        /// <summary>
        /// UITableView是否正在动画中,如果非无限循环并且支持弹性这里的Animation会持续很久
        /// </summary>
        /// <value><c>true</c> if in animation; otherwise, <c>false</c>.</value>
        public bool InAnimation{ get; private set; }

        /// <summary>
        /// 是否正在播放回弹动画 这个动画的时间很久
        /// </summary>
        /// <value><c>true</c> if in elasticing; otherwise, <c>false</c>.</value>
        public bool InElasticing => _inElasticing;

        /// <summary>
        /// 如果是垂直滑动返回总数据需要有多少行才能装的下
        /// 如果是水平滑动返回总数据需要有多少列才能装的下
        /// </summary>
        /// <value>The data group number.</value>
        private int _dataGroupNum{
            get{
                if (ScrollDirection == EHorzOrVert.Vertical){
                    if (Data.Length % ColumnNum == 0){
                        return Data.Length / ColumnNum;
                    }
                    else{
                        return Data.Length / ColumnNum + 1;
                    }
                }
                else{
                    if (Data.Length % RowNum == 0){
                        return Data.Length / RowNum;
                    }
                    else{
                        return Data.Length / RowNum + 1;
                    }
                }
            }
        }

        /// <summary>
        /// 滑动一个格子的步长
        /// </summary>
        /// <value>The step.</value>
        public float StepSize{
            get{
                if (ScrollDirection == EHorzOrVert.Vertical){
                    return (CellSize.y + Space.y);
                }
                else{
                    return (CellSize.x + Space.x);
                }
            }
        }

        /// <summary>
        /// 整个重复使用的items格子的大小
        /// </summary>
        /// <value>The size of the page.</value>
        private float _reuseSize{
            get{
                if (ScrollDirection == EHorzOrVert.Vertical){
                    return StepSize * RowNum;
                }
                else{
                    return StepSize * ColumnNum;
                }
            }
        }

        /// <summary>
        /// 整个container的尺寸
        /// </summary>
        /// <value>The size of the container.</value>
        private float _containerSize{
            get{
                if (ScrollDirection == EHorzOrVert.Vertical){
                    return _container.sizeDelta.y + Space.y;
                }
                else{
                    return _container.sizeDelta.x + Space.x;
                }
            }
        }

        /// <summary>
        /// 显示对应的Cell
        /// </summary>
        /// <param name="cell">Cell.</param>
        /// <param name="dataIndex">Data index.</param>
        private void OnWillShowCell(ITableViewCell cell, int dataIndex){
            object cellData = Data[GetInDatasRangeRelativeIndex(dataIndex)];
            if (dataIndex < Data.Length && dataIndex >= 0){
                if (_cellsTab[cell] != cellData){
                    _cellsTab[cell] = cellData;
                    cell.gameObject.SetActive(true);
                    cell.OnInShow(cellData);
                }
            }
            else{
                if (Loop){
                    if (_cellsTab[cell] != cellData){
                        _cellsTab[cell] = cellData;
                        cell.gameObject.SetActive(true);
                        cell.OnInShow(cellData);
                    }
                }
                else{
                    _cellsTab[cell] = null;
                    cell.OnWillHidden();
                    cell.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 刷新每个格子的位置和数据
        /// </summary>
        private void RefreshCells(){
            if (ScrollDirection == EHorzOrVert.Vertical){
                float containerOffset = _container.anchoredPosition.y;
                if (containerOffset > 0){
                    //往上滑动
                    int offsetContent = Mathf.FloorToInt(containerOffset / _containerSize);
                    containerOffset -= offsetContent * _containerSize;
                    int offsetPage    = Mathf.FloorToInt(containerOffset                             / _reuseSize);
                    int offsetItemNum = Mathf.FloorToInt((containerOffset - offsetPage * _reuseSize) / StepSize);
                    for (int i = 0; i < Items.Count; i++){
                        float x         = _startX + i % ColumnNum * (CellSize.x + Space.x);
                        float y         = _startY - i / ColumnNum * (CellSize.y + Space.y);
                        int   dataIndex = ColumnNum               * _dataGroupNum * offsetContent + offsetPage * (RowNum * ColumnNum) + i;
                        if ((i / ColumnNum) < offsetItemNum){
                            y         -= _reuseSize;
                            dataIndex += (RowNum * ColumnNum);
                        }

                        //因为是使用的UGUI的滚动算法所以这里的Y需要一直的叠加
                        y                                                                 -= offsetPage * _reuseSize + offsetContent * _containerSize;
                        (Items[i].gameObject.transform as RectTransform).anchoredPosition =  new Vector2(x, y);
                        OnWillShowCell(Items[i], dataIndex);
                    }
                }
                else{
                    //往下滑动
                    int offsetContent = Mathf.CeilToInt(containerOffset / _containerSize);
                    containerOffset -= offsetContent * _containerSize;
                    int offsetPage    = Mathf.CeilToInt(containerOffset                              / _reuseSize);
                    int offsetItemNum = Mathf.FloorToInt((containerOffset - offsetPage * _reuseSize) / StepSize);
                    for (int i = 0; i < Items.Count; i++){
                        float x         = _startX + i % ColumnNum * (CellSize.x + Space.x);
                        float y         = _startY - i / ColumnNum * (CellSize.y + Space.y);
                        int   dataIndex = ColumnNum               * _dataGroupNum * offsetContent + offsetPage * (RowNum * ColumnNum) + i;
                        if ((i / ColumnNum) >= (RowNum + offsetItemNum)){
                            y         += _reuseSize;
                            dataIndex -= (RowNum * ColumnNum);
                        }

                        y                                                                 -= offsetPage * _reuseSize + offsetContent * _containerSize;
                        (Items[i].gameObject.transform as RectTransform).anchoredPosition =  new Vector2(x, y);
                        OnWillShowCell(Items[i], dataIndex);
                    }
                }
            }
            else{
                float containerOffset = _container.anchoredPosition.x;
                if (containerOffset > 0){
                    //往右滑动
                    int offsetContent = Mathf.FloorToInt(containerOffset / _containerSize); //先检查偏移了多少个Content
                    containerOffset -= offsetContent * _containerSize;
                    int offsetPage    = Mathf.FloorToInt(containerOffset                                         / _reuseSize);
                    int offsetItemNum = Mathf.CeilToInt((containerOffset + CellSize.x - offsetPage * _reuseSize) / StepSize);
                    for (int i = 0; i < Items.Count; i++){
                        float x         = _startX + i / RowNum * (CellSize.x + Space.x);
                        float y         = _startY - i % RowNum * (CellSize.y + Space.y);
                        int   dataIndex = -RowNum     * _dataGroupNum * offsetContent - offsetPage * (RowNum * ColumnNum) + i;
                        if ((i / RowNum) > (ColumnNum - offsetItemNum)){
                            x         -= _reuseSize;
                            dataIndex -= (RowNum * ColumnNum);
                        }

                        //因为是使用的UGUI的滚动算法所以这里的X需要一直的叠加
                        x                                                                 -= offsetPage * _reuseSize + offsetContent * _containerSize;
                        (Items[i].gameObject.transform as RectTransform).anchoredPosition =  new Vector2(x, y);
                        OnWillShowCell(Items[i], dataIndex);
                    }
                }
                else{
                    //往左边滑动
                    int offsetContent = Mathf.CeilToInt(containerOffset / _containerSize);
                    containerOffset -= offsetContent * _containerSize;
                    int offsetPage    = Mathf.CeilToInt(containerOffset                             / _reuseSize);
                    int offsetItemNum = Mathf.CeilToInt((containerOffset - offsetPage * _reuseSize) / StepSize);
                    for (int i = 0; i < Items.Count; i++){
                        float x         = _startX + i / RowNum * (CellSize.x + Space.x);
                        float y         = _startY - i % RowNum * (CellSize.y + Space.y);
                        int   dataIndex = -RowNum     * _dataGroupNum * offsetContent - offsetPage * (RowNum * ColumnNum) + i;
                        if ((i / RowNum) < Mathf.Abs(offsetItemNum)){
                            x         += _reuseSize;
                            dataIndex += (RowNum * ColumnNum);
                        }

                        //因为是使用的UGUI的滚动算法所以这里的X需要一直的叠加
                        x                                                                 -= offsetPage * _reuseSize + offsetContent * _containerSize;
                        (Items[i].gameObject.transform as RectTransform).anchoredPosition =  new Vector2(x, y);
                        OnWillShowCell(Items[i], dataIndex);
                    }
                }
            }
        }

        protected override void OnEnable(){
            base.OnEnable();
            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        protected override void OnDisable(){
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
            _hasRebuiltLayout = false;
            _tracker.Clear();
            _velocity = Vector2.zero;
            LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
            base.OnDisable();
        }

        public override bool IsActive(){ return base.IsActive() && _content != null; }

        private void EnsureLayoutHasRebuilt(){
            if (!_hasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout()) Canvas.ForceUpdateCanvases();
        }

        public virtual void StopMovement(){ _velocity = Vector2.zero; }

        public virtual void OnScroll(PointerEventData data){
            if (!IsActive()) return;
            EnsureLayoutHasRebuilt();
            UpdateBounds();
            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;
            if (_vertical && !_horizontal){
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) delta.y = delta.x;
                delta.x = 0;
            }

            if (_horizontal && !_vertical){
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x)) delta.x = delta.y;
                delta.y = 0;
            }

            Vector2 position = _content.anchoredPosition;
            position += delta * _scrollSensitivity;
            if (_movementType == ScrollRect.MovementType.Clamped) position += CalculateOffset(position - _content.anchoredPosition);
            SetContentAnchoredPosition(position);
            UpdateBounds();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData){
            if (eventData.button != PointerEventData.InputButton.Left) return;
            _velocity = Vector2.zero;
            if (_forceBlockTweener != null && _forceBlockTweener.IsPlaying()){
                _forceBlockTweener.Kill();
            }

            InAnimation = true;
        }

        public virtual void OnBeginDrag(PointerEventData eventData){
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!IsActive()) return;
            UpdateBounds();
            _pointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewRect, eventData.position, eventData.pressEventCamera, out _pointerStartLocalCursor);
            _ContentStartPosition = _content.anchoredPosition;
            _dragging             = true;
            if (_forceBlockTweener != null && _forceBlockTweener.IsPlaying()){
                _forceBlockTweener.Kill();
            }

            InAnimation = true;
        }

        public virtual void OnEndDrag(PointerEventData eventData){
            if (eventData.button != PointerEventData.InputButton.Left) return;
            _dragging        = false;
            _inSliding       = false;
            _inElasticing    = false;
            _needPlayMoveEnd = true;
        }

        /// <summary>
        /// 可以通过pointerDelta获取到Drag的方向
        /// </summary>
        Vector2 pointerDelta;

        public virtual void OnDrag(PointerEventData eventData){
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!IsActive()) return;
            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewRect, eventData.position, eventData.pressEventCamera, out localCursor)) return;
            UpdateBounds();
            pointerDelta = localCursor - _pointerStartLocalCursor;
            Vector2 position = _ContentStartPosition + pointerDelta;

            // Offset to get content into place in the view.
            Vector2 offset = CalculateOffset(position - _content.anchoredPosition);
            position += offset;
            if (_movementType == ScrollRect.MovementType.Elastic){
                if (offset.x != 0) position.x = position.x - RubberDelta(offset.x, _viewBounds.size.x);
                if (offset.y != 0) position.y = position.y - RubberDelta(offset.y, _viewBounds.size.y);
            }

            SetContentAnchoredPosition(position);
        }

        protected virtual void SetContentAnchoredPosition(Vector2 position){
            if (!_horizontal) position.x = _content.anchoredPosition.x;
            if (!_vertical) position.y   = _content.anchoredPosition.y;
            if (position != _content.anchoredPosition){
                _content.anchoredPosition = position;
                UpdateBounds();
            }
        }

        protected virtual void LateUpdate(){
            if (!_content){
                InAnimation = false;
                return;
            }

            EnsureLayoutHasRebuilt();
            UpdateBounds();
            float   deltaTime = Time.unscaledDeltaTime;
            Vector2 offset    = CalculateOffset(Vector2.zero);
            if (!_dragging && (offset != Vector2.zero || _velocity != Vector2.zero)){
                Vector2 position = _content.anchoredPosition;
                for (int axis = 0; axis < 2; axis++){
                    // Apply spring physics if movement is elastic and content has an offset from the view.
                    if (_movementType == ScrollRect.MovementType.Elastic && offset[axis] != 0){
                        float speed = _velocity[axis];
                        position[axis] = Mathf.SmoothDamp(_content.anchoredPosition[axis], _content.anchoredPosition[axis] + offset[axis], ref speed, _elasticity, Mathf.Infinity, deltaTime);
                        if (Mathf.Abs(speed) < 1) speed = 0;
                        _velocity[axis] = speed;
                    }
                    // Else move content according to velocity with deceleration applied.
                    else if (_inertia){
                        _velocity[axis] *= Mathf.Pow(_decelerationRate, deltaTime);
                        if (Mathf.Abs(_velocity[axis]) < 1) _velocity[axis] = 0;
                        position[axis] += _velocity[axis] * deltaTime;
                    }
                    // If we have neither elaticity or friction, there shouldn't be any velocity.
                    else{
                        _velocity[axis] = 0;
                    }
                }

                if (_movementType == ScrollRect.MovementType.Clamped){
                    offset   =  CalculateOffset(position - _content.anchoredPosition);
                    position += offset;
                }

                SetContentAnchoredPosition(position);
            }

            if (_dragging && _inertia){
                Vector3 newVelocity = (_content.anchoredPosition - _prevPosition) / deltaTime;
                _velocity = Vector3.Lerp(_velocity, newVelocity, deltaTime * 10);
            }

            if (_movementType == ScrollRect.MovementType.Elastic){
                //支持回弹动画，如果支持强制整块滑动，当到达回弹的最上面和最下面的时候会忽略掉强制整块滑动动画，这里仅仅播放回弹动画
                if (!_dragging && offset != Vector2.zero){
                    //这里是回弹动画
                    _inElasticing = true;
                }
                else if (!_inElasticing){
                    if (ForceBlock){
                        if (offset == Vector2.zero && _needPlayMoveEnd && _velocity.sqrMagnitude <= Mathf.Pow(ForceBlockSpeed, 2)){
                            //不在回弹动画范围内，在中间部分的惯性滑动
                            _inSliding = true;
                            _velocity  = Vector2.zero;
                        }
                    }
                    else{
                        if (offset == Vector2.zero && _velocity != Vector2.zero){
                            //不在回弹动画范围内，在中间部分的惯性滑动
                            _inSliding = true;
                        }
                    }
                }
            }
            else{
                //不支持回弹动画,如果不支持惯性m_Velocity永远为0不会触发InSliding
                if (ForceBlock){
                    if (_needPlayMoveEnd && !_dragging && _velocity.sqrMagnitude <= Mathf.Pow(ForceBlockSpeed, 2)){
                        _inSliding = true;
                        _velocity  = Vector2.zero;
                    }
                }
                else{
                    if (_velocity != Vector2.zero && !_dragging){
                        _inSliding = true;
                    }
                }
            }

            //这里是如果不支持惯性拖拽并且没有播放回弹动画完成就会走到这里
            if (!_dragging && !_inElasticing && !_inertia && _needPlayMoveEnd){
                _needPlayMoveEnd = false;
                //Debug.Log ("滑动状态变为了禁止" + System.DateTime.Now+" ====  "+pointerDelta);
                forceBlockTween();
            }

            //			if (m_Velocity.x != 0) {
            //				Debug.Log (m_Velocity.x);
            //			}
            //如果支持惯性肯定会有m_Velocity
            if (!_dragging && offset == Vector2.zero && _velocity == Vector2.zero){
                if (_inSliding){
                    _inSliding       = false;
                    _needPlayMoveEnd = false;
                    //Debug.Log ("滑动状态变为了禁止" + System.DateTime.Now);
                    forceBlockTween();
                }

                if (_inElasticing){
                    _inElasticing    = false;
                    _needPlayMoveEnd = false;
                    InAnimation      = false;
                    //Debug.Log ("播放回弹动画完成" + System.DateTime.Now);
                }
            }

            if (_viewBounds != _prevViewBounds || _ContentBounds != _prevContentBounds || _content.anchoredPosition != _prevPosition){
                //调用valueChange事件
                _onValueChanged.Invoke(normalizedPosition);
                UpdatePrevData();
                RefreshCells();
            }
        }

        /// <summary>
        /// 强制整块滑动动画
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        private void forceBlockTween(){
            if (ForceBlock){
                float start = GetStartBlockOffset;
                float end   = GetEndBlockOffset();
                float time  = Math.Abs(end - start) / ForceBlockSpeed;
                _forceBlockTweener = DOTween.To(
                                                () => {
                                                    return start;
                                                },
                                                (pos) => {
                                                    if (ScrollDirection == EHorzOrVert.Vertical){
                                                        _container.anchoredPosition = new Vector2(_container.anchoredPosition.x, pos);
                                                    }
                                                    else{
                                                        _container.anchoredPosition = new Vector2(pos, _container.anchoredPosition.y);
                                                    }
                                                },
                                                end,
                                                time
                                               ).
                                             SetEase(ForceBlockEase).
                                             OnComplete(
                                                        () => {
                                                            InAnimation = false;
                                                        }
                                                       );
            }
            else{
                InAnimation = false;
            }
        }

        /// <summary>
        /// 获取开始强制整块滑动动画的位置
        /// </summary>
        /// <value>The get start block offset.</value>
        private float GetStartBlockOffset{
            get{
                if (ScrollDirection == EHorzOrVert.Vertical){
                    return _container.anchoredPosition.y;
                }
                else{
                    return _container.anchoredPosition.x;
                }
            }
        }

        /// <summary>
        /// 获取需要到达的强制整块滑动动画的位置
        /// </summary>
        /// <returns>The nearly block slider.</returns>
        /// <param name="slider">Slider.</param>
        private float GetEndBlockOffset(){
            float startAnchoredPosition = GetStartBlockOffset;
            float stepNum               = startAnchoredPosition / StepSize;
            if (stepNum == Convert.ToInt32(stepNum)){
                //刚好是整块滑动
                return startAnchoredPosition;
            }

            int num = 0;
            while (true){
                if (StepSize * num < Math.Abs(startAnchoredPosition) && Math.Abs(startAnchoredPosition) < StepSize * (num + 1)){
                    break;
                }

                num++;
            }

            if (ScrollDirection == EHorzOrVert.Vertical){
                if (pointerDelta.y < 0){
                    //向下滑动,相邻较小的
                    if (startAnchoredPosition > 0){
                        return StepSize * num;
                    }
                    else{
                        return -1 * StepSize * (num + 1);
                    }
                }
                else{
                    //向上滑动，相邻较大的
                    if (startAnchoredPosition > 0){
                        return StepSize * (num + 1);
                    }
                    else{
                        return -1 * StepSize * num;
                    }
                }
            }
            else{
                if (pointerDelta.x < 0){
                    //向左滑动,取相邻较小的
                    if (startAnchoredPosition > 0){
                        return StepSize * num;
                    }
                    else{
                        return -1 * StepSize * (num + 1);
                    }
                }
                else{
                    //向右滑动,取相邻较大的
                    if (startAnchoredPosition > 0){
                        return StepSize * (num + 1);
                    }
                    else{
                        return -1 * StepSize * num;
                    }
                }
            }
        }

        protected void UpdatePrevData(){
            if (_content == null)
                _prevPosition = Vector2.zero;
            else
                _prevPosition = _content.anchoredPosition;
            _prevViewBounds    = _viewBounds;
            _prevContentBounds = _ContentBounds;
        }

        public Vector2 normalizedPosition{
            get{ return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition); }
            set{
                SetNormalizedPosition(value.x, 0);
                SetNormalizedPosition(value.y, 1);
            }
        }

        public float horizontalNormalizedPosition{
            get{
                UpdateBounds();
                if (_ContentBounds.size.x <= _viewBounds.size.x) return (_viewBounds.min.x > _ContentBounds.min.x) ? 1 : 0;
                return (_viewBounds.min.x - _ContentBounds.min.x) / (_ContentBounds.size.x - _viewBounds.size.x);
            }
            set{ SetNormalizedPosition(value, 0); }
        }

        public float verticalNormalizedPosition{
            get{
                UpdateBounds();
                if (_ContentBounds.size.y <= _viewBounds.size.y) return (_viewBounds.min.y > _ContentBounds.min.y) ? 1 : 0;
                ;
                return (_viewBounds.min.y - _ContentBounds.min.y) / (_ContentBounds.size.y - _viewBounds.size.y);
            }
            set{ SetNormalizedPosition(value, 1); }
        }

        private void SetNormalizedPosition(float value, int axis){
            EnsureLayoutHasRebuilt();
            UpdateBounds();
            // How much the content is larger than the view.
            float hiddenLength = _ContentBounds.size[axis] - _viewBounds.size[axis];
            // Where the position of the lower left corner of the content bounds should be, in the space of the view.
            float contentBoundsMinPosition = _viewBounds.min[axis] - value * hiddenLength;
            // The new content localPosition, in the space of the view.
            float   newLocalPosition = _content.localPosition[axis] + contentBoundsMinPosition - _ContentBounds.min[axis];
            Vector3 localPosition    = _content.localPosition;
            if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f){
                localPosition[axis]    = newLocalPosition;
                _content.localPosition = localPosition;
                _velocity[axis]        = 0;
                UpdateBounds();
            }
        }

        private static     float RubberDelta(float overStretching, float viewSize){ return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching); }
        protected override void  OnRectTransformDimensionsChange(){ SetDirty(); }
        public virtual     void  CalculateLayoutInputHorizontal() { }
        public virtual     void  CalculateLayoutInputVertical()   { }
        public virtual     float minWidth                         { get{ return -1; } }
        public virtual     float preferredWidth                   { get{ return -1; } }
        public virtual     float flexibleWidth                    { get{ return -1; } }
        public virtual     float minHeight                        { get{ return -1; } }
        public virtual     float preferredHeight                  { get{ return -1; } }
        public virtual     float flexibleHeight                   { get{ return -1; } }
        public virtual     int   layoutPriority                   { get{ return -1; } }
        public virtual     void  SetLayoutHorizontal()            { _tracker.Clear(); }

        public virtual void SetLayoutVertical(){
            _viewBounds    = new Bounds(_viewRect.rect.center, _viewRect.rect.size);
            _ContentBounds = GetBounds();
        }

        protected void UpdateBounds(){
            _viewBounds    = new Bounds(_viewRect.rect.center, _viewRect.rect.size);
            _ContentBounds = GetBounds();
            if (_content == null) return;
            Vector3 contentSize  = _ContentBounds.size;
            Vector3 contentPos   = _ContentBounds.center;
            var     contentPivot = _content.pivot;
            AdjustBounds(ref _viewBounds, ref contentPivot, ref contentSize, ref contentPos);
            _ContentBounds.size   = contentSize;
            _ContentBounds.center = contentPos;
            if (_movementType == ScrollRect.MovementType.Clamped){
                // Adjust content so that content bounds bottom (right side) is never higher (to the left) than the view bounds bottom (right side).
                // top (left side) is never lower (to the right) than the view bounds top (left side).
                // All this can happen if content has shrunk.
                // This works because content size is at least as big as view size (because of the call to InternalUpdateBounds above).
                Vector2 delta = Vector2.zero;
                if (_viewBounds.max.x > _ContentBounds.max.x){
                    delta.x = Math.Min(_viewBounds.min.x - _ContentBounds.min.x, _viewBounds.max.x - _ContentBounds.max.x);
                }
                else if (_viewBounds.min.x < _ContentBounds.min.x){
                    delta.x = Math.Max(_viewBounds.min.x - _ContentBounds.min.x, _viewBounds.max.x - _ContentBounds.max.x);
                }

                if (_viewBounds.min.y < _ContentBounds.min.y){
                    delta.y = Math.Max(_viewBounds.min.y - _ContentBounds.min.y, _viewBounds.max.y - _ContentBounds.max.y);
                }
                else if (_viewBounds.max.y > _ContentBounds.max.y){
                    delta.y = Math.Min(_viewBounds.min.y - _ContentBounds.min.y, _viewBounds.max.y - _ContentBounds.max.y);
                }

                if (delta.sqrMagnitude > float.Epsilon){
                    contentPos = _content.anchoredPosition + delta;
                    if (!_horizontal) contentPos.x = _content.anchoredPosition.x;
                    if (!_vertical) contentPos.y   = _content.anchoredPosition.y;
                    AdjustBounds(ref _viewBounds, ref contentPivot, ref contentSize, ref contentPos);
                }
            }
        }

        internal static void AdjustBounds(ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize, ref Vector3 contentPos){
            // Make sure content bounds are at least as large as view by adding padding if not.
            // One might think at first that if the content is smaller than the view, scrolling should be allowed.
            // However, that's not how scroll views normally work.
            // Scrolling is *only* possible when content is *larger* than view.
            // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
            // E.g. if pivot is at top, bounds are expanded downwards.
            // This also works nicely when ContentSizeFitter is used on the content.
            Vector3 excess = viewBounds.size - contentSize;
            if (excess.x > 0){
                contentPos.x  -= excess.x * (contentPivot.x - 0.5f);
                contentSize.x =  viewBounds.size.x;
            }

            if (excess.y > 0){
                contentPos.y  -= excess.y * (contentPivot.y - 0.5f);
                contentSize.y =  viewBounds.size.y;
            }
        }

        private readonly Vector3[] m_Corners = new Vector3[4];

        private Bounds GetBounds(){
            if (_content == null) return new Bounds();
            _content.GetWorldCorners(m_Corners);
            var viewWorldToLocalMatrix = _viewRect.worldToLocalMatrix;
            return InternalGetBounds(m_Corners, ref viewWorldToLocalMatrix);
        }

        internal static Bounds InternalGetBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix){
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int j = 0; j < 4; j++){
                Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        private Vector2 CalculateOffset(Vector2 delta){ return InternalCalculateOffset(ref _viewBounds, ref _ContentBounds, _horizontal, _vertical, _movementType, ref delta); }

        internal static Vector2 InternalCalculateOffset(ref Bounds viewBounds, ref Bounds contentBounds, bool horizontal, bool vertical, ScrollRect.MovementType movementType, ref Vector2 delta){
            Vector2 offset = Vector2.zero;
            if (movementType == ScrollRect.MovementType.Unrestricted) return offset;
            Vector2 min = contentBounds.min;
            Vector2 max = contentBounds.max;
            if (horizontal){
                min.x += delta.x;
                max.x += delta.x;
                if (min.x > viewBounds.min.x)
                    offset.x                                = viewBounds.min.x - min.x;
                else if (max.x < viewBounds.max.x) offset.x = viewBounds.max.x - max.x;
            }

            if (vertical){
                min.y += delta.y;
                max.y += delta.y;
                if (max.y < viewBounds.max.y)
                    offset.y                                = viewBounds.max.y - max.y;
                else if (min.y > viewBounds.min.y) offset.y = viewBounds.min.y - min.y;
            }

            return offset;
        }

        protected void SetDirty(){
            if (!IsActive()) return;
            LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
        }

        protected void SetDirtyCaching(){
            if (!IsActive()) return;
            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate(){ SetDirtyCaching(); }
#endif
    }
}