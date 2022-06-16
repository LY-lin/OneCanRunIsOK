using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class FireRain : MonoBehaviour
    {
        WeaponController m_SkillWeapon;
        //BulletStandard m_BulletStandard;
        // Start is called before the first frame update
        void Start()
        {
            m_SkillWeapon = GetComponent<WeaponController>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, FireRain>(m_SkillWeapon,
                this, gameObject);

        }

        // Update is called once per frame
        void Update()
        {
            m_SkillWeapon.HandleShootInputs(true, false, false);
        }
    }
}
