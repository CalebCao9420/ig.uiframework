using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class GameInputField : InputField{
    #region Place holder

        private Text       _placeholder{ get; set; }
        public  Text       Placeholder { get{ return this._placeholder ?? (this._placeholder = this.placeholder.gameObject.GetComponent<Text>()); } }
        public  UIVertex[] CursorVerts { get{ return this.m_CursorVerts; } }

    #endregion

        private GameInputCaret _gameInputCaret;

        protected override void Awake(){
            base.Awake();
            _gameInputCaret = this.GetComponentInChildren<GameInputCaret>();
            if (_gameInputCaret != null){
                _gameInputCaret.SetGameInputField(this);
            }
        }

        // UnityEngine.Update
        protected override void LateUpdate(){
            base.LateUpdate();
            this.OnUpdateCaret();
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData){
            base.OnPointerDown(eventData);
            this.InitCaret();
        }

    #region Caret

        public  Vector2       CursorOffset = Vector2.zero;
        private bool          _initCaretFlg{ get; set; }
        private RectTransform _caret       { get; set; }
        private RectTransform _inputCaret  { get{ return this._caret ??= this.Find(this.gameObject.name + " Input Caret") as RectTransform; } }

        private void InitCaret(){
            if (_gameInputCaret != null){
                _gameInputCaret.gameObject.SetActive(true);
                _gameInputCaret.PlayAnimation();
            }

            if (this._initCaretFlg) return;
            if (this._inputCaret == null) return;
            this.InitCaretAnchoredPosition();
            this.InitCustomCaret();
            this._initCaretFlg = true;
        }

        private void          InitCaretAnchoredPosition(){ this._inputCaret.anchoredPosition = new Vector2(this._inputCaret.anchoredPosition.x + CursorOffset.x, this._inputCaret.anchoredPosition.y + CursorOffset.y); }
        public  RectTransform CustomCaret;
        private bool          _isUpdateCaret{ get; set; }

        private void InitCustomCaret(){
            if (this.CustomCaret == null) return;
            this.onEndEdit.AddListener(OnCaretEndEdit);
            this.customCaretColor = true;
            var c = this.caretColor;
            c.a             = 0;
            this.caretColor = c;
            this.CustomCaret.transform.SetParent(this._inputCaret, true);
            this.CustomCaret.anchorMin        = new Vector2(0,    0.5f);
            this.CustomCaret.anchorMax        = new Vector2(0,    0.5f);
            this.CustomCaret.pivot            = new Vector2(0.2f, 0.5f);
            this.CustomCaret.anchoredPosition = -CursorOffset;
            this._isUpdateCaret               = true;
            this.CustomCaret.gameObject.SetActive(false);
        }

        public void OnUpdateCaret(){
            if (!this._isUpdateCaret) return;
            this.UpdateCaretPosition();
            this.UpdateCaretEnabled();
        }

        [SerializeField]
        protected float _CaretStartPoint;

        private float _initCaretPoint        { get; set; }
        public  bool  _initCacheCaretPointFlg{ get; private set; }

        private Vector2 _cursorVertsPoint{
            get{
                var x = this.m_CursorVerts[0].position.x + (this.m_CursorVerts[1].position.x - this.m_CursorVerts[0].position.x) * 0.5f;
                var y = this.m_CursorVerts[1].position.y + (this.m_CursorVerts[2].position.y - this.m_CursorVerts[1].position.y) * 0.5f;
                return new Vector2(x, y);
            }
        }

        private void InitCacheCaretPoint(){
            if (!this._initCacheCaretPointFlg){
                this._initCaretPoint         = this._cursorVertsPoint.x;
                this._initCacheCaretPointFlg = true;
            }
        }

        public void UpdateCaretPosition(){
            if (this.m_CursorVerts        == null) return;
            if (this.m_CursorVerts.Length < 1) return;
            this.InitCacheCaretPoint();
            if (this._cursorVertsPoint.x == this._initCaretPoint){
                this.CustomCaret.anchoredPosition = new Vector2(this._CaretStartPoint, this._cursorVertsPoint.y);
            }
            else{
                this.CustomCaret.localPosition = this._cursorVertsPoint;
            }
        }

        public void UpdateCaretEnabled(){
            if (!this._initCacheCaretPointFlg) return;
            if (this.CustomCaret.gameObject.activeSelf == this.m_CaretVisible) return;
            this.CustomCaret.gameObject.SetActive(this.m_CaretVisible);
        }

        public void OnCaretEndEdit(string str){
            if (_gameInputCaret != null){
                _gameInputCaret.gameObject.SetActive(false);
            }

            if (this.CustomCaret == null) return;
            this.m_CaretVisible = false;
        }

    #endregion
    }
}