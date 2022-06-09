using OneCanRun.Game.Share;
using UnityEngine;


namespace OneCanRun.Game
{
    public class SkillBuffGiver : MonoBehaviour
    {
        public GameObject buffObject;
        private BuffController mbuff;
        public ActorBuffManager aim_actorBuffManager;

        public void buffGive()
        {
            aim_actorBuffManager = GetComponentInParent<ActorBuffManager>();
            Buff aimBuff = buffObject.GetComponent<Buff>();
            mbuff = new BuffController(aimBuff);
            aim_actorBuffManager.buffGain(mbuff);
        }
    }
}