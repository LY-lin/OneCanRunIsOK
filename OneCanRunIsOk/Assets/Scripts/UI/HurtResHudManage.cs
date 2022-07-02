using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;
using OneCanRun.Game;

namespace OneCanRun.UI
{
    public class HurtResHudManage : MonoBehaviour
    {
        [Tooltip("Hurt Resoource Prefab")]
        public GameObject HurtResPrefab;

        [Tooltip("Plane to show hurt number")]
        public RectTransform plane;
        // Start is called before the first frame update

        CollectDamage collect;
        public static HurtResourcePoolManager poolManager;
        void Start()
        {
            if (HurtResourcePoolManager.instance == null)
                poolManager = new HurtResourcePoolManager(HurtResPrefab, plane);
            if (poolManager == null)
            {
                poolManager = HurtResourcePoolManager.instance;
                poolManager.reset(HurtResPrefab, plane);
            }
            collect = GetComponentInParent<CollectDamage>();
            collect.hurted += AddHR;
        }

        void AddHR(GameObject position)
        {

            GameObject HRInstance = poolManager.getObject();
            Debug.LogError("create");
            HurtResource newHurtRes = HRInstance.GetComponent<HurtResource>();
            DebugUtility.HandleErrorIfNullGetComponent<HurtResource, HurtResHudManage>(newHurtRes, this,
                HRInstance.gameObject);
            position = position.transform.Find("WeaponRoot").gameObject;
            newHurtRes.init(position);

        }
    }
}