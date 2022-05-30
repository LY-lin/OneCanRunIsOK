using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event {
    //public abstract void fireEvent();
    public abstract Event(int _id);
    int id;
}

public class DefaultEvent : Event{

}
