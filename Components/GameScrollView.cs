using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameScrollView : ScrollRect{
        /// <summary>
        /// Using dynamic width
        /// </summary>
        public bool DynamicWidth;

        /// <summary>
        /// Using dynamic height
        /// </summary>
        public bool DynamicHeight;
        
        /// <summary>
        /// Display column count
        /// </summary>
        public int ColumnCount = 1;

        /// <summary>
        /// Display row count
        /// </summary>
        public int RowCount = 1;

        public float Left = 0f;
        public float Top  = 0f;

        /// <summary>
        /// Bottom height
        /// </summary>
        public float Bottom = 0f;

        /// <summary>
        /// The size of the cell.
        /// </summary>
        public Vector2 CellSize = Vector2.zero;

        /// <summary>
        /// The spacing.
        /// </summary>
        public Vector2 Spacing = Vector2.zero;

        /// <summary>
        /// The item prefab
        /// </summary>
        public GameObject ItemPrefab = null;

        /// <summary>
        /// All item collections
        /// </summary>
        protected List<IGameScrollItemData> _DataList = new List<IGameScrollItemData>();

        /// <summary>
        /// The item collection currently in the display area
        /// </summary>
        private List<GameScrollItem> _currentShowList = new List<GameScrollItem>();

        /// <summary>
        /// The index of the begin.
        /// </summary>
        private int _beginIndex = -1;

        /// <summary>
        /// initialization complete
        /// </summary>
        private bool _isInit = false;

        /// <summary>
        /// Forced refresh
        /// </summary>
        private bool _isForce;

        private Action _initCmpCallBack;

        protected override void Awake(){
            base.Awake();
            this.scrollSensitivity = 50f;
            if (Application.isPlaying && this.content != null){
                //			this.content.anchorMin = new Vector2 (0, 1);
                //			this.content.anchorMax = new Vector2 (0, 1);
                //			this.content.sizeDelta = new Vector2 (0, 0);
                //			this.content.anchoredPosition = Vector2.zero;
                if (this.verticalScrollbar != null){
                    this.verticalScrollbar.gameObject.SetActive(false);
                    this.verticalScrollbar.onValueChanged.AddListener(OnValueChanged);
                    this.onValueChanged.RemoveAllListeners();
                }

                if (this.horizontalScrollbar != null){
                    this.horizontalScrollbar.gameObject.SetActive(false);
                    this.horizontalScrollbar.onValueChanged.AddListener(OnValueChanged);
                    this.onValueChanged.RemoveAllListeners();
                }

                //			if (this.verticalScrollbar == null && this.horizontalScrollbar == null) {
                this.onValueChanged.AddListener(OnValueChanged);
                //			}
            }
        }

        /// <summary>
        /// Override initial slide
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public override void OnInitializePotentialDrag(UnityEngine.EventSystems.PointerEventData eventData){
            base.OnInitializePotentialDrag(eventData);
            _isInit = true;
        }

        public override void Rebuild(CanvasUpdate executing){
            //		base.Rebuild (executing);
        }

        protected override void LateUpdate(){
            //When there is no initialization to return directly, here is to prevent the scrollview from scrolling when it is added to the item.
            if (!this._isInit){
                return;
            }

            base.LateUpdate();
        }

        /// <summary>
        /// Set the data source
        /// </summary>
        /// <param name="data">Datas.</param>
        /// <typeparam name="T">IGameScrollItemData.</typeparam>
        public void SetData<T>(List<T> data, Action callBack = null) where T : IGameScrollItemData{
            _DataList.Clear();
            for (int i = 0; i < data.Count; i++){
                _DataList.Add(data[i]);
            }

            _initCmpCallBack = callBack;
            ResetSizeDelta();
            InitItemPrefab();
            //		this.RefreshShowList (true);
        }

        /// <summary>
        /// add data source
        /// </summary>
        /// <param name="datas">Datas.</param>
        /// <typeparam name="T">IGameScrollItemData.</typeparam>
        public void AddData<T>(List<T> datas,bool refresh) where T : IGameScrollItemData{
            for (int i = 0; i < datas.Count; i++){
                _DataList.Add(datas[i]);
            }

            ResetSizeDelta();
            if (refresh){
                if (_DataList.Count <= _currentShowList.Count){
                    this.RefreshShowList();
                }
                else{
                    InitItemPrefab();
                }
            }
        }

        /// <summary>
        /// add data source
        /// </summary>
        /// <param name="data">data.</param>
        /// <typeparam name="T">IGameScrollItemData.</typeparam>
        public void AddData<T>(T data, bool refresh) where T : IGameScrollItemData{
            _DataList.Add(data);
            ResetSizeDelta();
            if (refresh){
                if (_DataList.Count <= _currentShowList.Count){
                    this.RefreshShowList();
                }
                else{
                    InitItemPrefab();
                }
            }
        }

        private void InitItemPrefab(){
            if (_DataList == null || _DataList.Count == 0){
                this.RefreshShowList(true);
                if (_initCmpCallBack != null){
                    _initCmpCallBack.Invoke();
                }

                return;
            }

            if (ItemPrefab == null){
                Debug.LogError("GameScrollRect' ItemPrefab is null.");
                return;
            }

            if (_currentShowList.Count >= this.RowCount * this.ColumnCount){
                this.RefreshShowList(true);
                if (_initCmpCallBack != null){
                    _initCmpCallBack.Invoke();
                }

                return;
            }

            int count = Mathf.Clamp(_DataList.Count, 0, this.RowCount * this.ColumnCount);
            int num   = 0;
            for (int i = 0; i < count; i++){
                if (i < _currentShowList.Count){
                    _currentShowList[i].SetData(_DataList[_beginIndex + i]);
                    num++;
                    if (num >= count){
                        this.RefreshShowList(true);
                        if (_initCmpCallBack != null){
                            _initCmpCallBack.Invoke();
                        }
                    }

                    continue;
                }

                //有问题，Refresh里边没有涉及InitPrefab内容
                // UnityEngine.GameObject it   = AssetSystem.LoadByMedian(PathConfig.BundleRelated.UI_BUNDLE, ItemPrefabPath, ItemMediaPath, typeof(GameObject)) as GameObject;
                GameObject     ga   = GameObject.Instantiate(ItemPrefab,this.content);
                GameScrollItem item = ga.GetComponent<GameScrollItem>();
                item.gameObject.SetActive(false);
                this.Add(item);
                this._isForce = true;
                num++;
                if (num >= count){
                    this.RefreshShowList(true);
                    if (_initCmpCallBack != null){
                        _initCmpCallBack.Invoke();
                    }
                }
            }
        }

        private void Add(GameScrollItem gameScrollRectItem){
            if (_currentShowList.Count >= this.RowCount * this.ColumnCount || _DataList.Count <= _currentShowList.Count){
                GameObject.Destroy(gameScrollRectItem.gameObject);
                return;
            }

            //		currentShowList.Add (gameScrollRectItem);
            //		gameScrollRectItem.SetData (dataList [currentShowList.Count - 1]);
            //		this.Add (gameScrollRectItem.transform);
            if (this.Add(gameScrollRectItem.transform)){
                _currentShowList.Add(gameScrollRectItem);
                gameScrollRectItem.SetData(_DataList[_currentShowList.Count - 1]);
            }
        }

        private bool Add(Transform transform){ return Add(transform as RectTransform); }

        private bool Add(RectTransform rectTransform){
            if (rectTransform == null){
                return false;
            }

            if (this.content == null){
                GameObject.Destroy(rectTransform.gameObject);
                return false;
            }

            rectTransform.gameObject.SetActive(true);
            rectTransform.SetParent(this.content);
            rectTransform.transform.localScale = Vector3.one;
            rectTransform.pivot                = new Vector2(0, 1);
            rectTransform.anchorMin            = new Vector2(0, 1);
            rectTransform.anchorMax            = new Vector2(0, 1);
            rectTransform.sizeDelta            = CellSize;
            rectTransform.anchoredPosition     = new Vector2(1000, 1000);
            ResetSizeDelta();
            IEShowInit();
            return true;
        }

        protected virtual void ResetSizeDelta(){
            //Horizontal and vertical
            if (this.horizontal && this.vertical){
                int wh = Mathf.CeilToInt(Mathf.Pow(_DataList.Count, 0.5f));
                this.content.sizeDelta = new Vector2(itemSize.x * wh, itemSize.y * wh + Top + Bottom);
            }
            else if (this.horizontal){
                //Horizontal
                int totalColumnCount = _DataList.Count / RowCount;
                if (_DataList.Count % RowCount > 0){
                    totalColumnCount++;
                }

                int totalRowCount = RowCount; //dataList.Count < rowCount ? dataList.Count : rowCount;
                this.content.sizeDelta = new Vector2(itemSize.x * totalColumnCount, itemSize.y * totalRowCount + Top + Bottom);
            }
            else if (this.vertical){
                // vertical
                int totalRowCount = _DataList.Count / ColumnCount;
                if (_DataList.Count % ColumnCount > 0){
                    totalRowCount++;
                }

                int totalColumnCount = ColumnCount; //dataList.Count < columnCount ? dataList.Count : columnCount;
                this.content.sizeDelta = new Vector2(itemSize.x * totalColumnCount, itemSize.y * totalRowCount + Top + Bottom);
            }
        }

        /// <summary>
        /// Drag the finished event.
        /// </summary>
        private void OnValueChanged(){
            int beginIndexTemp = GetMinIndex();
            if (!_isForce){
                if ((beginIndexTemp == _beginIndex && _currentShowList.Count >= pageSize) || beginIndexTemp < 0){
                    return;
                }
            }

            _beginIndex = beginIndexTemp;
            RefreshShowList(_isForce);
            if (_isForce){
                _isForce = false;
            }
        }

        public virtual void OnValueChanged(Vector2 v){ OnValueChanged(); }

        public void OnValueChanged(float value){
            this._isInit = true;
            OnValueChanged();
        }

        protected void RefreshShowList(bool force = false){
            for (int i = 0; i < _currentShowList.Count;){
                if (_currentShowList[i] != null){
                    _currentShowList[i].gameObject.SetActive(false);
                    i++;
                }
                else{
                    _currentShowList.RemoveAt(i);
                }
            }

            int count = Mathf.Clamp(_currentShowList.Count, 0, pageSize);
            //Display objects that appear within the display range
            Vector2 prePos = Vector2.zero;
            for (int i = _beginIndex; i < _DataList.Count && i < _beginIndex + count; i++){
                int index = i % count;
                _currentShowList[index].gameObject.SetActive(true);
                _currentShowList[index].transform.SetParent(this.content);
                _currentShowList[index].SetData(_DataList[i]);
                // SetItemPosition(i, _currentShowList[index].transform as RectTransform);
                // 新方式,考虑动态item 宽高
                if (DynamicHeight || DynamicWidth){
                    prePos = SetItemPosition(i,_currentShowList[index],prePos);
                }
                else{
                    SetItemPosition(i, _currentShowList[index].transform as RectTransform);
                }
            }
        }

        /// <summary>
        /// Sort the specified dlgSort.
        /// </summary>
        /// <param name="dlgSort">Dlg sort.</param>
        public void Sort(int index, int count, IComparer<IGameScrollItemData> comparer){
            _DataList.Sort(index, count, comparer);
            RefreshShowList(true);
        }

        /// <summary>
        /// Sort the specified dlgSort.
        /// </summary>
        /// <param name="dlgSort">Dlg sort.</param>
        public virtual void Sort(IComparer<IGameScrollItemData> comparer){
            _DataList.Sort(comparer);
            RefreshShowList(true);
        }

        /// <summary>
        /// Set item coordinates
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="rectTransform">Rect transform.</param>
        protected virtual void SetItemPosition(int index, RectTransform rectTransform){
            int x = 0;
            int y = 0;
            if (this.horizontal){
                x = index / RowCount;
                y = -index % RowCount;
            }

            if (this.vertical){
                y = -index / ColumnCount;
                x = index % ColumnCount;
            }

            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale    = Vector3.one;
            //        rectTransform.anchoredPosition = new Vector2(x * itemSize.x + spacing.x, y * itemSize.y - spacing.y);
            Vector2 newPos = new Vector2(x * itemSize.x + Left, y * itemSize.y - Top);
            rectTransform.anchoredPosition = newPos;
        }
        
        /// <summary>
        /// Set item coordinates
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="srItem">Rect transform.</param>
        protected virtual Vector2 SetItemPosition(int index, GameScrollItem srItem, Vector2 prePos){
            Vector2 newPos = Vector2.zero;
            int     x      = 0;
            int     y      = 0;
            if (this.horizontal){
                x = index / RowCount;
                y = -index % RowCount;
            }

            if (this.vertical){
                y = -index / ColumnCount;
                x = index % ColumnCount;
            }

            float width  = x * itemSize.x + Left;
            float height = y * itemSize.y - Top;
            if (DynamicWidth){
                width = prePos.x;
            }

            if (DynamicHeight){
                height = prePos.y;
            }

            srItem.UIRect().localPosition    = Vector3.zero;
            srItem.UIRect().localScale       = Vector3.one;
            newPos                           = new Vector2(width,height );
            srItem.UIRect().anchoredPosition = newPos;
            
            if (DynamicWidth){
                newPos.x += srItem.GetWidth() + Spacing.x + Left;
            }

            if (DynamicHeight){
                newPos.y += -srItem.GetHeight() - Spacing.y - Top; 
            }
            
            return newPos;
        }

        /// <summary>
        /// Get the current minimum index
        /// </summary>
        /// <returns>The index.</returns>
        protected virtual int GetMinIndex(){
            int index = 0;
            if (this.horizontal){
                if (this.content.anchoredPosition.x > 0){
                    return 0;
                }

                float x = Mathf.Abs(this.content.anchoredPosition.x);
                index = Mathf.FloorToInt(x / (itemSize.x * this.content.localScale.x)) * RowCount;
                if (index + pageSize > _DataList.Count){
                    index = _DataList.Count - pageSize;
                }
            }

            if (this.vertical){
                if (this.content.anchoredPosition.y < 0){
                    return 0;
                }

                float y = Mathf.Abs(this.content.anchoredPosition.y);
                index = Mathf.FloorToInt(y / (itemSize.y * this.content.localScale.y)) * ColumnCount;
                if (index + pageSize > _DataList.Count){
                    index = _DataList.Count - pageSize;
                }
            }

            if (index < 0){
                index = 0;
            }

            return index;
        }

        /// <summary>
        /// Current display area item number
        /// </summary>
        /// <value>The size of the get page.</value>
        protected int pageSize{ get{ return ColumnCount * RowCount; } }

        protected Vector2 itemSize{ get{ return CellSize + Spacing; } }

        private void IEShowInit(){
            if (this.content.childCount <= this.pageSize){
                OnValueChanged();
            }

            if (this.horizontalScrollbar != null){
                //			this.horizontalScrollbar.gameObject.SetActive (this.horizontal);
                this.horizontalScrollbar.size  = (this.transform as RectTransform).sizeDelta.x / this.content.sizeDelta.x;
                this.horizontalScrollbar.value = 1f;
            }

            if (this.verticalScrollbar != null){
                //			this.verticalScrollbar.gameObject.SetActive (this.vertical);
                this.verticalScrollbar.size  = (this.transform as RectTransform).sizeDelta.y / this.content.sizeDelta.y;
                this.verticalScrollbar.value = 1f;
            }
        }

        /// <summary>
        /// Remove specified element
        /// </summary>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="isDestroy">release the element.</param>
        public void Remove(IGameScrollItemData data){
            this._DataList.Remove(data);
            _isForce = true;
            IEShowInit();
        }

        /// <summary>
        /// Remove all elements
        /// </summary>
        /// <param name="isDestroy">release the element.</param>
        public void RemoveAll(bool isDestroy = true){
            if (_DataList == null) return;
            _DataList.Clear();
            while (_currentShowList.Count > 0){
                GameObject item = _currentShowList[0].gameObject;
                _currentShowList.RemoveAt(0);
                item.SetActive(false);
                if (isDestroy){
                    GameObject.Destroy(item);
                }
            }

            this.content.anchoredPosition = Vector2.zero;
            this.content.sizeDelta        = Vector2.zero;
            _isForce                      = true;
            _beginIndex                   = 0;
            OnValueChanged();
        }

        public void ResetRefresh(){ this.RefreshShowList(true); }
    }
}