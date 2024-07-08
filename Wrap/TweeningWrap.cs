using System;
using UnityEngine;

namespace IG.Module.Language{
    public static class TweeningWrapStatic{
        private static TweeningWrap s_ins;
        
        public static TweeningWrap DOAnchorPos(this RectTransform target, Vector3 post, float t){
            JudgeExist();
            return s_ins.DOAnchorPos(target,post,t);
        }

        private static void JudgeExist(){
            if (s_ins == null){
                s_ins = new TweeningWrap();
            }
        }

        public class TweeningWrap : WrapBase<ITweening>{
            public Action<Action> OnComplete;
            
            public virtual TweeningWrap DOAnchorPos(RectTransform target, Vector3 post, float t){
                return this;
            }
        }
    }
}