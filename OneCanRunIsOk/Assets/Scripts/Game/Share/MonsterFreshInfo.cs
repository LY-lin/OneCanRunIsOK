using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{

    public class MonsterFreshInfo
    {
        public int time;
        public float position_x;
        public float position_y;
        public float position_z;
        public int typeID;
        public int number;
        public MonsterFreshInfo(int _time, float _position_x, float _position_y, float _position_z,
            int _typeID, int _number)
        {
            time = _time;
            position_x = _position_x;
            position_y = _position_y;
            position_z = _position_z;
            typeID = _typeID;
            number = _number;
        }
    }
}
