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
        public void init(GameObject obj)
        {
            this.obj = obj;
            plane = this.transform.parent.GetComponent<RectTransform>();
            initTime = Time.time;
            
        }

        // Update is called once per frame
        void Update()
        {
            transform.localPosition = GetUIPosition(obj.transform.position);
            if (Time.time - initTime >= lifeTime)
                Destroy(this.gameObject);
        }

        private Vector3 GetUIPosition(Vector3 point)
        {
            HurtRes.gameObject.SetActive(true);
            Vector2 position = RectTransformUtility.WorldToScreenPoint(Camera.main, point);
            position = new Vector2(position.x - Screen.width / 2f, position.y - Screen.height / 2f);
            Debug.Log(position);
            Debug.Log(Mathf.Sqrt(Vector2.SqrMagnitude(position)));
            if (Mathf.Sqrt(Vector2.SqrMagnitude(position)) < radis)
                HurtRes.gameObject.SetActive(false);
            else
                HurtRes.gameObject.SetActive(true);
            position = position.normalized * radis;
            float tanValue = (0 - position.x) / position.y;
            float tanArc = Mathf.Atan(tanValue);
            float tanAngleValue2 = tanArc/Mathf.PI * 180;
            if (position.y < 0)
                tanAngleValue2 += 180;
            Vector3 pos = new Vector3(0, 0, tanAngleValue2);
           
            HurtRes.transform.rotation = Quaternion.Euler(pos);
            position = new Vector2(position.x + Screen.width / 2f, position.y + Screen.height / 2f);
            Vector2 uiPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(plane, position, null, out uiPosition);
            return uiPosition;
        }
    }
}