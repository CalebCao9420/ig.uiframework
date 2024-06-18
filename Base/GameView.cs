using System;
using System.Collections;
using System.Text.RegularExpressions;
using IG.AssetBundle;
using IG.Events;
using IG.IO;
using IG.Runtime.Log;
using UnityEditor;
using UnityEngine;

namespace IG.Module.UI{
    public class GameView : EventMonoBehaviour{
        /// <summary>
        /// The move in time.
        /// </summary>
        // public float MoveInTime = 0.3f;

        /// <summary>
        /// The move out time.
        /// </summary>
        // public float MoveOutTime = 0.3f;

        /// <summary>
        /// The delay destroy time.
        /// </summary>
        public virtual float DelayDestroyTime => 5;

        public bool IsNeverDestroy = false;
        public bool IsBackShow     = false;

        /// <summary>
        /// The escape flg.
        /// </summary>
        public bool EscFlg = true;

        public RectTransform RectTransform => this.transform as RectTransform;

        /// <summary>
        /// The is show complete.
        /// </summary>
        private bool _isShowComplete = false;

        /// <summary>
        /// The first initialize flg.
        /// </summary>
        private bool _firstInitializeFlg = true;

        /// <summary>
        /// The state of the game view.
        /// </summary>
        private GameViewState _gameViewState = GameViewState.CLOSE;

        /// <summary>
        /// The on move in or out complete.
        /// </summary>
        protected Action<MoveAnimationState> _OnMoveInCompleteCallBack;

        protected Action<MoveAnimationState> _OnMoveOutCompleteCallBack;
        protected bool                       _IsIgnoreLoading = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GameView"/> is active.
        /// </summary>
        /// <value><c>true</c> if is active; otherwise, <c>false</c>.</value>
        public bool IsActive{ get; protected set; }

        /// <summary>
        /// Show the specified isFoce and onMoveInComplete.
        /// </summary>
        /// <param name="isFoce">If set to <c>true</c> is foce.</param>
        /// <param name="onMoveInComplete">On move in complete.</param>
        public void Show(bool isFoce = false, Action<MoveAnimationState> onMoveInComplete = null){ PanelController.Instance.StartCoroutine(PShow(isFoce, onMoveInComplete)); }

        /// <summary>
        /// Ps the show.
        /// </summary>
        /// <returns>The show.</returns>
        /// <param name="isFoce">If set to <c>true</c> is foce.</param>
        /// <param name="onMoveInComplete">On move in complete.</param>
        private IEnumerator PShow(bool isFoce, Action<MoveAnimationState> onMoveInComplete){
            AddEvent();
            if (_firstInitializeFlg){
                SingleAPIRequest();
                //TODO:联网
                // if (!isIgnoreLoading) {
                // 	// wait api
                // 	while (!APIManager.GetInstance ().IsNoApiPerform) {
                // 		yield return new WaitForFixedUpdate ();
                // 	}
                // }
            }

            MultipleAPIRequest();
            //TODO:联网
            // if (!isIgnoreLoading) {
            // 	// wait api
            // 	while (!APIManager.GetInstance ().IsNoApiPerform) {
            // 		yield return new WaitForFixedUpdate ();
            // 	}
            // }

            if (_gameViewState == GameViewState.HIDE){
                LogHelper.Log($"GameView状态:{_gameViewState}");
            }

            if (this == null){
                yield break;
            }

            gameObject.SetActive(true);
            _gameViewState = GameViewState.SHOW;
            if (_firstInitializeFlg){
                _firstInitializeFlg = false;
                InitData();
            }

            ShowData();
            this.IsBackShow                = false;
            this._OnMoveInCompleteCallBack = onMoveInComplete;

            //begin animation
            if (this._OnMoveInCompleteCallBack != null){
                this._OnMoveInCompleteCallBack(MoveAnimationState.StartMoveIn);
            }

            if (isFoce){
                OnMoveInComplete();
            }
            else{
                PlayMoveInAnimation();
            }
        }

        /// <summary>
        /// Singles the API request.
        /// </summary>
        protected virtual void SingleAPIRequest(){ }

        /// <summary>
        /// Multiples the API request.
        /// </summary>
        protected virtual void MultipleAPIRequest(){ }

        public virtual void CloseAPIRequest(Action action){
            if (action != null){
                action();
            }
        }

        /// <summary>
        /// Inits the data.
        /// </summary>
        protected virtual void InitData(){ }

        /// <summary>
        /// Shows the data.
        /// </summary>
        protected virtual void ShowData(){ }

        /// <summary>
        /// If it is a view that allows this method is called to shut down, if is a panel can not invoke this method directly.
        /// </summary>
        /// <param name="isFoceHide"></param>
        public void Close(bool isFoce, Action<MoveAnimationState> onMoveOutComplete, bool isJumpPanel = false){
            CommonMaskController.Hide(this.gameObject);
            this._OnMoveOutCompleteCallBack = onMoveOutComplete;
            _gameViewState                  = GameViewState.CLOSE;
            if (OnClose() || isJumpPanel){
                this.RemoveEvent();
                if (isFoce){
                    OnMoveOutComplete(MoveAnimationState.EndMoveOutAndDestroy);
                }
                else{
                    PlayMoveOutAnimation();
                }
            }
            else{
                OnMoveOutComplete(MoveAnimationState.EndMoveOut);
            }
        }

