using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    public class BuffPickup : Pickup
    {
        CollisionBuffGiver buffGiver;
        //[Tooltip("拾取特效")]
        //public GameObject PickUpVfx;
        void OnTriggerEnter(Collider other)
        {
            buffGiver = GetComponent<CollisionBuffGiver>();     
            PlayerCharacterController pickingPlayer = other.GetComponent<PlayerCharacterController>();
            if(pickingPlayer.name == "Player1")
            {
                if (pickingPlayer != null)
                {
                    OnPicked(pickingPlayer);
                    buffGiver.buffGive(other);
                    PickupEvent evt = Events.PickupEvent;
                    evt.Pickup = gameObject;
                    EventManager.broadcast(evt);
                    GameObject VfxInstance = Instantiate(PickupVfxPrefab, pickingPlayer.transform);
                    Destroy(VfxInstance.gameObject, 1.5f);
                    AudioUtility.CreateSFX(PickupSfx, pickingPlayer.transform.position,  0f);
                    //VfxInstance.transform.position -= GetComponentInParent<Camera>().transform.right;
                    
                }
                Destroy(gameObject);
            }
            
        }
    }
}
