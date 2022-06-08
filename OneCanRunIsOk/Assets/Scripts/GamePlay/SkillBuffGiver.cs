using OneCanRun.Game.Share;
using UnityEngine;
using OneCanRun.Game;


namespace OneCanRun.GamePlay
{
    public class SkillBuffGiver : MonoBehaviour
    {
        public GameObject buffObject;
        private BuffController mbuff;
        private ActorBuffManager aim_actorBuffManager;
        private void Awake()
        {
            Buff aimBuff = buffObject.GetComponent<Buff>();
            mbuff = new BuffController(aimBuff);
            //mbuff = GetComponent<BuffController>();
            aim_actorBuffManager = GetComponentInParent<ActorBuffManager>();
            //aim_actorBuffManager = transform.parent.GetComponent<ActorBuffManager>();
            DebugUtility.HandleErrorIfNullGetComponent<ActorBuffManager, SkillBuffGiver>(aim_actorBuffManager,
                this, gameObject);
        }

        public void buffGive()
        {
            aim_actorBuffManager.buffGain(mbuff);
        }
    }
}
