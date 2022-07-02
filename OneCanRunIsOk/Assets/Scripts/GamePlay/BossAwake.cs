using UnityEngine.Events;
using UnityEngine;
using OneCanRun.Game;

namespace OneCanRun.GamePlay
{
    public class BossAwake : MonoBehaviour
    {
        public UnityAction bossAwake;
        ChestContoller Bosschest;
        CgManager cgManager;
        // Start is called before the first frame update
        void Start()
        {
            Bosschest = GetComponent<ChestContoller>();
            Bosschest.ChestOpen += ChestWasOpen;

            cgManager = GameObject.FindObjectOfType<CgManager>();
            DebugUtility.HandleErrorIfNullFindObject<CgManager, BossAwake>(cgManager, this);
        }
        void ChestWasOpen()
        {
            bossAwake?.Invoke();
            cgManager.PlayCG();
            AudioSource BGM = GameObject.Find("GameManager").GetComponent<AudioSource>();
            BGM.Stop();
            BGM.clip = Resources.Load<AudioClip>("Glenn Stafford - Heaven's Devils");
            BGM.Play();
        }
    }
}
