using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;
using OneCanRun.Game;

namespace OneCanRun.UI
{
    public class HurtNumberHudManage : MonoBehaviour
    {
        [Tooltip("Hurt Number Prefab")]
        public GameObject HurtNumberPrefab;

        [Tooltip("Plane to show hurt number")]
        public RectTransform plane;
        // Start is called before the first frame update

        public static Game.Share.HurtNumberPoolManager poolManager;

        CollectDamageNumber collect;
        static private int num = 0;
        void Start()
        {
            if (poolManager == null)
                poolManager = new HurtNumberPoolManager(HurtNumberPrefab, plane);
            collect = GetComponentInParent<CollectDamageNumber>();
            collect.Dmg += AddHN;
            
        }

        // Update is called once per frame
         void AddHN(GameObject position, DamageType type, float damage)
        {
            GameObject HNInstance = poolManager.getObject();

            HurtNum newHn = HNInstance.GetComponent<HurtNum>();
            DebugUtility.HandleErrorIfNullGetComponent<HurtNum, HurtNumberHudManage>(newHn, this, HNInstance.gameObject);
            newHn.init(position,damage, type);
            HNInstance.SetActive(true);
            
        }
    }
}
