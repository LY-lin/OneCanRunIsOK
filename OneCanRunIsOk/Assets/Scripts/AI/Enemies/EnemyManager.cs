using System.Collections.Generic;
using OneCanRun.Game;
using UnityEngine;

namespace OneCanRun.AI.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        // ���˿������������һһ��Ӧ
        public List<EnemyController> Enemies { get; private set; }
        // ��������
        public int NumberOfEnemiesTotal { get; private set; }
        // ʣ�������
        public int NumberOfEnemiesRemaining => Enemies.Count;

        void Awake()
        {
            Enemies = new List<EnemyController>();
        }

        // ע����˿�����
        public void RegisterEnemy(EnemyController enemy)
        {
            Enemies.Add(enemy);

            NumberOfEnemiesTotal++;
        }

        // ע�����˿��������������¼����������й㲥��
        public void UnregisterEnemy(EnemyController enemyKilled)
        {
            int enemiesRemainingNotification = NumberOfEnemiesRemaining - 1;

            EnemyKillEvent evt = Events.EnemyKillEvent;
            evt.Enemy = enemyKilled.gameObject;
            evt.RemainingEnemyCount = enemiesRemainingNotification;
            EventManager.broadcast(evt);

            // removes the enemy from the list, so that we can keep track of how many are left on the map
            Enemies.Remove(enemyKilled);
        }
    }
}