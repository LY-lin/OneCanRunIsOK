using System.Collections.Generic;
using OneCanRun.Game;
using UnityEngine;

namespace OneCanRun.AI.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        // 敌人控制器，与敌人一一对应
        public List<EnemyController> Enemies { get; private set; }
        // 敌人总数
        public int NumberOfEnemiesTotal { get; private set; }
        // 剩余敌人数
        public int NumberOfEnemiesRemaining => Enemies.Count;

        void Awake()
        {
            Enemies = new List<EnemyController>();
        }

        // 注册敌人控制器
        public void RegisterEnemy(EnemyController enemy)
        {
            if (!Enemies.Contains(enemy))
            {
                Enemies.Add(enemy);

                NumberOfEnemiesTotal++;

                GameObject myPath;
                if (Random.Range(0f,2f) > 1.0f)
                {
                    myPath = GameObject.Find("path");
                }
                else
                {
                    myPath = GameObject.Find("path1");
                }
                
                PatrolPath patrol = myPath.GetComponent<PatrolPath>();
                patrol.addEnemy(enemy);
            }
        }

        // 注销敌人控制器，并调用事件控制器进行广播？
        public void UnregisterEnemy(EnemyController enemyKilled)
        {
            // removes the enemy from the list, so that we can keep track of how many are left on the map
            Enemies.Remove(enemyKilled);
        }
    }
}