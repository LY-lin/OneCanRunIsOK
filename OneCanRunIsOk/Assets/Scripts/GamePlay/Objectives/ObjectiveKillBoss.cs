using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class ObjectiveKillBoss : Objective
    {
        //Boss m_boss;
        BossAwake trap;
        //Boss boss;
        protected override void Start()
        {
            base.Start();
            trap=GetComponent<BossAwake>();
            trap.bossAwake += BossAwake;
            Debug.Log(this.gameObject);
        }

        void BossAwake()
        {
            UpdateObjective("KILL THE DRAGON !!!",string.Empty, "Kill the dragon!!!");
        }

        public void BossKilled()
        {
            CompleteObjective("Mission Completed.",string.Empty, "Win");
        }
    }
}
