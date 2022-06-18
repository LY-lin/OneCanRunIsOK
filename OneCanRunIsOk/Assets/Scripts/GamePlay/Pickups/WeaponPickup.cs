using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    public class WeaponPickup : Pickup
    {
        [Tooltip("待拾取武器")]
        public WeaponController WeaponPrefab;

        Interactive m_Interactive;

        protected override void Start()
        {
            base.Start();
   
            m_Interactive = GetComponent<Interactive>();
            DebugUtility.HandleErrorIfNullGetComponent<Interactive, Pickup>(m_Interactive, this, gameObject);
            

            // Set all children layers to default (to prefent seeing weapons through meshes)
            //切换图层
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t != transform)
                    t.gameObject.layer = 0;
            }
            m_Interactive.beInteracted += bePickup;
        }

        /*protected override void Update()
        {
            base.Update();

        }*/

        protected void bePickup()
        {
            if (m_Interactive.hasInteracted == true)
            {
                OnPicked(m_Interactive.m_PlayerCharacterController);
                PickupEvent evt = Events.PickupEvent;
                evt.Pickup = gameObject;
                EventManager.broadcast(evt);
            }
        }
        void OnTriggerEnter(Collider other) { 
            
        }
        ////碰撞触发禁用
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

        //重写拾取函数
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
