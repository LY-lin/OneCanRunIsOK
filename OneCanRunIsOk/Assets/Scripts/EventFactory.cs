using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun
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

            return null;
        }

        public void delete(int index)
        {

        }
    }
}
