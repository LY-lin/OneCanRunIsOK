using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.Game.Share
{
    public class CollectDamageNumber : MonoBehaviour
    {
        public UnityAction<GameObject, DamageType, float> Dmg;
        public UnityAction Feedback;
        public UnityAction<GameObject> hurted;
        // Start is called before the first frame update

        public void produce(GameObject position, DamageType Type, float Damage)
        {
            Dmg?.Invoke(position, Type, Damage);
            Feedback?.Invoke();
        }

        public void getHurt(GameObject pos)
        {
            hurted?.Invoke(pos);
        }

    }
}
