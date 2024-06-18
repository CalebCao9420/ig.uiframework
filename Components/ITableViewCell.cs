using UnityEngine;

namespace IG.Module.UI{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ITableViewCell : MonoBehaviour{
        /// <summary>
        /// 当Item刷新数据重新展示的时候调用
        /// </summary>
        public virtual void OnInShow(object data){ }

        /// <summary>
        /// 如果非无限循环，当Item要设置成active(false)的时候调用
        /// </summary>
        public virtual void OnWillHidden(){ }
    }
}