using IG.Events;

namespace IG.Module.UI{
    /// <summary>
    /// Scrolling list item base class
    /// </summary>
    public class GameScrollItem : EventMonoBehaviour{
        protected object _Obj;

        public void SetData(IGameScrollItemData data){
            bool isRepeatRefresh = _Obj == data;
            _Obj = data;
            SetData(isRepeatRefresh);
        }

        protected virtual void SetData(bool isRepeatRefresh){ }
    }

    /// <summary>
    /// Scroll through the list item data source interface.
    /// </summary>
    public interface IGameScrollItemData{
    }
}