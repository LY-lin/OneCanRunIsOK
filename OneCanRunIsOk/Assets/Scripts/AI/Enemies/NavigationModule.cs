using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.AI.Enemies
{
    public class NavigationModule : MonoBehaviour
    {
        // ����ƶ��ٶ�
        [Header("Parameters")]
        [Tooltip("The maximum speed at which the enemy is moving (in world units per second).")]
        public float MoveSpeed = 0f;

        // ��תʱ������ٶ�
        [Tooltip("The maximum speed at which the enemy is rotating (degrees per second).")]
        public float AngularSpeed = 0f;

        // ���ٶ�
        [Tooltip("The acceleration to reach the maximum speed (in world units per second squared).")]
        public float Acceleration = 0f;
    }
}
