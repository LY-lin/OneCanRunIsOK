using UnityEngine;
using UnityEngine.Events;
using OneCanRun.Game.Share;

namespace OneCanRun.Game
{
    // This class contains general information describing an actor (player or enemies).
    // It is mostly used for AI detection logic and determining if an actor is friend or foe
    public class Actor : MonoBehaviour
    {
        [Tooltip("Represents the affiliation (or team) of the actor. Actors of the same affiliation are friendly to each other")]
        public int Affiliation;

        [Tooltip("Represents point where other actors will aim when they attack this actor")]
        public Transform AimPoint;

        ActorsManager m_ActorsManager;

        ActorProperties m_BaseProperties;

        ActorProperties m_PresentProperties;

        ActorBuffManager m_buffs;
        
        private void Awake()
        {
            m_buffs = GetComponent<ActorBuffManager>();
            m_buffs.buffChanged += buffsAct;
        }

        public ActorProperties getBaseProperties()
        {
            return m_BaseProperties;
        }

        public bool setPresentProperties(ActorProperties newActorProperties)
        {
            m_PresentProperties = newActorProperties;
            return true;
        }

        void Start()
        {
            m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, Actor>(m_ActorsManager, this);

            // Register as an actor
            if (!m_ActorsManager.Actors.Contains(this))
            {
                m_ActorsManager.Actors.Add(this);
            }
        }

        void OnDestroy()
        {
            // Unregister as an actor
            if (m_ActorsManager)
            {
                m_ActorsManager.Actors.Remove(this);
            }
        }


        public void buffsAct()
        {
            ActorProperties properties = getBaseProperties();
            foreach (BuffController m in m_buffs.NumBuffList)
            {
                m.ActorbuffAct(ref properties);
            }
            foreach (BuffController m in m_buffs.PercentBuffList)
            {
                m.ActorbuffAct(ref properties);
            }
            foreach (BuffController m in m_buffs.WeaponBuffList)
            {
                m.ActorbuffAct(ref properties);
            }
            setPresentProperties(properties);
        }
    }
}
