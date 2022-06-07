using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class ActorBufferManager : MonoBehaviour
    {
        private List<BuffController> NumBuffList;
        private List<BuffController> PercentBuffList;

        private List<BuffController> WeaponBuffList;

        public int timeSpend;
        
        public void buffGain(BuffController newBuff)
        {
            if(newBuff.getBuffType() == Buff.BufferType.NumBuff)
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
            {
            
            }
        }
        private void checkActive()
        {

        }

        public void buffLose()
        {
            foreach(BuffController m in NumBuffList)
            {
                checkActive();
            }
            
        }


    }
}
