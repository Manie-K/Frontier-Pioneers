using System;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace FrontierPioneers.Core.StateMachine
{
    /// <summary>
    /// This predicate listens to an event and evaluates to true when the event is fired.
    /// Currently, only EventHandler and parameterless Action events are supported.
    /// </summary>
    public class EventPredicate : IPredicate, IDisposable
    {
        readonly EventInfo _eventInfo;
        readonly Object _eventObject;
        readonly Delegate _eventMethod;
        bool _flag;
        bool _hasBeenEvaluated;
        
        /// <param name="eventName">
        /// Name of the event to listen to. (Waring: this is case-sensitive!)
        /// </param>
        /// <param name="eventOwnerObject">
        /// Object on which the event is defined.
        /// </param>
        public EventPredicate(string eventName, Object eventOwnerObject)
        {
            _eventObject = eventOwnerObject;
            _eventInfo = _eventObject.GetType().GetEvent(eventName);
            Type eventHandlerType = _eventInfo.EventHandlerType;

            if (eventHandlerType == typeof(Action)) {
                _eventMethod = new Action(SetFlag);
            }else if (eventHandlerType == typeof(EventHandler)){
                _eventMethod = new EventHandler(SetFlag);
            }
            else {
                Debug.LogError("Unsupported event type!");
            }
            
            _eventInfo.AddEventHandler(_eventObject, _eventMethod);
        }
        
        public void Dispose()
        {
            _eventInfo.RemoveEventHandler(_eventObject, _eventMethod);
        }

        void SetFlag()
        {
            _flag = true;
        }
        
        void SetFlag(object sender, EventArgs e)
        {
            if (sender == _eventObject) _flag = true;
        }

        public bool Evaluate()
        {
            bool result = _flag && _hasBeenEvaluated;
            _hasBeenEvaluated = true;
            _flag = false;
            if (result)
            {
                _hasBeenEvaluated = false;
            }
            return result;
        }
    }
}