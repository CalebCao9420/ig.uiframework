using System.Collections;
using System.Collections.Generic;
using IG.AssetBundle;
using IG.Module.Language;
using IG.Runtime.Common.Timer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GamePageScrollView : MonoBehaviour, IDragHandler, IEndDragHandler{
        public bool          Horizontal;
        public bool          Vertical;
        public RectTransform Content;
        public RectTransform Points;

        /// <summary>
        /// show column count
        /// </summary>
        public int ColumnCount = 1;

        /// <summary>
        /// shou row count
        /// </summary>
        public int RowCount = 1;

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
        public string ItemPrefabPath = null;

        /// <summary>
        /// The point path.
        /// </summary>
        public string PointPath;

        /// <summary>
        /// The animation time.
        /// </summary>
        public float AnimationTime = 0.5f;

        /// <summary>
        /// item list
        /// </summary>
        private List<IGameScrollItemData> _dataList;

        /// <summary>
        /// The item collection currently in the display area
        /// </summary>
        private List<GameScrollItem> _currentShowList;

        /// <summary>
        /// The index of the begin.
        /// </summary>
        private int _beginIndex = 0;

        private UnityTimer     _timer;
        private int            _oldBeginIndex = 0;
        private GameScrollItem _currentGameScrollItem;
        private bool           _isPlayAnim = false;
        private bool           _isEndDrag  = false;
        private int            _isNextPage = 0;
        private bool           _isDrag     = true;

        /// <summary>
        /// Current display area item number
        /// </summary>
        /// <value>The size of the get page.</value>
        protected int _PageSize{ get{ return ColumnCount * RowCount; } }

        protected Vector2 _ItemSize{ get{ return CellSize + Spacing; } }

        void Awake(){
            _dataList        = new List<IGameScrollItemData>();
            _currentShowList = new List<GameScrollItem>();
        }

        private void InitContent(){
            this.Content.sizeDelta = Vector2.zero;
            //		if (this.horizontal) {
            //			this.content.sizeDelta = new Vector2 (this.dataList.Count / rowCount * itemSize.x, itemSize.y);
            //		}
            //
            //		if (this.vertical) {
            //			this.content.sizeDelta = new Vector2 (itemSize.x, this.dataList.Count / columnCount * itemSize.y);
            //		}
        }

        /// <summary>
        /// Set the data source
        /// </summary>
        /// <param name="data">Datas.</param>
        /// <typeparam name="T">IGameScrollItemData.</typeparam>
        public void SetData<T>(List<T> data) where T : IGameScrollItemData{
            this._dataList.Clear();
            int len = data?.Count ?? 0;
            for (int i = 0; i < len; ++i){
                _dataList.Add(data[i]);
            }

            _isDrag = _dataList.Count > 1;
            InitContent();
            InitItemPrefab();
            if (_isDrag){
                if (_timer != null){
                    this._timer = UnityTimer.SetInterval(3.0f, AutoChangePageEvent);
                }
            }
        }

        private void AutoChangePageEvent(){ OnNextPage(); }

        private void InitItemPrefab(){
            if (_dataList == null || _dataList.Count == 0){
                return;
            }

            if (string.IsNullOrEmpty(ItemPrefabPath)){
                Debug.LogError("GameScrollRect' ItemPrefabPath is null.");
                return;
            }

            if (_currentShowList.Count >= this.RowCount * this.ColumnCount){
                return;
            }

            int count = this.RowCount * this.ColumnCount;
            if (_currentShowList.Count == count){
                this.RefreshData();
                return;
            }

            for (int i = 0; i < count; i++){
                if (i < _currentShowList.Count){
                    continue;
                }

                AssetSystem.LoadAsync(
                                      (o, oArg) => {
                                          GameScrollItem item = (o as GameObject).GetComponent<GameScrollItem>();
                                          item.transform.SetParent(this.Content);
                                          item.gameObject.SetActive(false);
                                          this.Add(item);
                                          //				this.ResetPos ();
                                          if (_currentShowList.Count == count){
                                              this.RefreshData();
                                          }
                                      },
                                      ItemPrefabPath,
                                      typeof(GameObject)
                                     );
            }

            if (this.Points.childCount >= _dataList.Count){
                RestPoints();
                return;
            }

            if (_isDrag){
                for (int i = 0; i < _dataList.Count; i++){
                    if (!string.IsNullOrEmpty(this.PointPath)){
                        StartCoroutine(ResetPointsPos(true));
                        AssetSystem.LoadAsync(
                                              (o, oArg) => {
                                                  ScrollViewPoint item = (o as GameObject).GetComponent<ScrollViewPoint>();
                                                  item.transform.SetParent(this.Points);
                                                  if (this.Points.childCount > _dataList.Count){
                                                      Destroy(item.gameObject);
                                                      return;
                                                  }

                                                  if (this.Points.childCount == _dataList.Count){
                                                      StartCoroutine(ResetPointsPos(false));
                                                  }

                                                  RestPoints();
                                              },
                                              PointPath,
                                              typeof(GameObject)
                                             );
                    }
                }
            }
        }

        private IEnumerator ResetPointsPos(bool enabled){
            if (!enabled){
                yield return new WaitForEndOfFrame();
            }

            ContentSizeFitter contentSizeFitter = this.Points.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter != null){
                contentSizeFitter.enabled = enabled;
            }

            HorizontalLayoutGroup horizontalLayoutGroup = this.Points.GetComponent<HorizontalLayoutGroup>();
            if (horizontalLayoutGroup != null){
                horizontalLayoutGroup.enabled = enabled;
            }
        }

        private void RestPoints(){
            float childCount = this.Points.childCount;
            for (int i = 0; i < childCount; i++){
                RectTransform   pointRT         = this.Points.GetChild(i).UIRect();
                ScrollViewPoint scrollViewPoint = pointRT.GetComponent<ScrollViewPoint>();
                float           w               = scrollViewPoint.Size.x;
                //			pointRT.anchoredPosition = new Vector2 (i * w - (childCount * w) / 2, 0);
                scrollViewPoint.Init(i, OnPointChange);
                scrollViewPoint.OnSelected(this._beginIndex);
            }
        }

        private void RefreshPoints(){
            float childCount = this.Points.childCount;
            for (int i = 0; i < childCount; i++){
                RectTransform   pointRT         = this.Points.GetChild(i).UIRect();
                ScrollViewPoint scrollViewPoint = pointRT.GetComponent<ScrollViewPoint>();
                scrollViewPoint.OnSelected(this._beginIndex);
            }
        }

        private void InitItemAnchor(RectTransform rectTransform){
            if (rectTransform == null){
                return;
            }

            rectTransform.gameObject.SetActive(true);
            rectTransform.SetParent(this.Content);
            rectTransform.transform.localScale = Vector3.one;
            if (this.Vertical){
                rectTransform.pivot = new Vector2(0.5f, 0f);
            }
            else{
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
            }

            rectTransform.anchorMin        = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax        = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta        = CellSize;
            rectTransform.anchoredPosition = new Vector2(1000, 0);
        }

        private void Add(GameScrollItem gameScrollRectItem){
            if (_currentShowList.Count >= this.RowCount * this.ColumnCount){
                Destroy(gameScrollRectItem.gameObject);
                return;
            }

            _currentShowList.Add(gameScrollRectItem);
            //		gameScrollRectItem.SetData (dataList [currentShowList.Count - 1]);
            this.InitItemAnchor(gameScrollRectItem.transform as RectTransform);
        }

        private int currentShowIndex{
            get{
                int count = Mathf.Clamp(_currentShowList.Count, 0, _PageSize);
                if (count == 0){
                    return 0;
                }

                int index = _beginIndex % count;
                if (index < 0){
                    index = _currentShowList.Count + index;
                }

                return index;
            }
        }

        public void OnEndDrag(PointerEventData eventData){
            if (this._isPlayAnim){
                return;
            }

            if (!_isDrag){
                return;
            }

            this._isEndDrag = true;
            this.ResetPos();
            this.RefreshPoints();
        }

        public void OnDrag(PointerEventData eventData){
            if (this._isPlayAnim){
                return;
            }

            if (!_isDrag){
                return;
            }

            float x = 0;
            float y = 0;
            if (this.Horizontal){
                x = Mathf.Clamp(eventData.delta.x, -70, 70);
            }

            if (this.Vertical){
                y = Mathf.Clamp(eventData.delta.y, -70, 70);
            }

            Content.anchoredPosition += new Vector2(x, y);
        }

        private void ResetPos(){
            if (_currentShowList.Count == 0){
                return;
            }

            this._isNextPage = 0;
            if (this.Horizontal){
                float x = Content.anchoredPosition.x;
                if (Mathf.Abs(x) > 100){
                    if (x > 0){
                        this.LastPage();
                    }
                    else{
                        this.NextPage();
                    }
                }
            }

            if (this.Vertical){
                float y = Content.anchoredPosition.y;
                if (Mathf.Abs(y) > 100){
                    if (y > 0){
                        this.LastPage();
                    }
                    else{
                        this.NextPage();
                    }
                }
            }

            Move(_beginIndex);
        }

        protected virtual void LocalItemScale(RectTransform rectTransform){ }

        private void Move(int index){
            Vector2 pos = Vector2.zero;
            if (this.Horizontal){
                if (this._isEndDrag && (index == _beginIndex || index == _oldBeginIndex)){
                    this._isPlayAnim = true;
                    float t = (this._ItemSize.x * (this.ColumnCount - 2) - Mathf.Abs(this.Content.anchoredPosition.x)) / this._ItemSize.x * AnimationTime;
                    if (this._isNextPage != 0){
                        if (this._isNextPage == 1){
                            pos = new Vector2(-this._ItemSize.x * (this.ColumnCount - 2), 0);
                        }
                        else{
                            pos = new Vector2(this._ItemSize.x * (this.ColumnCount - 2), 0);
                        }
                    }
                    else{
                        t = this.Content.anchoredPosition.x / this._ItemSize.x * AnimationTime;
                    }

                    this.Content.DOAnchorPos(pos, t).
                         OnComplete(
                                    () => {
                                        this._isPlayAnim = false;
                                        this._isEndDrag  = false;
                                        this.RefreshData();
                                    }
                                   );
                }
                else{
                    this.Content.anchoredPosition = pos;
                }
            }

            if (this.Vertical){
                pos = Vector2.zero;
                if (this._isEndDrag && (index == _beginIndex || index == _oldBeginIndex)){
                    this._isPlayAnim = true;
                    float t = Mathf.Abs(pos.y - this.Content.anchoredPosition.y * (this.RowCount - 2)) / this._ItemSize.y * AnimationTime;
                    if (this._isNextPage != 0){
                        if (this._isNextPage == 1){
                            pos = new Vector2(0, -this._ItemSize.y * (this.RowCount - 2));
                        }
                        else{
                            pos = new Vector2(0, this._ItemSize.y * (this.RowCount - 2));
                        }
                    }
                    else{
                        t = this.Content.anchoredPosition.x / this._ItemSize.x * AnimationTime;
                    }

                    this.Content.DOAnchorPos(pos, t).
                         OnComplete(
                                    () => {
                                        this._isPlayAnim = false;
                                        this._isEndDrag  = false;
                                        this.RefreshData();
                                    }
                                   );
                }
                else{
                    this.Content.anchoredPosition = pos;
                }
            }
        }

        private void NextPage(){
            if (this._currentShowList.Count == 0){
                return;
            }

            this._isNextPage = 1;
            _oldBeginIndex   = _beginIndex;
            _beginIndex++;
            if (_beginIndex >= _dataList.Count){
                _beginIndex = 0;
            }

            GameScrollItem temp = this._currentShowList[0];
            this._currentShowList.RemoveAt(0);
            this._currentShowList.Add(temp);
            this.RefreshPoints();
        }

        private void LastPage(){
            if (this._currentShowList.Count == 0){
                return;
            }

            this._isNextPage = -1;
            _oldBeginIndex   = _beginIndex;
            _beginIndex--;
            if (_beginIndex < 0){
                _beginIndex = _dataList.Count - 1;
            }

            GameScrollItem temp = this._currentShowList[this._currentShowList.Count - 1];
            this._currentShowList.Remove(temp);
            this._currentShowList.Insert(0, temp);
            this.RefreshPoints();
        }

        private void OnPointChange(int index){
            if (this._isPlayAnim || index == _beginIndex){
                return;
            }

            _oldBeginIndex   = _beginIndex;
            _beginIndex      = index;
            this._isNextPage = _beginIndex > _oldBeginIndex ? 1 : -1;
            this._isEndDrag  = true;
            Move(_beginIndex);
            RefreshPoints();
        }

        public void OnLastPage(){
            if (this._isPlayAnim){
                return;
            }

            this.LastPage();
            this._isEndDrag = true;
            Move(_beginIndex);
            this.RefreshPoints();
        }

        public void OnNextPage(){
            if (this._isPlayAnim){
                return;
            }

            this.NextPage();
            this._isEndDrag = true;
            Move(_beginIndex);
            this.RefreshPoints();
        }

        private void RefreshData(){
            for (int i = 0; i < _currentShowList.Count; i++){
                _currentShowList[i].transform.parent = this.Content.parent;
            }

            this.Content.anchoredPosition = Vector2.zero;
            int index = this._beginIndex - this.ColumnCount / 2;
            for (int i = 0; i < _currentShowList.Count; i++){
                _currentShowList[i].transform.parent = this.Content;
                if (this.Horizontal){
                    _currentShowList[i].UIRect().anchoredPosition = new Vector2((index - this._beginIndex) * this._ItemSize.x, 0);
                }

                if (this.Vertical){
                    _currentShowList[i].UIRect().anchoredPosition = new Vector2(0, (index - this._beginIndex) * this._ItemSize.y);
                }

                if (index < 0){
                    _currentShowList[i].SetData(this._dataList[this._dataList.Count + index]);
                }
                else if (index >= this._dataList.Count){
                    _currentShowList[i].SetData(this._dataList[index - this._dataList.Count]);
                }
                else{
                    _currentShowList[i].SetData(this._dataList[index]);
                }

                index++;
            }
        }
    }
}