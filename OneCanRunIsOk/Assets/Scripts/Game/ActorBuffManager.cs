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
            buff.getTime -= Time.deltaTime;
            if (buff.getTime < 0)
            {
                return false;
            }
            return true;
        }

        public void buffLose()
        {
            bool changed = false;
            List<BuffController> listToDelete = new List<BuffController>();
            List<BuffController> listToPercentDelete = new List<BuffController>();
            for (int i = 0; i < NumBuffList.Count;i++)
            {
                if (!checkActive(NumBuffList[i]))
                {
                    changed = true;
                    listToDelete.Add(NumBuffList[i]);
                }
            }
            if (listToDelete.Count > 0)
            {
                for (int i = 0; i < listToDelete.Count; i++)
                {
                    NumBuffList.Remove(listToDelete[i]);
                }
            }
            //检查PercentBuff
            for (int i = 0; i < PercentBuffList.Count; i++)
            {
                if (!checkActive(PercentBuffList[i]))
                {
                    changed = true;
                    listToPercentDelete.Add(PercentBuffList[i]);
                }
            }
            if (listToPercentDelete.Count > 0)
            {
                for (int i = 0; i < listToPercentDelete.Count; i++)
                {
                    PercentBuffList.Remove(listToPercentDelete[i]);
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
