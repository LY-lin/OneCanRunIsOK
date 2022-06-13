using OneCanRun.Game.Share;
using System.Collections.Generic;
using UnityEngine;


namespace OneCanRun.Game
{
    public class DetectBuffGiver : MonoBehaviour
    {
        public Transform DetectionSourcePoint;
        public GameObject buffObject;
        private BuffController mbuff;
        private ActorBuffManager aim_actorBuffManager;
        ActorsManager manager;
        List<Actor> buffReciver;

        public float BuffRange = 30f;
        public affiliationType Affiliation;

        // Start is called before the first frame update
        void Start()
        {
            buffReciver = new List<Actor>();
            manager = FindObjectOfType<ActorsManager>();
            Buff aimBuff = buffObject.GetComponent<Buff>();
            mbuff = new BuffController(aimBuff);
            //DebugUtility.HandleErrorIfNullFindObject<ActorsManager, DetectionModule>(manager, this);
        }
        void Update()
        {
            detectActor();
        }
        public void buffGive(Actor actor)
        {
            aim_actorBuffManager = actor.GetComponent<ActorBuffManager>();
            
            aim_actorBuffManager.buffGain(mbuff);
            
        }

        public void buffTake(Actor actor)
        {
            aim_actorBuffManager = actor.GetComponent<ActorBuffManager>();
            aim_actorBuffManager.buffDelete(mbuff);
        }

        public void detectActor()
        {
            foreach (Actor actor in manager.Actors)
            {
                if (actor.Affiliation == Affiliation)
                {
                    float sqrDistance = (actor.transform.position - DetectionSourcePoint.position).sqrMagnitude;
                    // 
                    if (sqrDistance < BuffRange)
                    {
                        if (!buffReciver.Contains(actor))
                        {
                            buffGive(actor);
                            buffReciver.Add(actor);
                        }

                    }
                    else
                    {
                        if (buffReciver.Contains(actor))
                        {
                            buffTake(actor);
                            buffReciver.Remove(actor);
                        }
                    }
                }
            }
        }
    }
}