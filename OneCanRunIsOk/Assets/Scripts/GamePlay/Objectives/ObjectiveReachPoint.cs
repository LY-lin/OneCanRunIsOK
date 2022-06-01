using OneCanRun;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay.Objectives
{
    public class ObjectiveReachPoint : Objective
    {
        [Tooltip("Visible transform that will be destroyed once the objective is completed")]
        public Transform DestroyRoot;

        void Awake()
        {
            if (DestroyRoot == null)
                DestroyRoot = transform;
        }
        protected override void Start()
        {
            base.Start();

            //EventManager.addListener<ReachPointEvent>(OnPickupEvent);
        }



        void OnTriggerEnter(Collider other)
        {
            if (IsCompleted)
                return;

            var player = other.GetComponent<PlayerCharacterController>();
            // test if the other collider contains a PlayerCharacterController, then complete
            if (player != null)
            {
                CompleteObjective(string.Empty, string.Empty, "Objective complete : " + Title);

                Debug.Log("Reach Point");
                // destroy the transform, will remove the compass marker if it has one
                Destroy(DestroyRoot.gameObject);
            }
        }
    }
}


