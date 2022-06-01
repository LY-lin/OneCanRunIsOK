using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game
{
    public abstract class AbstractEventFactory
    {

        public abstract Event createEvent(string eventName);

    }

    public class DefaultEventFactory : AbstractEventFactory
    {

        private Dictionary<int, Event> container;

        public override Event createEvent(string eventName)
        {
            if (eventName == "DefaultEvent")
            {
                return new DefaultEvent();

            }
            return null;
        }
    }
}
