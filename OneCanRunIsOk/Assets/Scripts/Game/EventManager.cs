using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun
{
    public static class EventManager{

        private static Dictionary<Type, Action<OneCanRun.Event>> mEventMap = new Dictionary<Type, Action<OneCanRun.Event>>();
        private static Dictionary<Delegate, Action<OneCanRun.Event>> mEventLookups = new Dictionary<Delegate, Action<OneCanRun.Event>>();

        public static void addListener<T>(Action<T> evt) where T : OneCanRun.Event{
            if (!mEventLookups.ContainsKey(evt))
            {

                Action<OneCanRun.Event> newAction = (e) => evt((T)e);
                mEventLookups[evt] = newAction;

                if (mEventMap.TryGetValue(typeof(T), out Action<OneCanRun.Event> internalAction))
                {
                    mEventMap[typeof(T)] = internalAction += newAction;
                }
                else
                    mEventMap[typeof(T)] = newAction;

            }

        }

        public static void removeListener<T>(Action<T> evt) where T : OneCanRun.Event{
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