        /// <summary>
        /// close
        /// </summary>
        protected virtual bool OnClose(){ return true; }

        /// <summary>
        /// Plaies the move in animation.
        /// </summary>
        protected virtual void PlayMoveInAnimation(){ OnMoveInComplete(); }

        /// <summary>
        /// Raises the move in complete event.
        /// </summary>
        protected virtual void OnMoveInComplete(){
            IsActive = true;
            if (_OnMoveInCompleteCallBack != null){
                _OnMoveInCompleteCallBack(MoveAnimationState.EndMoveIn);
                _OnMoveInCompleteCallBack = null;
            }

            _isShowComplete = true;
        }

        /// <summary>
        /// Plaies the move out animation.
        /// </summary>
        protected virtual void PlayMoveOutAnimation(){ OnMoveOutComplete(MoveAnimationState.EndMoveOutAndDestroy); }

        /// <summary>
        /// Raises the move out complete event.
        /// </summary>
        /// <param name="value">Value.</param>
        protected virtual void OnMoveOutComplete(MoveAnimationState value){
            if (value == MoveAnimationState.EndMoveOutAndDestroy){
                Destroy(gameObject);
                IsActive = false;
            }

            //		foreach (GameView item in childViewStack) {
            //			item.gameObject.SetActive (false);
            //		}
            if (_OnMoveOutCompleteCallBack != null){
                _OnMoveOutCompleteCallBack(value);
                _OnMoveOutCompleteCallBack = null;
            }
        }

        private void Awake(){
            if (!Application.isPlaying){
                return;
            }

            _firstInitializeFlg = true;
            //		childViewStack = new Stack<GameView> ();
            OnAwake();
        }

        protected virtual void OnAwake(){ this.gameObject.SetActive(false); }
        private           void Start()  { this.OnStart(); }
        protected virtual void OnStart(){ }

        private void Update(){
            if (!_isShowComplete) return;
            OnUpdate();
        }

        protected virtual void OnUpdate(){ }

        /// <summary>
        /// Backs the key.
        /// </summary>
        public void BackKey(){
            if (!this.EscFlg || !_isShowComplete){
                return;
            }

            OnBackKey();
        }

        /// <summary>
        /// Raises the back key event.
        /// </summary>
        protected virtual void OnBackKey(){ }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        protected override void OnDestroy(){
            base.OnDestroy();
            _firstInitializeFlg = true;
        }

#if UNITY_EDITOR
        void Reset()     { UpdatePrefabPaths(); }
        void OnValidate(){ UpdatePrefabPaths(); }

        private void UpdatePrefabPaths(){
            if (Application.isPlaying || EditorApplication.isCompiling){
                return;
            }

            string path = AssetDatabase.GetAssetPath(this.GetInstanceID());
            // string[] strs = path.Split (new string[]{ "/Resources/" }, new StringSplitOptions ());
            string[] strs = path.Split(new string[]{ $"{AssetSystemConfig.Instance.ABDIR_URL}/" }, new StringSplitOptions());
            if (strs.Length < 2){
                return;
            }

            UpdatePrefabPath(this.GetType().Name, strs[1]);
        }

        private void UpdatePrefabPath(string className, string path){
            var cnf = UIConfig.Instance;
            if (cnf == null){
                return;
            }

            string          classPath       = FileManager.PathFormat(cnf.PrefabPathScriptFullPath, FileManager.DirectoryType.ASSETS);
            string          data            = FileManager.ReadString(classPath);
            Regex           regex           = new Regex("(?=AddViewPath<" + className + ">).*?(\\);)");
            MatchCollection matchCollection = regex.Matches(data);
            if (matchCollection.Count > 0){
                //AddViewPath<PopupScrollText>("Prefabs/PopupUI/PopupScrollText.prefab");
                string rep = "AddViewPath<" + className + ">(\"" + path + "\");";
                if (matchCollection[0].Value.Contains("true")){
                    rep = "AddViewPath<" + className + ">(\"" + path + "\",true);";
                }

                data = regex.Replace(data, rep);
            }
            else{
                //			data = data.Replace ("}", "");
                int index     = data.LastIndexOf(';');
                int indexTemp = data.LastIndexOf('{');
                // Make sure the ‘;’ position is behind the ‘{’ position
                if (index < indexTemp){
                    index = data.IndexOf('}') - 1;
                }

                data =  data.Remove(index + 1);
                data += "\n\t\tAddViewPath<" + className + ">(\"" + path + "\");\n\t}\n}";
            }

            //		Debug.LogError(classPath+"\n"+data);
            FileManager.WriteString(classPath, data);
        }

#endif
    }

    public enum GameViewState{
        /// <summary>
        /// The SHOW.
        /// </summary>
        SHOW,

        /// <summary>
        /// The HIDE.
        /// </summary>
        HIDE,

        /// <summary>
        /// The CLOSE.
        /// </summary>
        CLOSE
    }
}