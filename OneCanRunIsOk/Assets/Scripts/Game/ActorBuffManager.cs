using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;
using UnityEngine.Events;
namespace OneCanRun.Game
{
    public class ActorBuffManager : MonoBehaviour
    {
        public List<BuffController> NumBuffList;
        public List<BuffController> PercentBuffList;

        public List<BuffController> WeaponBuffList;

        public float timeSpend = 0;

        public UnityAction buffChanged;

        private void Awake()
        {
            
        }
        private void Update()
        {
            timeSpend += Time.deltaTime;
            if (timeSpend % 4 == 0)
                buffLose();
        }

        public void buffGain(BuffController newBuff)
        {
            newBuff.setGetTime (timeSpend);
            if (newBuff.getBuffType() == Buff.BufferType.NumBuff)
            {
                NumBuffList.Add(newBuff);
            }
            else if (newBuff.getBuffType() == Buff.BufferType.PercentBuff)
            {
                PercentBuffList.Add(newBuff);
            }
            else if (newBuff.getBuffType() == Buff.BufferType.WeaponBuff)
            {
                WeaponBuffList.Add(newBuff);
            }
            else { }
            buffChanged?.Invoke();
        }
        private bool checkActive(BuffController buff)
        {
            if(buff.getHasExistTime())
                if (timeSpend - buff.getGetTime() >= buff.getExistTime())
                {
                    return false;
                }
            return true;
        }

        public void buffLose()
        {
            foreach (BuffController m in NumBuffList)
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
            buffChanged?.Invoke();
        }
    }
}
