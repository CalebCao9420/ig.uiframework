using System;
using IG.Events;
using UnityEngine;

namespace IG.Module.UI{
    public class PopupItemView : EventMonoBehaviour{
        protected PopupItemData _PopupItemData;
        protected Component     _ParentCom;
        public    Action[]      CallBacks{ get; private set; }
        private   void          Start()  { this.AddEvent(); }

        public void SetPopupItemData(PopupItemData data){
            _PopupItemData = data;
            ShowData();
            CallBacks    = new Action[3];
            CallBacks[0] = OnBtn0;
            CallBacks[1] = OnBtn1;
            CallBacks[2] = OnBtn2;
        }

        public            void SetParentView(Component com){ _ParentCom = com; }
        protected virtual void ShowData()                  { }

        /// <summary>
        /// The index from right to left
        /// </summary>
        protected virtual void OnBtn0(){ }

        /// <summary>
        /// The index from right to left
        /// </summary>
        protected virtual void OnBtn1(){ }

        /// <summary>
        /// The index from right to left
        /// </summary>
        protected virtual void OnBtn2(){ }
    }
}