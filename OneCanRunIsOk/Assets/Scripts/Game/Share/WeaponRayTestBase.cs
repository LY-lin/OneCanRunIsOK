using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{ 

    public class WeaponRayTestBase : MonoBehaviour
    {
        [Tooltip("layer")]
        public LayerMask layer;

        GameObject Owner;
        bool isAttacking =false;
        public Dictionary<int, Vector3> dic_lastPoints = new Dictionary<int, Vector3>(); //存放上个位置信息
        public Transform[] Points;
        Dictionary<GameObject,int> GetDamaged = new Dictionary<GameObject, int>();
        public void Attack(WeaponController wc)
        {
            isAttacking = wc.isAttacking;
            Owner = wc.Owner;
            if (dic_lastPoints.Count == 0)
            {
                for (int i = 0; i < wc.Points.Length; i++)
                {
                    dic_lastPoints.Add(wc.Points[i].GetHashCode(), wc.Points[i].position);
                }
            }
            Points = wc.Points;
        }

        void Update()
        {
            if(isAttacking)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    var nowPos = Points[i];
                    dic_lastPoints.TryGetValue(nowPos.GetHashCode(), out Vector3 lastPos);

                    Debug.DrawLine(nowPos.position, lastPos, Color.blue, 1f); ;
                    Debug.DrawRay(lastPos, nowPos.position - lastPos, Color.blue, 1f);

                    Ray ray = new Ray(lastPos, nowPos.position - lastPos);
                    RaycastHit[] raycastHits = new RaycastHit[6];
                    Physics.RaycastNonAlloc(ray, raycastHits, Vector3.Distance(lastPos, nowPos.position), layer, QueryTriggerInteraction.Ignore);

                    foreach (var item in raycastHits)
                    {
                        if (item.collider == null) 
                            continue;
                        else
                        {
                            if (GetDamaged.ContainsKey(item.collider.gameObject))
                            {
                                continue;
                            }
                            else
                            {
                                if (item.collider.gameObject.layer == LayerMask.NameToLayer("weapon"))
                                {
                                    continue;
                                }
                                Damageable damageable = item.collider.GetComponent<Damageable>();
                                if (damageable)
                                {
                                    damageable.InflictDamage(10f, false, Owner);
                                }
                                GetDamaged.Add(item.collider.gameObject, 1);
                            }
                        }
                    }

                    if (nowPos.position != lastPos)
                    {
                        dic_lastPoints[nowPos.GetHashCode()] = nowPos.position;//存入上个位置信息
                    }
                }
            }
        }
    }
   
}
