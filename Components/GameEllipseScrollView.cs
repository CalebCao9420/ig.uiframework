using IG.Module.UI.Editor;
using UnityEngine;

namespace IG.Module.UI{
    public class GameEllipseScrollView : GamePageScrollView{
        public float MinScale = 1;
        public float MaxScale = 1;

        protected override void LocalItemScale(RectTransform rectTransform){
            base.LocalItemScale(rectTransform);
            float rate = Mathf.Abs(rectTransform.anchoredPosition.x / ((_ItemSize.x + Spacing.x) * MinScale));
            rate                     = Mathf.Clamp(MaxScale - rate, MinScale, MaxScale);
            rectTransform.localScale = Vector3.one * (rate);
        }
    }
}