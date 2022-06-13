using TMPro;
using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;
using UnityEngine.UI;
using OneCanRun.GamePlay;
using System.Threading;

namespace OneCanRun.UI
{
    [RequireComponent(typeof(FillBarColorChange))]
    public class AmmoCounter : MonoBehaviour
    {

        [Tooltip("CanvasGroup to fade the ammo UI")]
        public CanvasGroup CanvasGroup;

        [Tooltip("Image for the weapon icon")] public Image WeaponImage;

        [Tooltip("Image component for the background")]
        public Image AmmoBackgroundImage;

        [Tooltip("Image component to display fill ratio")]
        public Image AmmoFillImage;

        [Tooltip("Text for Weapon index")]
        public TextMeshProUGUI WeaponIndexText;

        [Tooltip("Text for Bullet Counter")]
        public TextMeshProUGUI BulletCounter;

        [Tooltip("Reload Text for Weapons with physical bullets")]
        public RectTransform Reload;

        [Tooltip("Reloading text for Reloading Animation")]
        public RectTransform Reloading;

        [Header("Selection")]
        [Range(0, 1)]
        [Tooltip("Opacity when weapon not selected")]
        public float UnselectedOpacity = 0.5f;

        [Tooltip("Scale when weapon not selected")]
        public Vector3 UnselectedScale = Vector3.one * 0.8f;

        [Tooltip("Root for the control keys")] public GameObject ControlKeysRoot;

        [Header("Feedback")]
        [Tooltip("Component to animate the color when empty or full")]
        public FillBarColorChange FillBarColorChange;

        [Tooltip("Sharpness for the fill ratio movements")]
        public float AmmoFillMovementSharpness = 20f;

        public int WeaponCounterIndex { get; set; }

        PlayerWeaponsManager m_PlayerWeaponsManager;
        WeaponController m_Weapon;

        void Awake()
        {
            AmmoFillImage.fillAmount = 1;
            //EventManager.AddListener<AmmoPickupEvent>(OnAmmoPickup);
        }


        public void Initialize(WeaponController weapon, int weaponIndex)
        {
            
            m_Weapon = weapon;
            WeaponCounterIndex = weaponIndex;
            WeaponImage.sprite = weapon.WeaponIcon;
            if (!weapon.HasPhysicalBullets||!weapon.RemoteWeapons)
                BulletCounter.transform.parent.gameObject.SetActive(false);//非物理子弹强，剩余子弹量不可见
            else
            {
                BulletCounter.text = weapon.GetCarriedPhysicalBullets().ToString();
            }

            Reload.gameObject.SetActive(false);
            Reloading.gameObject.SetActive(false);
            //Reloading.gameObject.SetActive(true);
            m_PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerWeaponsManager, AmmoCounter>(m_PlayerWeaponsManager, this);

            WeaponIndexText.text = (WeaponCounterIndex + 1).ToString();//枪械标号

            FillBarColorChange.Initialize(1f, m_Weapon.GetAmmoNeededToShoot());
            
        }

        void Update()
        {
            bool isActiveWeapon = m_Weapon == m_PlayerWeaponsManager.GetActiveWeapon();


            transform.localScale = Vector3.Lerp(transform.localScale, isActiveWeapon ? Vector3.one : UnselectedScale,
                Time.deltaTime * 10);//非正在使用枪子弹条的大小平滑变化
            ControlKeysRoot.SetActive(!isActiveWeapon);
            if (m_Weapon.RemoteWeapons)
            {
                float currenFillRatio = m_Weapon.CurrentAmmoRatio;

                AmmoFillImage.fillAmount = Mathf.Lerp(AmmoFillImage.fillAmount, currenFillRatio,
                    Time.deltaTime * AmmoFillMovementSharpness);//子弹条的根据当前弹量比的平滑变化

                BulletCounter.text = m_Weapon.GetCarriedPhysicalBullets().ToString();

                FillBarColorChange.UpdateVisual(currenFillRatio);

                //当当前子弹量为0时，Reload提示弹出
                Reload.gameObject.SetActive(m_Weapon.GetCarriedPhysicalBullets() == 0 && m_Weapon.GetCurrentAmmo() == 0 && m_Weapon.IsWeaponActive && !m_Weapon.IsReloading);
                Reloading.gameObject.SetActive(m_Weapon.IsReloading);
            }
        }

    }
}
