using UnityEngine;
using OneCanRun.Game.Share;
using OneCanRun;

namespace OneCanRun.GamePlay
{
    public class ObjectiveKillBoss : Objective
    {
        //Boss m_boss;
        BossAwake trap;
        protected override void Start()
        {
            base.Start();
            trap=GetComponent<BossAwake>();
            trap.bossAwake += BossAwake;
        }

        void BossAwake()
        {
            UpdateObjective("KILL THE DRAGON !!!",string.Empty, "Kill the dragon!!!");
        }

        void BossKilled()
        {
            CompleteObjective("Mission Completed.",string.Empty, "Win");
        }
    }
}
