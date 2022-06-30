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
        public float lastTime = 0;

        public UnityAction buffChanged;
        //用于通知UI发生了BUFF的获取和损失
        public UnityAction<BuffController> buffGained;
        public UnityAction<BuffController> buffLost;
        public Actor aim_Actor;
        void Awake()
        {
            NumBuffList = new List<BuffController>();
            PercentBuffList = new List<BuffController>();
            WeaponBuffList = new List<BuffController>();
            aim_Actor = GetComponent<Actor>();
        }

        void Update()
        {
            timeSpend += Time.deltaTime;
            buffLose(timeSpend - lastTime);
            lastTime = timeSpend;
        }

        public void buffGain(BuffController newBuff)
        {
            //newBuff.getTime = timeSpend;
                 Buff BuffContext = newBuff.getMBuff();

            if (newBuff.getBuffType() == Buff.BufferType.NumBuff)
            {
                if (!newBuff.isForever)
                for (int i = 0; i < NumBuffList.Count; i++)
                { 
                    if(newBuff.BuffName == NumBuffList[i].BuffName)
                    {
                        NumBuffList[i].getTime = newBuff.getExistTime();
                        return;
                    }
                }
                NumBuffList.Add(newBuff);
            }
            else if (newBuff.getBuffType() == Buff.BufferType.PercentBuff)
            {
                if(!newBuff.isForever)
                for (int i = 0; i < PercentBuffList.Count; i++)
                {
                    if (newBuff.BuffName == PercentBuffList[i].BuffName)
                    {
                        PercentBuffList[i].getTime = newBuff.getExistTime();
                        return;
                    }
                }
                PercentBuffList.Add(newBuff);
            }
            else if (newBuff.getBuffType() == Buff.BufferType.WeaponBuff)
            {
                WeaponBuffList.Add(newBuff);
            }
            else { }
            buffChanged?.Invoke();
            buffGained?.Invoke(newBuff);

            if (BuffContext.ImpactVfx)
            {
                GameObject impactVfxInstance = Instantiate(BuffContext.ImpactVfx, aim_Actor.gameObject.transform.position,
                    Quaternion.LookRotation(aim_Actor.gameObject.transform.up));
                impactVfxInstance.transform.parent = aim_Actor.transform;
                if (BuffContext.ExistTime > 0)
                {
                    Destroy(impactVfxInstance.gameObject, BuffContext.ExistTime);
                }
            }
        }
        private bool checkActive(BuffController buff, float time)
        {
            //如果buff永久生效
            if (buff.getExistTime() < 0)
                return true;
            buff.getTime -= time;
            if (buff.getTime < 0)
            {
                return false;
            }
            return true;
        }

        public void buffLose(float time)
        {
            bool changed = false;
            List<BuffController> listToDelete = new List<BuffController>();
            List<BuffController> listToPercentDelete = new List<BuffController>();
            for (int i = 0; i < NumBuffList.Count; i++)
            {
                if (!checkActive(NumBuffList[i], time))
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
                    buffLost?.Invoke(listToDelete[i]);
                }
            }
            //检查PercentBuff
            for (int i = 0; i < PercentBuffList.Count; i++)
            {
                if (!checkActive(PercentBuffList[i], time))
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
                    buffLost?.Invoke(listToPercentDelete[i]);
                }
            }
            /*if (WeaponBuffList.Count > 0)
                foreach (BuffController m in WeaponBuffList)
                {
                    if (!checkActive(m, time))
                    {
                        changed = true;
                        WeaponBuffList.Remove(m);
                    }
                }
            if (listToWeaponBuffDelete.Count > 0)
            {
                for (int i = 0; i < listToPercentDelete.Count; i++)
                {
                    WeaponBuffList.Remove(listToPercentDelete[i]);
                    buffLost?.Invoke(listToDelete[i]);
                }
            }*/
            if (changed)
                buffChanged?.Invoke();
        }
        public void buffDelete(BuffController buff)
        {
            Buff.BufferType aimType = buff.getBuffType();
            switch (aimType)
            {
                case Buff.BufferType.NumBuff:
                    NumBuffList.Remove(buff);
                    buffChanged?.Invoke();
                    break;

                case Buff.BufferType.PercentBuff:
                    PercentBuffList.Remove(buff);
                    buffChanged?.Invoke();
                    break;
            }
            buffLost?.Invoke(buff);
        }
    }
}