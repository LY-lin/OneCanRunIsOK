using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun
{
    public abstract class Event
    {
        //public abstract void fireEvent();
        public int id;
        public abstract void setID(int _id);
    }

    public class DefaultEvent : Event
    {
        public DefaultEvent(int id)
        {
            setID(id);
        }

        public override void setID(int _id)
        {
            id = _id;
        }
    }

}
