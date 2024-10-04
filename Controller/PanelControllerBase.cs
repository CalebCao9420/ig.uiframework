using System;
using System.Collections;
using System.Collections.Generic;
using IG.AssetBundle;
using IG.Module.UI.Res;
using IG.Runtime.Extensions;
using IG.Runtime.Log;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IG.Module.UI{
    public class PanelControllerBase<T> : SingletonMono<T> where T : MonoBehaviour, ISingleton{
        public Transform Canvas;

        [HideInInspector]
        public Camera UICamera;

        private GameObject       _eventSystem;
        private Stack<PagePanel> _openPagePanelList = null;
        private List<PagePanel>  _hidePagePanelList = null;
        private DialogPanel      _dialogGamePanel   = null;
        private HeaderPanel      _headerPanel;
        private FooterPanel      _footerPanel;
        private GameBGPanel      _gameBGPanel;
        private PopupPanel       _popupPanel;
        private SystemPanel      _systemPanel;
        private TutorialPanel    _tutorialPanel;
        private GameObject       _backKeyController;

        /// <summary>
        /// Current Panel type
        /// </summary>
        private PagePanel _currentContentPanel;

        /// <summary>
        /// Panel closes waiting for the destruction queue
        /// </summary>
        private Dictionary<string, Spawn> _spawnDestroyDic = null;

        public RectTransform UIPanelRootContainer;

        /// <summary>
        /// Game background container
        /// </summary>
        private RectTransform _gameBGContainer;

        /// <summary>
        /// Game page container
        /// </summary>
        private RectTransform _contentPanelContainer;

        /// <summary>
        /// Game header container
        /// </summary>
        private RectTransform _headerPanelContainer;

        /// <summary>
        /// Game footer container
        /// </summary>
        private RectTransform _footerPanelContainer;

        /// <summary>
        /// Game popup view container
        /// </summary>
        private RectTransform _dialogPanelContainer;

        /// <summary>
        /// Game popup container
        /// </summary>
        private RectTransform _popupPanelContainer;

        /// <summary>
        /// System front interface container
        /// </summary>
        private RectTransform _systemPanelContainer;

        /// <summary>
        /// Novice interface
        /// </summary>
        private RectTransform _tutorialPanelContainer;

        /// <summary>
        /// Dialog box mask
        /// </summary>
        private Image _dialogPanelContainerMask;

        /// <summary>
        /// Pop-up mask
        /// </summary>
        private Image _popupPanelContainerMask;

        /// <summary>
        /// System Pop-up mask
        /// </summary>
        private Image _systemPanelContainerMask;

        /// <summary>
        /// The short loading mask.
        /// </summary>
        private RectTransform _shortLoadingMask;

        private RectTransform _uiMask = null;
        private RectTransform _deBugBtnRT;
        private Button        _deBugBtn;

        //private DebugPanel debugPanel;
        private         RectTransform _debugPanelContainer;
        public override void          OnDispose(){ }

        public override void Init(){
            _eventSystem       = EventSystem.current.gameObject;
            _openPagePanelList = new Stack<PagePanel>();
            _hidePagePanelList = new List<PagePanel>();
            _spawnDestroyDic   = new Dictionary<string, Spawn>();
            UICamera           = Canvas.GetComponent<Canvas>().worldCamera;
            DontDestroyOnLoad(Canvas.gameObject);
            DontDestroyOnLoad(UICamera.gameObject);
            DontDestroyOnLoad(this.gameObject);
            UIPanelRootContainer = CreateBaseObj("UIPanelRootContainer", Canvas, true);
            UIPanelRootContainer.SetSiblingIndex(0);
            _backKeyController = CreateBaseObj("backKeyController", UIPanelRootContainer).gameObject;
            _backKeyController.AddComponent<BackKeyController>();
            _gameBGContainer = CreateBaseObj("GameBGContainer", UIPanelRootContainer, false);
            //Image image = gameBGContainer.gameObject.AddComponent<Image> ();
            //image.color = Color.black;
            //image.raycastTarget = false;
            _contentPanelContainer          = CreateBaseObj("ContentPanelContainer",    UIPanelRootContainer);
            _headerPanelContainer           = CreateBaseObj("HeaderPanelContainer",     UIPanelRootContainer);
            _footerPanelContainer           = CreateBaseObj("FooterPanelContainer",     UIPanelRootContainer);
            _dialogPanelContainer           = CreateBaseObj("DialogPanelContainer",     UIPanelRootContainer);
            _dialogPanelContainerMask       = CreateBaseObj("dialogPanelContainerMask", _dialogPanelContainer).gameObject.AddComponent<Image>();
            _dialogPanelContainerMask.color = new Color(0, 0, 0, 0f);
            _dialogPanelContainerMask.gameObject.SetActive(false);
            _popupPanelContainer             = CreateBaseObj("PopupPanelContainer", UIPanelRootContainer, false, true);
            _popupPanelContainerMask         = _popupPanelContainer.gameObject.AddComponent<Image>();
            _popupPanelContainerMask.color   = new Color(0, 0, 0, 0f);
            _popupPanelContainerMask.enabled = false;
            _tutorialPanelContainer          = CreateBaseObj("TutorialPanelContainer", UIPanelRootContainer, false, true);
            _shortLoadingMask                = CreateBaseObj("ShortLoadingMask",       UIPanelRootContainer, false);
            _shortLoadingMask.GetOrAddComponent<ShortLoadingMaskController>();
            _uiMask                                        = CreateBaseObj("UIMask", UIPanelRootContainer, false, true);
            _uiMask.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 1f);
            HideUIMask();
            _systemPanelContainer             = CreateBaseObj("SystemPanelContainer", UIPanelRootContainer, false, true);
            _systemPanelContainerMask         = _systemPanelContainer.gameObject.AddComponent<Image>();
            _systemPanelContainerMask.color   = new Color(0, 0, 0, 150f / 255f);
            _systemPanelContainerMask.enabled = false;
#if !PRO
            //		debugPanelContainer = CreateBaseObj ("DebugPanelContainer", uiPanelRootContainer, true, true);
            //
            //		deBugBtnRT = CreateBaseObj ("DeBugBtn", uiPanelRootContainer);
            //		deBugBtnRT.gameObject.AddComponent<Image> ().color = new Color (255f, 0f, 0f, 0.01f);
            //		deBugBtn = deBugBtnRT.gameObject.AddComponent<Button> ();
            //		deBugBtn.onClick.AddListener (OnOpenDebugPanel);
            //		deBugBtnRT.offsetMin = new Vector2 (0f, 400f);
            //		deBugBtnRT.offsetMax = new Vector2 (-1850f, -400f);
#endif
        }

        private void OnOpenDebugPanel(){
            if (_debugPanelContainer == null){
                return;
            }

            //if (debugPanel != null)
            //{
            //    debugPanel.gameObject.SetActive(!debugPanel.gameObject.activeSelf);
            //}
            //else
            //{
            //    DebugPanel tempDebug = Resources.Load<DebugPanel>("Prefabs/DebugUI/DebugPanel");
            //    debugPanel = GameObject.Instantiate(tempDebug, debugPanelContainer);
            //    debugPanel.RectTransform().anchoredPosition = Vector2.zero;
            //    debugPanel.RectTransform().localScale = Vector3.one;
            //    debugPanel.gameObject.SetActive(true);
            //}
        }

        private void ShowUIMask(){
            _uiMask.gameObject.SetActive(true);
            //EngineEventManager.DispatchEvent(new EngineEvent(EngineEventType.SHOW_WAIT_UI));
        }

        private void HideUIMask(){ _uiMask.gameObject.SetActive(false); }
        private void Update()    { CheckSpawnDestroy(); }

        /// <summary>
        /// Creates the base object.
        /// </summary>
        /// <returns>The base object.</returns>
        /// <param name="objName">Object name.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="anchorCenter">If set to <c>true</c> anchor center.</param>
        private RectTransform CreateBaseObj(string objName, Transform parent, bool anchorCenter = false, bool isModelUp = false){
            GameObject obj = new GameObject(objName);
            obj.layer            = parent.gameObject.layer;
            obj.transform.parent = parent;
            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            if (anchorCenter){
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = Canvas.GetComponent<GameCanvasScaler>().referenceResolution;
            }
            else{
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.sizeDelta = Vector2.zero;
            }

            if (isModelUp){
                rectTransform.localPosition = new Vector3(0f, 0f, -1500f);
            }
            else{
                rectTransform.localPosition = Vector2.zero;
            }

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale       = Vector3.one;
            return rectTransform;
        }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="callBack">Call back.</param>
        protected virtual void GetPanel(PanelParam panelParam, Action<GamePanel> callBack){
            GamePanel t = null;
            if (panelParam.PType.BaseType == typeof(PagePanel) || panelParam.PType.BaseType == typeof(RootPagePanel)){
                for (int i = _hidePagePanelList.Count - 1; i >= 0; i--){
                    if (_hidePagePanelList[i].GetType() == panelParam.PType){
                        t = _hidePagePanelList[i];
                        //If the last page
                        if (_hidePagePanelList.Count - 1 == i){
                            t.IsBackShow = true;
                        }
                        else{
                            t.IsBackShow = false;
                        }

                        _hidePagePanelList.RemoveAt(i);
                        break;
                    }
                }

                if (t != null){
                    SetParent(t.gameObject, panelParam.Parent);
                    callBack(t);
                    return;
                }
            }

            t = GetPanelInSpawnDestroy(panelParam.PType);
            if (t != null){
                t.IsBackShow = false;
                SetParent(t.gameObject, panelParam.Parent);
                callBack(t);
                return;
            }

            // panelParam.checkPanelAsset.CheckAssetAndDownload(
            //                                                  () => {
            //                                                      PrefabInfo prefabInfo = GamePrefabPaths.GetPath(panelParam.type);
            //                                                      if (!panelParam.isAsync){
            //                                                          GamePanel gamePanel = GameAssetManager.InstantiatePrefab<GamePanel>(prefabInfo.path, panelParam.parent, prefabInfo.isForceLocal);
            //                                                          callBack(gamePanel);
            //                                                          return;
            //                                                      }
            //
            //                                                      GameAssetManager.InstantiatePrefabAsync<GamePanel>(prefabInfo.path, panelParam.parent, callBack, prefabInfo.isForceLocal);
            //                                                  }
            //                                                 );
            PrefabInfo prefabInfo = GamePrefabPaths.GetPath(panelParam.PType);
            if (panelParam.IsAsync){
                AssetsSystem.LoadAsync(
                                      (o, o1) => {
                                          GameObject obj = Instantiate(o as GameObject);
                                          obj.name = (o as GameObject).name;
                                          GamePanel gamePanel = obj.GetComponent<GamePanel>();
                                          SetParent(gamePanel.gameObject, panelParam.Parent);
                                          callBack(gamePanel);
                                      },
                                      prefabInfo.path
                                     );
            }
            else{
                GameObject prefab = AssetsSystem.Load<GameObject>(prefabInfo.path);
                GameObject obj    = Instantiate(prefab);
                obj.name = prefab.name;
                GamePanel gamePanel = obj.GetComponent<GamePanel>();
                SetParent(gamePanel.gameObject, panelParam.Parent);
                callBack(gamePanel);
            }
        }

        protected static void SetParent(GameObject obj, Transform parent){
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale    = Vector3.one;
        }

        /// <summary>
        /// Ps the show panel.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="isForceShowCurrentPanel">If set to <c>true</c> is force show current panel.</param>
        /// <param name="isForceCloseLastPanel">If set to <c>true</c> is force close last panel.</param>
        protected void PShowPanel(PanelParam panelParam){
            Type baseType = panelParam.PType.BaseType;
            if (baseType == typeof(FooterPanel)){
                this.ShowFooterPanel(panelParam);
            }
            else if (baseType == typeof(HeaderPanel)){
                this.ShowHeaderPanel(panelParam);
            }
            else if (baseType == typeof(GameBGPanel)){
                this.ShowBGPanel(panelParam);
            }
            else if (baseType == typeof(PagePanel) || baseType == typeof(RootPagePanel)){
                this.ShowContentPanel(panelParam);
            }
            else if (baseType == typeof(DialogPanel)){
                this.ShowDialogGamePanel(panelParam);
            }
            else if (baseType == typeof(PopupPanel)){
                this.ShowPopupGamePanel(panelParam);
            }
            else if (baseType == typeof(SystemPanel)){
                this.ShowSystemPanel(panelParam);
            }
            else if (baseType == typeof(TutorialPanel)){
                this.ShowTutorialPanel(panelParam);
            }
            else{
                throw new Exception("Please register " + panelParam.PType.ToString());
            }
        }

    #region Shows the footer panel.

        /// <summary>
        /// Shows the footer panel.
        /// </summary>
        /// <param name="type">Type.</param>
        private void ShowFooterPanel(PanelParam panelParam){
            panelParam.Parent = _footerPanelContainer;
            if (_footerPanel != null){
                _footerPanel.Close(
                                   false,
                                   (MoveAnimationState value) => {
                                       GetPanel(
                                                panelParam,
                                                c => {
                                                    _footerPanel = c as FooterPanel;
                                                    _footerPanel.Show(true, OnFooterMoveInComplete);
                                                }
                                               );
                                   }
                                  );
                _footerPanel = null;
            }
            else{
                GetPanel(
                         panelParam,
                         c => {
                             _footerPanel = c as FooterPanel;
                             _footerPanel.Show(true, OnFooterMoveInComplete);
                         }
                        );
            }
        }

        private void OnFooterMoveInComplete(MoveAnimationState value){ }

    #endregion

    #region Shows the header panel.

        /// <summary>
        /// Shows the header panel.
        /// </summary>
        /// <param name="type">Type.</param>
        private void ShowHeaderPanel(PanelParam panelParam){
            panelParam.Parent = _headerPanelContainer;
            if (_headerPanel != null){
                _headerPanel.Close(
                                   false,
                                   (MoveAnimationState value) => {
                                       GetPanel(
                                                panelParam,
                                                c => {
                                                    _headerPanel = c as HeaderPanel;
                                                    _headerPanel.Show(true, OnHeaderMoveInComplete);
                                                }
                                               );
                                   }
                                  );
                _headerPanel = null;
            }
            else{
                GetPanel(
                         panelParam,
                         c => {
                             _headerPanel = c as HeaderPanel;
                             _headerPanel.Show(true, OnHeaderMoveInComplete);
                         }
                        );
            }
        }

        private void OnHeaderMoveInComplete(MoveAnimationState value){ }

    #endregion

    #region Shows the GB panel.

        /// <summary>
        /// Shows the GB panel.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="isForceShowCurrentPanel">If set to <c>true</c> is force show current panel.</param>
        /// <param name="isForceCloseLastPanel">If set to <c>true</c> is force close last panel.</param>
        private void ShowBGPanel(PanelParam panelParam){
            panelParam.Parent = _gameBGContainer;
            if (_gameBGPanel != null){
                _gameBGPanel.Close(
                                   panelParam.IsForceCloseLastPanel,
                                   (MoveAnimationState value) => {
                                       GetPanel(
                                                panelParam,
                                                c => {
                                                    _gameBGPanel = c as GameBGPanel;
                                                    _gameBGPanel.Show(panelParam.IsForceShowCurrentPanel, OnGameBGMoveInComplete);
                                                }
                                               );
                                   }
                                  );
                _gameBGPanel = null;
            }
            else{
                GetPanel(
                         panelParam,
                         c => {
                             _gameBGPanel = c as GameBGPanel;
                             _gameBGPanel.Show(panelParam.IsForceShowCurrentPanel, OnGameBGMoveInComplete);
                         }
                        );
            }
        }

        private void OnGameBGMoveInComplete(MoveAnimationState value){ }

    #endregion

    #region Shows the popup game panel.

        /// <summary>
        /// Shows the popup game panel.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="isForceShowCurrentPanel">If set to <c>true</c> is force show current panel.</param>
        /// <param name="isForceCloseLastPanel">If set to <c>true</c> is force close last panel.</param>
        private void ShowPopupGamePanel(PanelParam panelParam){
            panelParam.Parent = _popupPanelContainer;
            if (_popupPanel != null){
                _popupPanel.Close(
                                  panelParam.IsForceCloseLastPanel,
                                  (MoveAnimationState value) => {
                                      AddSpawnDestroyDic(_popupPanel);
                                      GetPanel(
                                               panelParam,
                                               c => {
                                                   _popupPanel = c as PopupPanel;
                                                   _popupPanel.Show(panelParam.IsForceShowCurrentPanel, OnPopupMoveInComplete);
                                               }
                                              );
                                  }
                                 );
            }
            else{
                GetPanel(
                         panelParam,
                         c => {
                             _popupPanel = c as PopupPanel;
                             _popupPanel.Show(panelParam.IsForceShowCurrentPanel, OnPopupMoveInComplete);
                         }
                        );
            }

            _popupPanelContainerMask.enabled = true;
        }

        private void OnPopupMoveInComplete(MoveAnimationState value){ }

    #endregion

    #region Shows the content panel.

        /// <summary>
        /// Shows the content panel.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="isForceShowCurrentPanel">If set to <c>true</c> is force show current panel.</param>
        /// <param name="isForceCloseLastPanel">If set to <c>true</c> is force close last panel.</param>
        private void ShowContentPanel(PanelParam panelParam){
            if (_openPagePanelList.Count > 0){
                //if it is the same view
                if (_openPagePanelList.Peek().GetType() == panelParam.PType){
                    return;
                }

                ShowUIMask();
                PagePanel lastGamePanel = _openPagePanelList.Pop();
                //			Type lastGamePanelType = lastGamePanel.GetType ();
                Action<MoveAnimationState> onMoveOutComplete = (MoveAnimationState value) => {
                    //				AddSpawnDestroyDic (lastGamePanel);
                    _hidePagePanelList.Add(lastGamePanel);
                    GetContentPanel(
                                    panelParam,
                                    c => {
                                        if (c == null){
                                            this.Log("ShowContentPanel t is null.type:" + panelParam.PType, LogType.Error);
                                        }

                                        //					c.lastGamePanelType = lastGamePanelType;
                                        c.Show(panelParam.IsForceShowCurrentPanel, OnContentMoveInComplete);
                                    }
                                   );
                };
                lastGamePanel.Close(panelParam.IsForceCloseLastPanel, onMoveOutComplete, true);
                HideDialogOfPage(lastGamePanel);
            }
            else{
                ShowUIMask();
                GetContentPanel(
                                panelParam,
                                c => {
                                    c.Show(panelParam.IsForceShowCurrentPanel, OnContentMoveInComplete);
                                    ShowDialogOfPage(c);
                                }
                               );
            }
        }

        /// <summary>
        /// Ps the get last normal panel.
        /// </summary>
        /// <returns>The get last normal panel.</returns>
        protected PagePanel PGetLastPagePanel(){
            if (_openPagePanelList.Count > 0){
                return _openPagePanelList.Peek();
            }

            return null;
        }

        /// <summary>
        /// Gets the content panel.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="callBack">Call back.</param>
        private void GetContentPanel(PanelParam panelParam, Action<PagePanel> callBack){
            panelParam.Parent = _contentPanelContainer;
            GetPanel(
                     panelParam,
                     c => {
                         _currentContentPanel = c as PagePanel;
                         if (_currentContentPanel.DlgPresent == null){
                             _currentContentPanel.DlgPresent = CreateBaseObj("DialogPanelContainer", _currentContentPanel.transform);
                             _currentContentPanel.DlgPresent.gameObject.SetActive(false);
                         }

                         if (panelParam.PType.BaseType == typeof(RootPagePanel)){
                             while (_hidePagePanelList.Count > 0){
                                 PagePanel page = _hidePagePanelList[0];
                                 _hidePagePanelList.RemoveAt(0);
                                 if (page.GetType().BaseType != typeof(RootPagePanel)){
                                     AddSpawnDestroyDic(page);
                                 }
                             }
                         }

                         _openPagePanelList.Push(_currentContentPanel);
                         callBack(_currentContentPanel);
                     }
                    );
        }

        /// <summary>
        /// Show completion callback.
        /// </summary>
        private void OnContentMoveInComplete(MoveAnimationState value){
            //Removed short loading when starting to enter the animation
            if (value == MoveAnimationState.StartMoveIn){
                //EngineEventManager.DispatchEvent(new EngineEvent(EngineEventType.CLOSE_WAIT_UI));
                return;
            }

            HideUIMask();
        }

    #endregion

    #region Shows the dialog game panel.

        private void HideDialogOfPage(PagePanel pagePanel){
            if (pagePanel.DlgPresent.childCount > 0){
                for (int i = 0; i < _dialogPanelContainer.childCount; i++){
                    DialogPanel dialogPanel = _dialogPanelContainer.GetChild(i).GetComponent<DialogPanel>();
                    if (dialogPanel != null){
                        dialogPanel.transform.SetParent(pagePanel.DlgPresent);
                    }
                }
            }

            if (_dialogPanelContainer.childCount == 1){
                _dialogPanelContainerMask.gameObject.SetActive(false);
            }
        }

        private void ShowDialogOfPage(PagePanel pagePanel){
            if (pagePanel.DlgPresent.childCount > 0){
                for (int i = 0; i < pagePanel.DlgPresent.childCount; i++){
                    DialogPanel dialogPanel = _dialogPanelContainer.GetChild(i).GetComponent<DialogPanel>();
                    if (dialogPanel != null){
                        dialogPanel.transform.SetParent(_dialogPanelContainer);
                        dialogPanel.IsBackShow = true;
                        if (dialogPanel.IsActive){
                            dialogPanel.Show(true, null);
                        }
                    }
                }

                _dialogPanelContainerMask.transform.SetSiblingIndex(_dialogPanelContainer.childCount - 1);
                _dialogPanelContainerMask.gameObject.SetActive(true);
            }

            if (_dialogPanelContainer.childCount == 1){
                _dialogPanelContainerMask.gameObject.SetActive(false);
            }
        }

        private void CloseDialogOfPage(PagePanel pagePanel){
            for (int i = 0; i < pagePanel.DlgPresent.childCount; i++){
                DialogPanel dialogPanel = _dialogPanelContainer.GetChild(i).GetComponent<DialogPanel>();
                if (dialogPanel != null){
                    dialogPanel.gameObject.SetActive(false);
                    AddSpawnDestroyDic(dialogPanel);
                }
            }

            if (_dialogPanelContainer.childCount == 1){
                _dialogPanelContainerMask.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Shows the dialog game panel.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="isForceShowCurrentPanel">If set to <c>true</c> is force show current panel.</param>
        /// <param name="isForceCloseLastPanel">If set to <c>true</c> is force close last panel.</param>
        private void ShowDialogGamePanel(PanelParam panelParam){
            GetDialogGamePanel(
                               panelParam,
                               c => {
                                   DialogPanel t = c as DialogPanel;
                                   if (t.IsHideLastDialogPanel){
                                       if (_dialogGamePanel != null){
                                           _dialogGamePanel.Close(
                                                                  panelParam.IsForceCloseLastPanel,
                                                                  (MoveAnimationState value) => {
                                                                      //						AddSpawnDestroyDic (dialogGamePanel);
                                                                      _dialogGamePanel = t;
                                                                      _dialogGamePanel.Show(panelParam.IsForceShowCurrentPanel, OnDialogMoveInComplete);
                                                                  }
                                                                 );
                                       }
                                       else{
                                           _dialogGamePanel = t;
                                           _dialogGamePanel.Show(panelParam.IsForceShowCurrentPanel, OnDialogMoveInComplete);
                                       }
                                   }
                                   else{
                                       _dialogGamePanel = t;
                                       _dialogGamePanel.Show(panelParam.IsForceShowCurrentPanel, OnDialogMoveInComplete);
                                   }
                               }
                              );
        }

        /// <summary>
        /// Show game popup DialogGamePanel
        /// </summary>
        /// <returns>The child game view.</returns>
        /// <param name="isForceShowCurrentPanel">If it is true, it is forced to open the view, without animation; if it is false, it is an animation to open the view.</param>
        /// <typeparam name="T">DialogGamePanel.</typeparam>
        private void GetDialogGamePanel(PanelParam panelParam, Action<GamePanel> callback){
            _dialogPanelContainerMask.transform.SetSiblingIndex(_dialogPanelContainer.childCount);
            _dialogPanelContainerMask.gameObject.SetActive(true);
            panelParam.Parent = _dialogPanelContainer;
            GetPanel(
                     panelParam,
                     c => {
                         if (c == null){
                             _dialogPanelContainerMask.gameObject.SetActive(false);
                             throw new Exception(panelParam.PType.ToString() + "The game Dialog GamePanel does not exist.");
                         }

                         c.transform.SetSiblingIndex(_dialogPanelContainer.childCount);
                         callback(c);
                     }
                    );
        }

        /// <summary>
        /// Show completion callback.
        /// </summary>
        /// <param name="value">True: closes successfully, false: closes failed.</param>
        private void OnDialogMoveInComplete(MoveAnimationState value){ }

    #endregion

        /// <summary>
        /// Display pop-up panel
        /// </summary>
        /// <param name="type">Popup type.</param>
        private void ShowSystemPanel(PanelParam panelParam){
            panelParam.Parent = _systemPanelContainer;
            if (_systemPanel != null){
                _systemPanel.Close(
                                   panelParam.IsForceCloseLastPanel,
                                   (MoveAnimationState value) => {
                                       AddSpawnDestroyDic(_systemPanel);
                                       panelParam.IsAsync = false;
                                       GetPanel(
                                                panelParam,
                                                c => {
                                                    _systemPanel = c as SystemPanel;
                                                    _systemPanel.Show(panelParam.IsForceShowCurrentPanel, OnSystemMoveInComplete);
                                                }
                                               );
                                   }
                                  );
                _systemPanel = null;
            }
            else{
                panelParam.IsAsync = false;
                GetPanel(
                         panelParam,
                         c => {
                             _systemPanel = c as SystemPanel;
                             _systemPanel.Show(panelParam.IsForceShowCurrentPanel, OnSystemMoveInComplete);
                         }
                        );
            }

            _systemPanelContainerMask.enabled = true;
            HideUIMask();
        }

        private void OnSystemMoveInComplete(MoveAnimationState s){ }

        private void ShowTutorialPanel(PanelParam panelParam){
            panelParam.Parent = _tutorialPanelContainer;
            if (_tutorialPanel != null){
                _tutorialPanel.Close(
                                     false,
                                     (MoveAnimationState value) => {
                                         GetPanel(
                                                  panelParam,
                                                  c => {
                                                      _tutorialPanel = c as TutorialPanel;
                                                      _tutorialPanel.Show(true, OnTutorialMoveInComplete);
                                                  }
                                                 );
                                     }
                                    );
                _footerPanel = null;
            }
            else{
                GetPanel(
                         panelParam,
                         c => {
                             _tutorialPanel = c as TutorialPanel;
                             _tutorialPanel.Show(true, OnTutorialMoveInComplete);
                         }
                        );
            }
        }

        private void OnTutorialMoveInComplete(MoveAnimationState value)                   { }
        private void CloseGamePanelSendApi(GamePanel             gamePanel, Action action){ gamePanel.CloseAPIRequest(action); }

        /// <summary>
        /// close GamePanel
        /// </summary>
        /// <param name="gamePanel">Game view.</param>
        private void CloseGamePanel(GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel){
            Type baseType = gamePanel.GetType().BaseType;
            if (baseType == typeof(GameBGPanel)){
                CloseBGPanel();
            }
            else if (baseType == typeof(HeaderPanel)){
                CloseHeaderPanel();
            }
            else if (baseType == typeof(FooterPanel)){
                CloseFooterPanel();
            }
            else if (baseType == typeof(PagePanel)){
                CloseContentPanel(gamePanel, isForceCloseCurrentPanel, isForceShowLastPanel);
            }
            else if (baseType == typeof(DialogPanel)){
                CloseDialogGamePanel(gamePanel as DialogPanel, isForceCloseCurrentPanel, isForceShowLastPanel);
            }
            else if (baseType == typeof(PopupPanel)){
                ClosePopupGamePanel(gamePanel, isForceCloseCurrentPanel, isForceShowLastPanel);
            }
            else if (baseType == typeof(SystemPanel)){
                CloseSystemGamePanel(gamePanel, isForceCloseCurrentPanel, isForceShowLastPanel);
            }
            else if (baseType == typeof(TutorialPanel)){
                CloseTutorialGamePanel(gamePanel, isForceCloseCurrentPanel, isForceShowLastPanel);
            }
            else{
                throw new Exception("no find type：" + gamePanel.GetType().ToString());
            }
            //		AddSpawnDestroyDic (gamePanel);
        }

        /// <summary>
        /// close Header
        /// </summary>
        private void CloseHeaderPanel(){
            if (_headerPanel != null){
                _headerPanel.Close(false, null);
                _headerPanel = null;
            }
        }

        /// <summary>
        /// close Footer
        /// </summary>
        private void CloseFooterPanel(){
            if (_footerPanel != null){
                _footerPanel.Close(false, null);
                _footerPanel = null;
            }
        }

        /// <summary>
        /// close PopupPanel
        /// </summary>
        private void ClosePopupGamePanel(GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel){
            if (gamePanel != null){
                Type lastGamePanelType = gamePanel.LastGamePanelType;
                gamePanel.Close(
                                isForceCloseCurrentPanel,
                                (MoveAnimationState value) => {
                                    AddSpawnDestroyDic(gamePanel);
                                    if (lastGamePanelType != null){
                                        PanelParam panelParam = new PanelParam(lastGamePanelType);
                                        panelParam.IsForceCloseLastPanel   = isForceCloseCurrentPanel;
                                        panelParam.IsForceShowCurrentPanel = isForceShowLastPanel;
                                        PShowPanel(panelParam);
                                    }
                                    else{
                                        _popupPanelContainerMask.enabled = false;
                                    }
                                }
                               );
                //			gamePanel = null;
            }
        }

        /// <summary>
        /// close SystemPanel
        /// </summary>
        private void CloseSystemGamePanel(GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel){
            if (gamePanel != null){
                Type lastGamePanelType = gamePanel.LastGamePanelType;
                gamePanel.Close(
                                isForceCloseCurrentPanel,
                                (MoveAnimationState value) => {
                                    AddSpawnDestroyDic(gamePanel);
                                    if (lastGamePanelType != null){
                                        PanelParam panelParam = new PanelParam(lastGamePanelType);
                                        panelParam.IsForceCloseLastPanel   = isForceCloseCurrentPanel;
                                        panelParam.IsForceShowCurrentPanel = isForceShowLastPanel;
                                        PShowPanel(panelParam);
                                    }
                                    else{
                                        _systemPanelContainerMask.enabled = false;
                                    }
                                }
                               );
                //			gamePanel = null;
            }
        }

        /// <summary>
        /// close tutorial
        /// </summary>
        private void CloseTutorialGamePanel(GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel){
            if (_tutorialPanel != null){
                _tutorialPanel.Close(false, null);
                _tutorialPanel = null;
                AddSpawnDestroyDic(gamePanel);
            }
        }

        /// <summary>
        /// Close the sub-root of the game page
        /// </summary>
        /// <param name="gamePanel">Game view.</param>
        /// <param name="isForceCloseCurrentPanel">If true, it is forced to close the view, without animation; if it is false, it is an animation to close the view.</param>
        /// <param name="isForceShowLastPanel">If it is true, it is forced to open the view, without animation; if it is false, it is an animation to open the view.</param>
        private void CloseContentPanel(GamePanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel){
            if (_openPagePanelList.Count > 0){
                if (_openPagePanelList.Peek() != gamePanel){
                    throw new Exception("current:" + gamePanel.name + "Panel And " + _openPagePanelList.Peek().name + "Panel Mismatch.");
                }
            }

            Type lastGamePanelType = gamePanel.LastGamePanelType;
            gamePanel.Close(
                            isForceCloseCurrentPanel,
                            (MoveAnimationState value) => {
                                if (value == MoveAnimationState.EndMoveOutAndDestroy){
                                    _currentContentPanel = null;
                                    CloseDialogOfPage(gamePanel as PagePanel);
                                    AddSpawnDestroyDic(gamePanel);
                                    if (_openPagePanelList.Count > 0){
                                        _openPagePanelList.Pop();
                                    }

                                    //				if (lastGamePanelType != null) {
                                    //					PShowPanel (lastGamePanelType, isForceShowLastPanel, true);
                                    //				}else
                                    //				{
                                    if (lastGamePanelType != null){
                                        PanelParam panelParam = new PanelParam(lastGamePanelType);
                                        panelParam.IsForceCloseLastPanel   = true;
                                        panelParam.IsForceShowCurrentPanel = isForceShowLastPanel;
                                        PShowPanel(panelParam);
                                    }
                                    else{
                                        if (_hidePagePanelList.Count > 0){
                                            PanelParam panelParam = new PanelParam(_hidePagePanelList[^1].GetType());
                                            panelParam.IsForceCloseLastPanel   = true;
                                            panelParam.IsForceShowCurrentPanel = isForceShowLastPanel;
                                            PShowPanel(panelParam);
                                        }
                                    }
                                    //				}
                                }
                            }
                           );
        }

        /// <summary>
        /// Closes the dialog game panel.
        /// </summary>
        /// <param name="gamePanel">Game panel.</param>
        /// <param name="isForceCloseCurrentPanel">If set to <c>true</c> is force close current panel.</param>
        /// <param name="isForceShowLastPanel">If set to <c>true</c> is force show last panel.</param>
        private void CloseDialogGamePanel(DialogPanel gamePanel, bool isForceCloseCurrentPanel, bool isForceShowLastPanel){
            Type lastGamePanelType = gamePanel.LastGamePanelType;
            gamePanel.Close(
                            isForceCloseCurrentPanel,
                            (MoveAnimationState value) => {
                                AddSpawnDestroyDic(gamePanel);
                                if (_dialogPanelContainer.childCount > 3){
                                    _dialogPanelContainerMask.transform.SetSiblingIndex(_dialogPanelContainerMask.transform.GetSiblingIndex() - 1);
                                    this._dialogGamePanel = _dialogPanelContainer.GetChild(_dialogPanelContainer.childCount - 2).GetComponent<DialogPanel>();
                                    if (ExistSpawnDestroyDic(this._dialogGamePanel)){
                                        this._dialogGamePanel = null;
                                        _dialogPanelContainerMask.gameObject.SetActive(false);
                                    }
                                    else{
                                        this._dialogGamePanel.Show(true, null);
                                    }
                                }
                                else{
                                    _dialogPanelContainerMask.gameObject.SetActive(false);
                                }
                            }
                           );
        }

        /// <summary>
        /// Adds the spawn destroy dic.
        /// </summary>
        /// <param name="gamePanel">Game panel.</param>
        private void AddSpawnDestroyDic(GamePanel gamePanel){
            string key = gamePanel.GetType().ToString();
            if (!_spawnDestroyDic.ContainsKey(key)){
                _spawnDestroyDic.Add(key, new Spawn(gamePanel));
            }
        }

        private bool ExistSpawnDestroyDic(GamePanel gamePanel){
            string key = gamePanel.GetType().ToString();
            return _spawnDestroyDic.ContainsKey(key);
        }

        /// <summary>
        /// Gets the panel in spawn destroy.
        /// </summary>
        /// <returns>The panel in spawn destroy.</returns>
        /// <param name="type">Type.</param>
        private GamePanel GetPanelInSpawnDestroy(Type type){
            string    key       = type.ToString();
            GamePanel gamePanel = null;
            if (_spawnDestroyDic.ContainsKey(key)){
                gamePanel = _spawnDestroyDic[key].gamePanel;
                _spawnDestroyDic.Remove(key);
            }

            return gamePanel;
        }

        /// <summary>
        /// Checks the spawn destroy.
        /// </summary>
        private void CheckSpawnDestroy(){
            foreach (var item in _spawnDestroyDic){
                item.Value.delayTime += Time.deltaTime;
                // 达到延迟销毁时间销毁对象
                if (item.Value.isCanDestroy()){
                    _spawnDestroyDic.Remove(item.Key);
                    PanelController.RemoveGamePanel(item.Value.gamePanel);
                    return;
                }
            }
        }

        /// <summary>
        /// Closes the background panel.
        /// </summary>
        private void CloseBGPanel(){
            if (_gameBGPanel != null){
                _gameBGPanel.Close(false, null);
                AddSpawnDestroyDic(_gameBGPanel);
                _gameBGPanel = null;
            }
        }

        /// <summary>
        /// Closes the content panel.
        /// </summary>
        protected void CloseContentPanel(){
            if (_currentContentPanel == null){
                return;
            }

            CloseGamePanelSendApi(
                                  _currentContentPanel,
                                  () => {
                                      if (_currentContentPanel.GetType().BaseType != typeof(RootPagePanel)){
                                          CloseDialogOfPage(_currentContentPanel);
                                          CloseGamePanel(_currentContentPanel, false, false);
                                      }
                                  }
                                 );
        }

        /// <summary>
        /// Closes the dialog panel.
        /// </summary>
        protected void CloseDialogPanel(){
            if (_dialogGamePanel == null){
                return;
            }

            CloseGamePanelSendApi(
                                  _dialogGamePanel,
                                  () => {
                                      DialogPanel temp = _dialogGamePanel;
                                      _dialogGamePanel = null;
                                      CloseGamePanel(temp, false, false);
                                  }
                                 );
        }

        /// <summary>
        /// Ps the close panel.
        /// </summary>
        /// <param name="t">T.</param>
        protected void PClosePanel(Type t){
            if (t == typeof(PagePanel)){
                CloseContentPanel();
            }
            else if (t == typeof(HeaderPanel)){
                CloseHeaderPanel();
            }
            else if (t == typeof(FooterPanel)){
                CloseFooterPanel();
            }
            else if (t == typeof(PopupPanel)){
                ClosePopupPanel();
            }
            else if (t == typeof(SystemPanel)){
                CloseSytemPanel();
            }
            else if (t == typeof(TutorialPanel)){
                CloseTutorialPanel();
            }
            else if (t == typeof(DialogPanel)){
                CloseDialogPanel();
            }
            else if (t == typeof(GameBGPanel)){
                CloseBGPanel();
            }
        }

        /// <summary>
        /// Closes the popup panel.
        /// </summary>
        protected void ClosePopupPanel(){
            if (_popupPanel == null){
                return;
            }

            CloseGamePanelSendApi(
                                  _popupPanel,
                                  () => {
                                      PopupPanel temp = _popupPanel;
                                      _popupPanel = null;
                                      CloseGamePanel(temp, false, false);
                                  }
                                 );
        }

        /// <summary>
        /// Closes the sytem panel.
        /// </summary>
        protected void CloseSytemPanel(){
            if (_systemPanel == null){
                return;
            }

            CloseGamePanelSendApi(
                                  _systemPanel,
                                  () => {
                                      SystemPanel temp = _systemPanel;
                                      _systemPanel = null;
                                      CloseGamePanel(temp, false, false);
                                  }
                                 );
        }

        /// <summary>
        /// Closes the tutorial panel.
        /// </summary>
        protected void CloseTutorialPanel(){
            if (_tutorialPanel == null){
                return;
            }

            CloseGamePanelSendApi(
                                  _tutorialPanel,
                                  () => {
                                      var temp = _tutorialPanel;
                                      _tutorialPanel = null;
                                      CloseGamePanel(temp, false, false);
                                  }
                                 );
        }

        /// <summary>
        /// the back key.
        /// </summary>
        protected void PBackKey(){
            //System panel
            if (_systemPanel != null && _systemPanel.isActiveAndEnabled){
                _systemPanel.BackKey();
                return;
            }

            //popup panel
            if (_popupPanel != null && _popupPanel.isActiveAndEnabled){
                _popupPanel.BackKey();
                return;
            }

            //dialog panel
            if (_dialogGamePanel != null && _dialogGamePanel.isActiveAndEnabled){
                _dialogGamePanel.BackKey();
                return;
            }

            //Ordinary panel
            if (_currentContentPanel != null && _currentContentPanel.isActiveAndEnabled){
                _currentContentPanel.BackKey();
                return;
            }
        }

        /// <summary>
        /// Enabled UICanvas
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        public void EnabledCanvas(bool value){
            this._contentPanelContainer.gameObject.SetActive(value);
            this._footerPanelContainer.gameObject.SetActive(value);
            this._headerPanelContainer.gameObject.SetActive(value);
            this._gameBGContainer.gameObject.SetActive(value);
            this.UICamera.gameObject.SetActive(value);
            Canvas.GetComponent<GameCanvasScaler>().SetHideLeftAndRightMask(!value);
            this.StartCoroutine(ActiveEventSystem(value));
        }

        /// <summary>
        /// Actives the event system.
        /// </summary>
        /// <returns>The event system.</returns>
        /// <param name="value">If set to <c>true</c> value.</param>
        private IEnumerator ActiveEventSystem(bool value){
            yield return new WaitForFixedUpdate();
            //TODO:实际上这么做是不合适的，EventSystem内有Unity内部事件，这么粗暴操作可能造成内存泄露
            this._eventSystem.SetActive(value);
            this.Canvas.gameObject.SetActive(value);
            //TODO:补充SoundController的时候修改
            // SoundController.GetInstance().gameObject.SetActive(value);
        }

        public K FindPanel<K>() where K : PagePanel{
            foreach (PagePanel item in _openPagePanelList){
                if (item is K){
                    return item as K;
                }
            }

            return null;
        }

        /// <summary>
        /// Spawn.
        /// </summary>
        protected class Spawn{
            public Spawn(GamePanel gamePanel){ this.gamePanel = gamePanel; }
            public GamePanel gamePanel{ get; private set; }

            /// <summary>
            /// The delay time.
            /// </summary>
            public float delayTime;

            /// <summary>
            /// Is can destroy.
            /// </summary>
            /// <returns><c>true</c>, if can destroy was ised, <c>false</c> otherwise.</returns>
            public bool isCanDestroy(){ return this.gamePanel.DelayDestroyTime < delayTime; }
        }
    }

    /// <summary>
    /// Move animation state.
    /// </summary>
    public enum MoveAnimationState{
        /// <summary>
        /// The start move in.
        /// </summary>
        StartMoveIn,

        /// <summary>
        /// The end move in.
        /// </summary>
        EndMoveIn,

        /// <summary>
        /// The start move out.
        /// </summary>
        StartMoveOut,

        /// <summary>
        /// The end move out.
        /// </summary>
        EndMoveOut,

        /// <summary>
        /// The end move out and destroy.
        /// </summary>
        EndMoveOutAndDestroy
    }
}