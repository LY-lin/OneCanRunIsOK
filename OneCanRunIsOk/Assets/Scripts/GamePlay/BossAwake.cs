using UnityEngine.Events;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    public class BossAwake : MonoBehaviour
    {
        public UnityAction bossAwake;
        ChestContoller Bosschest;
        // Start is called before the first frame update
        void Start()
        {
            Bosschest = GetComponent<ChestContoller>();
            Bosschest.ChestOpen += ChestWasOpen;
        }
        void ChestWasOpen()
        {
            bossAwake?.Invoke();
        }
    }
}
