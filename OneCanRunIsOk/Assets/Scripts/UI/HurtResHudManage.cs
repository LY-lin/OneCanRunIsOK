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

        private int n = 0;
        CollectDamage collect;
        void Start()
        {
            collect = GetComponentInParent<CollectDamage>();
            collect.hurted += AddHR;
        }

        void AddHR(GameObject position)
        { 
            GameObject HRInstance = Instantiate(HurtResPrefab, plane);
            HurtResource newHurtRes = HRInstance.GetComponent<HurtResource>();
            DebugUtility.HandleErrorIfNullGetComponent<HurtResource, HurtResHudManage>(newHurtRes, this,
                HRInstance.gameObject);
            position = position.transform.Find("WeaponRoot").gameObject;
            newHurtRes.init(position);

        }
    }
}