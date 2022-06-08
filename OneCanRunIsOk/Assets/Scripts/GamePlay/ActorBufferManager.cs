using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;
using OneCanRun.Game;

namespace OneCanRun.GamePlay
{
    public class ActorBufferManager : MonoBehaviour
    {
        private List<BuffController> NumBuffList;
        private List<BuffController> PercentBuffList;

        private List<BuffController> WeaponBuffList;

        public float timeSpend=0;

        private void Update()
        {
            timeSpend += Time.deltaTime;
            if (timeSpend % 4 == 0)
                buffLose();
        }

        public void buffGain(BuffController newBuff)
        {
            newBuff.getTime = timeSpend;
            if (newBuff.getBuffType() == Buff.BufferType.NumBuff)
            {
                NumBuffList.Add(newBuff);
            }
            else if(newBuff.getBuffType() == Buff.BufferType.PercentBuff)
            {
                PercentBuffList.Add(newBuff);
            }
            else if(newBuff.getBuffType() == Buff.BufferType.WeaponBuff)
            {
                WeaponBuffList.Add(newBuff);
            }
            else 
            {}
            buffsAct();
        }
        private bool checkActive(BuffController buff)
        {
            if(timeSpend - buff.getTime >= buff.getExistTime())
            {
                return false;
            }
            return true;
        }

        public void buffLose()
        {
            foreach(BuffController m in NumBuffList)
            {
                if (!checkActive(m))
                {
                    NumBuffList.Remove(m);
                }
            }
            foreach (BuffController m in PercentBuffList)
            {
                if (!checkActive(m))
                {
                    PercentBuffList.Remove(m);
                }
            }
            foreach (BuffController m in WeaponBuffList)
            {
                if (!checkActive(m))
                {
                    WeaponBuffList.Remove(m);
                }
            }
        }

        public void buffsAct()
        {
            ActorProperties Properties = GetComponentInParent<ActorProperties>();
            foreach (BuffController m in NumBuffList)
            {
                m.ActorbuffAct(ref Properties);
            }
            foreach (BuffController m in PercentBuffList)
            {
                m.ActorbuffAct(ref Properties);
            }
            foreach (BuffController m in WeaponBuffList)
            {
                m.ActorbuffAct(ref Properties);
            }
        }
    }
}
