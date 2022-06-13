using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    public class WeaponPickup : Pickup
    {
        [Tooltip("´ýÊ°È¡ÎäÆ÷")]
        public WeaponController WeaponPrefab;

        protected override void Start()
        {
            base.Start();

            // Set all children layers to default (to prefent seeing weapons through meshes)
            //ÇÐ»»Í¼²ã
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t != transform)
                    t.gameObject.layer = 0;
            }
        }

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
