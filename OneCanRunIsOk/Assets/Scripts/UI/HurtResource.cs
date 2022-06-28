using OneCanRun.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneCanRun.UI
{
    public class HurtResource : MonoBehaviour
    { 

        [Tooltip("image to show resource")]
        public Image HurtRes;

        [Tooltip("life time")]
         public float lifeTime = 3f;
    // Start is called before the first frame update

        private RectTransform plane;
        private GameObject obj;
        private float radis = Screen.height / 3f;
        private float initTime;
        private Health health;
        public void init(GameObject obj)
        {
            this.obj = obj;
            plane = this.transform.parent.GetComponent<RectTransform>();
            initTime = Time.time;
            health = obj.transform.parent.GetComponent<Health>();
            if (health == null)
                health = obj.GetComponentInParent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            
            if (obj && obj.activeSelf && health.CurrentHealth > 0)
            {
                transform.localPosition = GetUIPosition(obj.transform.position);
                Vector3 temp = HurtRes.transform.localScale * 10f;
                //大小改变
                float Base = (lifeTime-(Time.time-initTime)) / lifeTime;
                Base = Base > 0.5f ? Base : 0.5f;
                HurtRes.transform.localScale = Vector3.one * 2f *Base;
            }
            else
                Destroy(this.gameObject);
           if (Time.time - initTime >= lifeTime)
                Destroy(this.gameObject);
        }

        //获得伤害来源的UI坐标
        private Vector3 GetUIPosition(Vector3 point)
        {
            HurtRes.gameObject.SetActive(true);
            Vector2 position = RectTransformUtility.WorldToScreenPoint(Camera.main, point);
            position = new Vector2(position.x - Screen.width / 2f, position.y - Screen.height / 2f);
            //如果在radis大小的圆内，不会显示
            if (Mathf.Sqrt(Vector2.SqrMagnitude(position)) < radis)
                HurtRes.gameObject.SetActive(false);
            else
                HurtRes.gameObject.SetActive(true);
            position = position.normalized * radis;
            //获得旋转的欧拉角
            float tanValue = (0 - position.x) / position.y;
            float tanArc = Mathf.Atan(tanValue);
            float tanAngleValue2 = tanArc/Mathf.PI * 180;
            if (position.y < 0)
                tanAngleValue2 += 180;
            Vector3 eulerAngle = new Vector3(0, 0, tanAngleValue2);
           
            HurtRes.transform.rotation = Quaternion.Euler(eulerAngle);
            position = new Vector2(position.x + Screen.width / 2f, position.y + Screen.height / 2f);
            Vector2 uiPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(plane, position, null, out uiPosition);
            return uiPosition;
        }
    }
}