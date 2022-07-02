using UnityEngine.Events;
using UnityEngine;
using OneCanRun.Game;

namespace OneCanRun.GamePlay
{
    public class ChestContoller : MonoBehaviour
    {
        Interactive m_Interactive;
        //Dropable dropable;
        public UnityAction ChestOpen;


        // Start is called before the first frame update
        void Start()
        {
            m_Interactive = GetComponent<Interactive>();
            m_Interactive.beInteracted += beOpened;

            //dropable = GetComponent<Dropable>();
        }

        // Update is called once per frame
        void beOpened()
        {
            GetComponent<Animator>().SetTrigger("Open");
            ChestOpen?.Invoke();
            //cgManager.PlayCG();
            //dropable.drop();
        }
    }
}
