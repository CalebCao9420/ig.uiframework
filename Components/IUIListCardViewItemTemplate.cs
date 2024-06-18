using UnityEngine;

namespace IG.Module.UI{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class IUIListCardViewItemTemplate<T> : MonoBehaviour where T : class{
        /// <summary>
        /// 当画面当前Item展示的时候调用
        /// </summary>
        public virtual void SetData(T data){ }

        public virtual void InCenter(bool isInCenter){ }
    }
}