using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    public class DisplaceAction
    {
        public GameObject displaceObject;
        public Vector3 destination;
        public float endTime;
        public float displaceSpeed;

        public DisplaceAction(GameObject _displaceObject, Vector3 _destination, float _startTime, float _existTime, float _displaceSpeed)
        {
            displaceObject = _displaceObject;
            destination = _destination;
            endTime = _startTime + _existTime;
            displaceSpeed = _displaceSpeed;
        }
    }
}
