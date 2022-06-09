using UnityEngine;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class CollisionBuffGiver : MonoBehaviour
    {
        public GameObject buffObject;
        private BuffController mbuff;
        private ActorBuffManager aim_actorBuffManager;

        public void buffGive(Collider other)
        {
            aim_actorBuffManager = other.GetComponentInParent<ActorBuffManager>();
            Buff aimBuff = buffObject.GetComponent<Buff>();
            mbuff = new BuffController(aimBuff);
            aim_actorBuffManager.buffGain(mbuff);
        }
    }
}
