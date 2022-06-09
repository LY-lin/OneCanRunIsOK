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

        public Actor aim_Actor;
        void Start()
        {
            NumBuffList = new List<BuffController>();
            PercentBuffList = new List<BuffController>();
            WeaponBuffList = new List<BuffController>();
            aim_Actor = GetComponent<Actor>();
        }

        void Update()
        {
            timeSpend += Time.deltaTime;
            buffLose();
        }

        public void buffGain(BuffController newBuff)
        {
            newBuff.getTime = timeSpend;
            Buff BuffContext = newBuff.getMBuff();
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
            if (timeSpend - buff.getTime >= buff.getExistTime())
            {
                return false;
            }
            return true;
        }

        public void buffLose()
        {
            bool changed = false;
            
            if (NumBuffList.Count > 0)
            {
                int numForNumBuffToDelet[NumBuffList.Count]{ -1};
                foreach (BuffController m in NumBuffList)
                {
                    if (!checkActive(m))
                    {
                        changed = true;
                        NumBuffList.Remove(m);
                    }
                }
            }
                
            if (PercentBuffList.Count > 0)
                foreach (BuffController m in PercentBuffList)
                {
                    if (!checkActive(m))
                    {
                        changed = true;
                        PercentBuffList.Remove(m);
                    }
                }
            if (WeaponBuffList.Count > 0)
                foreach (BuffController m in WeaponBuffList)
                {
                    if (!checkActive(m))
                    {
                        changed = true;
                        WeaponBuffList.Remove(m);
                    }
                }
            if(changed)
                buffChanged?.Invoke();
        }

    }
}
