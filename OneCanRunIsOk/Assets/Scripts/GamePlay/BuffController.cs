using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class BuffController : MonoBehaviour
    {
        
        private Buff mbuff;
        public Buff.BufferType getBuffType()
        {
            return mbuff.type;
        }

        public void ActorbuffAct(ActorProperties properties)
        {

        }

        public void WeaponbuffAct()
        {

        }
    }
}
