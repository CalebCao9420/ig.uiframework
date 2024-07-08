using System;
using System.Collections.Generic;
using IG.Runtime.Log;
using IG.Runtime.Utils;

namespace IG.Module.Language{
    public class WrapBase<T> where T : class{
        protected static T S_Ins;

        protected static void JudgeExist(){
            if (S_Ins == null){
                List<System.Type> implementingTypes = ADFUtils.GetImplementingTypes(typeof(T)) as List<System.Type>;
                if (implementingTypes is not{ Count: > 0 }){
                    LogHelper.Log($"没有找到 {typeof(T)} 继承类,加载错误:{implementingTypes}");
                    return;
                }

                S_Ins = Activator.CreateInstance(implementingTypes[0]) as T;
            }
        }
    }
}