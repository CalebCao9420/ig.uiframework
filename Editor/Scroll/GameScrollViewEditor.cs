#if UNITY_EDITOR
using UnityEditor;

namespace IG.Module.UI{
    [CustomEditor(typeof(GameScrollView), true)]
    public class GameScrollViewEditor : UnityEditor.Editor{
    }
}
#endif