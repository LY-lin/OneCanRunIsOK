using OneCanRun.Game.Share;
using UnityEngine;
using OneCanRun.Game;


namespace OneCanRun.GamePlay
{
    public class SkillBuffGiver : MonoBehaviour
    {
        public GameObject buffObject;
        private BuffController mbuff;
        public ActorBuffManager aim_actorBuffManager;

        public void buffGive()
        {
            Buff aimBuff = buffObject.GetComponent<Buff>();
            mbuff = new BuffController(aimBuff);
            aim_actorBuffManager.buffGain(mbuff);
        }
    }
}