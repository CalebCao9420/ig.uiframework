using System.Collections.Generic;
using IG.Events;

namespace IG.Module.UI.Controller{
    public class GameControllerBase<T> : SingletonAbs<T>, IGBC where T : SingletonAbs<T>, new(){
        public  string                                                   GUID{ get; }
        private Dictionary<string, List<GameEventManager.EventCallback>> _eventDic;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControllerBase`1"/> class.
        /// </summary>
        public GameControllerBase(){
            GUID      = System.Guid.NewGuid().ToString();
            _eventDic = new Dictionary<string, List<GameEventManager.EventCallback>>();
            AddEvent();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GameControllerBase(){ }

        public override void OnDispose(){ RemoveEvent(); }

    #region Event relative

        /// <summary>
        /// Add a listen event
        /// </summary>
        protected virtual void AddEvent(){ }

        /// <summary>
        /// Remove listener event
        /// </summary>
        protected void RemoveEvent(){
            if (_eventDic == null){
                return;
            }

            foreach (var item in _eventDic){
                while (item.Value.Count > 0){
                    GameEventManager.RemoveEventListener(item.Key, item.Value[0]);
                    item.Value.RemoveAt(0);
                }
            }

            _eventDic.Clear();
            _eventDic = null;
        }

        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <param name="engineEventType">Engine event type.</param>
        /// <param name="eventCallback">Event callback.</param>
        protected void AddEventListener(string engineEventType, GameEventManager.EventCallback eventCallback){
            if (!_eventDic.ContainsKey(engineEventType)){
                _eventDic.Add(engineEventType, new List<GameEventManager.EventCallback>());
            }

            if (!_eventDic[engineEventType].Contains(eventCallback)){
                _eventDic[engineEventType].Add(eventCallback);
            }

            GameEventManager.AddEventListener(engineEventType, eventCallback);
        }

        /// <summary>
        /// Removes the event.
        /// </summary>
        /// <param name="engineEventType">Engine event type.</param>
        /// <param name="eventCallback">Event callback.</param>
        protected void RemoveEventListener(string engineEventType, GameEventManager.EventCallback eventCallback){
            if (!_eventDic.ContainsKey(engineEventType)){
                return;
            }

            if (!_eventDic[engineEventType].Contains(eventCallback)){
                _eventDic[engineEventType].Remove(eventCallback);
            }

            GameEventManager.RemoveEventListener(engineEventType, eventCallback);
        }

    #endregion
    }
}