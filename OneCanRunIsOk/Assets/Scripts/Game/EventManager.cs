using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game
{
    public static class EventManager{

        private static Dictionary<Type, Action<Event>> mEventMap = new Dictionary<Type, Action<Event>>();
        private static Dictionary<Delegate, Action<Event>> mEventLookups = new Dictionary<Delegate, Action<Event>>();

        public static void addListener<T>(Action<T> evt) where T : Event{
            if (!mEventLookups.ContainsKey(evt))
            {

                Action<Event> newAction = (e) => evt((T)e);
                mEventLookups[evt] = newAction;

                if (mEventMap.TryGetValue(typeof(T), out Action<Event> internalAction))
                {
                    mEventMap[typeof(T)] = internalAction += newAction;
                }
                else
                    mEventMap[typeof(T)] = newAction;

            }

        }

        public static void removeListener<T>(Action<T> evt) where T : Event{
            if(mEventLookups.TryGetValue(evt, out var action)){
                if(mEventMap.TryGetValue(typeof(T), out var tempAction)){
                    tempAction -= action;
                    if (tempAction == null)
                        mEventMap.Remove(typeof(T));
                    else
                        mEventMap[typeof(T)] = tempAction;

                }
                mEventLookups.Remove(evt);
            }

        }

        public static void broadcast(Event evt){
            if (mEventMap.TryGetValue(evt.GetType(), out var action))
                action.Invoke(evt);

        }

        public static void clear(){
            mEventMap.Clear();
            mEventLookups.Clear();

        }
    }
}
