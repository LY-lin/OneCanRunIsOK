using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
namespace OneCanRun.GamePlay
{
    public class CollisionBuffGiver : MonoBehaviour
    {
        public GameObject buffObject;
        private BuffController mbuff;
        private ActorBuffManager aim_actorBuffManager;

        public void buffGive()
        {
            
            aim_actorBuffManager.buffGain(mbuff);
        }
    }
}
