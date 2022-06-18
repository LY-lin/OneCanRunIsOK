using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    public class BuffPickup : Pickup
    {
        CollisionBuffGiver buffGiver;
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
                }
                Destroy(gameObject);
            }
            
        }
    }
}
