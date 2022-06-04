using UnityEngine;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(Health), typeof(Actor))]
    public class Enemy : MonoBehaviour
    {
        [Tooltip("Maximum amount of health")]
        public float maxHealth;

        [Tooltip("Maximum amount of attck distance")]
        public float attckDistance;

        public Health health;

        public virtual void UpdateAiStateTransitions()
        {

        }

        public virtual void UpdateCurrentAIState()
        {

        }


    }
}
