using System;
using IG.Runtime.Utils;
using UnityEngine;

namespace IG.Module.UI{
    public class PanelController : PanelControllerBase<PanelController>{
        /// <summary>
        /// Removes the game panel.
        /// </summary>
        /// <param name="gamePanel">Game panel.</param>
        public static void RemoveGamePanel(GamePanel gamePanel){ GameObjUtils.DestroyObj(gamePanel.gameObject); }

        /// <summary>
        /// Shows the panel.
        /// </summary>
        /// <param name="isForceShowCurrentPanel">If set to <c>true</c> is force show current panel.</param>
        /// <param name="isForceCloseLastPanel">If set to <c>true</c> is force close last panel.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void ShowPanel<T>(bool isForceShowCurrentPanel, bool isForceCloseLastPanel) where T : GamePanel{
            PanelParam panelParam = new PanelParam(typeof(T));
            ShowPanel(panelParam);
        }

        /// <summary>
        /// Shows the panel.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void ShowPanel<T>(Action<GamePanel> action = null) where T : GamePanel{
            PanelParam panelParam = new PanelParam(typeof(T));
            panelParam.IsForceShowCurrentPanel = false;
            panelParam.IsForceCloseLastPanel   = false;
            panelParam.ShowCallBack            = action;
            ShowPanel(panelParam);
        }

        /// <summary>
        /// Shows the panel.
        /// </summary>
        /// <param name="t">T.</param>
        public static void ShowPanel(Type t, Action<GamePanel> action = null){
            PanelParam panelParam = new PanelParam(t);
            panelParam.IsForceShowCurrentPanel = false;
            panelParam.IsForceCloseLastPanel   = false;
            panelParam.ShowCallBack            = action;
            ShowPanel(panelParam);
        }

        /// <summary>
        /// Shows the panel.
        /// </summary>
        /// <param name="t">T.</param>
        /// <param name="isForceShowCurrentPanel">If set to <c>true</c> is force show current panel.</param>
        /// <param name="isForceCloseLastPanel">If set to <c>true</c> is force close last panel.</param>
        public static void ShowPanel(PanelParam panelParam){ Instance.PShowPanel(panelParam); }

        /// <summary>
        /// Closes the panel.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void ClosePanel<T>() where T : GamePanel{
            Type t = typeof(T).BaseType;
            Instance.PClosePanel(t);
        }

        /// <summary>
        /// Closes the panel.
        /// </summary>
        /// <param name="t">T.</param>
        public static void ClosePanel(Type t){ Instance.PClosePanel(t); }

        /// <summary>
        /// close top Popup
        /// </summary>
        public static void CloseTopPopupPanel(){ Instance.ClosePopupPanel(); }

        /// <summary>
        /// close top Page
        /// </summary>
        public static void CloseTopPagePanel(){ Instance.CloseContentPanel(); }

        /// <summary>
        /// close top system popup
        /// </summary>
        public static void CloseTopSytemPopupPanel(){ Instance.CloseSytemPanel(); }

        /// <summary>
        /// close top Tutorial
        /// </summary>
        public static void CloseTopTutorialPanel(){ Instance.CloseTutorialPanel(); }

        /// <summary>
        /// close top Dialog
        /// </summary>
        public static void CloseTopDialogPanel(){ Instance.CloseDialogPanel(); }

        /// <summary>
        /// close top Dialog
        /// </summary>
        public static void BackKey(){ Instance.PBackKey(); }

        public static PagePanel GetLastPagePanel(){ return Instance.PGetLastPagePanel(); }

        protected override void GetPanel(PanelParam panelParam, Action<GamePanel> callBack){
            base.GetPanel(
                          panelParam,
                          c => {
                              callBack?.Invoke(c);
                              panelParam.ShowCallBack?.Invoke(c);
                          }
                         );
        }
    }

    public class PanelParam{
        public PanelParam(Type panelType){ PType = panelType; }
        public Type              PType{ get; private set; }
        public bool              IsForceShowCurrentPanel;
        public bool              IsForceCloseLastPanel;
        public Transform         Parent;
        public bool              IsAsync = true;
        public Action<GamePanel> ShowCallBack;
    }
}