using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    public class WeaponPickup : Pickup
    {
        [Tooltip("´ýÊ°È¡ÎäÆ÷")]
        public WeaponController WeaponPrefab;

        Interactive m_Interactive;

        protected override void Start()
        {
            base.Start();
   
            m_Interactive = GetComponent<Interactive>();
            DebugUtility.HandleErrorIfNullGetComponent<Interactive, Pickup>(m_Interactive, this, gameObject);
            

            // Set all children layers to default (to prefent seeing weapons through meshes)
            //ÇÐ»»Í¼²ã
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t != transform)
                    t.gameObject.layer = 0;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (m_Interactive.hasInteracted == true)
            {
                OnPicked(m_Interactive.m_PlayerCharacterController);
                PickupEvent evt = Events.PickupEvent;
                evt.Pickup = gameObject;
                EventManager.broadcast(evt);
            }
        }

        void OnTriggerEnter(Collider other) { }
        ////Åö×²´¥·¢½ûÓÃ
        //void OnTriggerEnter(Collider other)
        //{
        //    PlayerCharacterController pickingPlayer = other.GetComponent<PlayerCharacterController>();

        //    if (pickingPlayer != null)
        //    {
        //        OnPicked(pickingPlayer);

        //        PickupEvent evt = Events.PickupEvent;
        //        evt.Pickup = gameObject;
        //        EventManager.broadcast(evt);
        //    }
        //}

        //ÖØÐ´Ê°È¡º¯Êý
        protected override void OnPicked(PlayerCharacterController byPlayer)
        {
            PlayerWeaponsManager playerWeaponsManager = byPlayer.GetComponent<PlayerWeaponsManager>();
            if (playerWeaponsManager)
            {
                if (playerWeaponsManager.AddWeapon(WeaponPrefab))
                {
                    // Handle auto-switching to weapon if no weapons currently
                    if (playerWeaponsManager.GetActiveWeapon() == null)
                    {
                        playerWeaponsManager.SwitchWeapon(true);
                    }

                    PlayPickupFeedback();
                    Destroy(gameObject);
                }
            }
        }
    }
}
