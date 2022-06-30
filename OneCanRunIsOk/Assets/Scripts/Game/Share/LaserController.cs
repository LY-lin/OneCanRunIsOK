using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    public class LaserController : MonoBehaviour
    {
        //是否当武器使用（进行激光弹道修正）
        public bool usedAsWeapon = false;

        public GameObject HitEffect;
        public float HitOffset = 0;

        public float MaxLength = 10f;
        private LineRenderer Laser;

        public float MainTextureLength = 1f;
        public float NoiseTextureLength = 1f;

        public Transform LaserSocket;
        public DamageType damageType;
        public float damage = 20f;

        [Tooltip("how many frames to calculate per attack")]
        public int deltaCount = 25;
        private float totalDeltaTime = 0;
        private int curDeltaCount = 0;

        public GameObject Owner;
        //public GameObject hurtNumber;

        private Vector4 Length = new Vector4(1, 1, 1, 1);
        //private Vector4 LaserSpeed = new Vector4(0, 0, 0, 0); {DISABLED AFTER UPDATE}
        //private Vector4 LaserStartSpeed; {DISABLED AFTER UPDATE}
        //One activation per shoot
        private bool LaserSaver = false;
        private bool UpdateSaver = false;

        private ParticleSystem[] Effects;
        private ParticleSystem[] Hit;

        private GameObject LaserInstance;
        public Transform PlayerSocket;

        private bool isLasering=false;
        //private Vector3 hitPoint;

        public AudioSource audioSource;
        public AudioClip LaserSfx;

        void Start()
        {
            //Get LineRender and ParticleSystem components from current prefab;  
            Laser = GetComponent<LineRenderer>();
            Effects = GetComponentsInChildren<ParticleSystem>();
            Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
            //if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) LaserStartSpeed = Laser.material.GetVector("_SpeedMainTexUVNoiseZW");
            //Save [1] and [3] textures speed
            //{ DISABLED AFTER UPDATE}
            //LaserSpeed = LaserStartSpeed;

            audioSource = GetComponent<AudioSource>();
            DebugUtility.HandleErrorIfNullGetComponent<AudioSource, LaserController>(audioSource,
                this, gameObject);

            Actor actor = Owner.GetComponent<Actor>();
            if (actor)
            {
                ActorProperties tmp = actor.GetActorProperties();
                if (damageType == DamageType.physical)
                {
                    damage += tmp.getPhysicalAttack();
                }
                else if (damageType == DamageType.magic)
                {
                    damage += tmp.getMagicAttack();
                }
            }

            audioSource.PlayOneShot(LaserSfx);
        }

        void Update()
        {
            //if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) Laser.material.SetVector("_SpeedMainTexUVNoiseZW", LaserSpeed);
            //SetVector("_TilingMainTexUVNoiseZW", Length); - old code, _TilingMainTexUVNoiseZW no more exist
            Laser.material.SetTextureScale("_MainTex", new Vector2(Length[0], Length[1]));
            Laser.material.SetTextureScale("_Noise", new Vector2(Length[2], Length[3]));

            //To set LineRender position
            if (Laser != null && UpdateSaver == false)
            {
                Laser.SetPosition(0, transform.position);
                Transform raySocket = (usedAsWeapon) ? PlayerSocket : this.transform;
                RaycastHit hit; //DELATE THIS IF YOU WANT USE LASERS IN 2D
                                //ADD THIS IF YOU WANNT TO USE LASERS IN 2D: RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, MaxLength);       
                if (Physics.Raycast(raySocket.position, raySocket.TransformDirection(Vector3.forward), out hit, MaxLength))//CHANGE THIS IF YOU WANT TO USE LASERRS IN 2D: if (hit.collider != null)
                {
                    //End laser position if collides with object
                    //hitPoint = hit.point;
                    Laser.SetPosition(1, hit.point);
                    HitEffect.transform.position = hit.point + hit.normal * HitOffset;
                    //Hit effect zero rotation
                    HitEffect.transform.rotation = Quaternion.identity;
                    foreach (var AllPs in Effects)
                    {
                        if (!AllPs.isPlaying) AllPs.Play();
                    }
                    //Texture tiling
                    Length[0] = MainTextureLength * (Vector3.Distance(transform.position, hit.point));
                    Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, hit.point));
                    //Texture speed balancer {DISABLED AFTER UPDATE}
                    //LaserSpeed[0] = (LaserStartSpeed[0] * 4) / (Vector3.Distance(transform.position, hit.point));
                    //LaserSpeed[2] = (LaserStartSpeed[2] * 4) / (Vector3.Distance(transform.position, hit.point));

                    //damageCalculate
                    if (curDeltaCount == deltaCount)
                    {
                        Collider col = hit.collider;
                        Damageable damageable = col.GetComponent<Damageable>();
                        if (damageable)
                        {

                            Actor actor = col.gameObject.GetComponentInParent<Actor>();
                            ActorProperties colliderProperty = actor.GetActorProperties();
                            float finalDamage = calculateDamage(colliderProperty, damage * totalDeltaTime, damageType);
                            damageable.InflictDamage(finalDamage, false, Owner, col.gameObject, damageType);
                        }
                    }
                }
                else
                {
                    //End laser position if doesn't collide with object
                    var EndPos = raySocket.position + raySocket.forward * MaxLength;
                    //hitPoint = EndPos;
                    Laser.SetPosition(1, EndPos);
                    HitEffect.transform.position = EndPos;
                    foreach (var AllPs in Hit)
                    {
                        if (AllPs.isPlaying) AllPs.Stop();
                    }
                    //Texture tiling
                    Length[0] = MainTextureLength * (Vector3.Distance(transform.position, EndPos));
                    Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, EndPos));
                    //LaserSpeed[0] = (LaserStartSpeed[0] * 4) / (Vector3.Distance(transform.position, EndPos)); {DISABLED AFTER UPDATE}
                    //LaserSpeed[2] = (LaserStartSpeed[2] * 4) / (Vector3.Distance(transform.position, EndPos)); {DISABLED AFTER UPDATE}
                }
                //Insurance against the appearance of a laser in the center of coordinates!
                if (Laser.enabled == false && LaserSaver == false)
                {
                    LaserSaver = true;
                    Laser.enabled = true;
                }


            }
            if (curDeltaCount == deltaCount)
            {
                curDeltaCount = 0;
                totalDeltaTime = 0;
            }
            curDeltaCount++;
            totalDeltaTime += Time.deltaTime;
        }

        public void DisablePrepare()
        {
            if (Laser != null)
            {
                Laser.enabled = false;
            }
            UpdateSaver = true;
            //Effects can = null in multiply shooting
            if (Effects != null)
            {
                foreach (var AllPs in Effects)
                {
                    if (AllPs.isPlaying) AllPs.Stop();
                }
            }
        }

        private float calculateDamage(ActorProperties colliderProperty, float damage, DamageType damageType)
        {
            if (colliderProperty == null)
                return 0;
            float finalDamage = 0;
            if (damageType == DamageType.magic)
            {
                finalDamage = damage - colliderProperty.getMagicDefence();
            }
            else
            {
                finalDamage = damage - colliderProperty.getPhysicalDefence();
            }
            if (finalDamage < 0f)
                finalDamage = 0f;

            //下取整
            finalDamage = Mathf.Floor(finalDamage);

            //GameObject hurtNumberParent = GameObject.Find("HurtNumberCollector");
            //if (hurtNumber && hurtNumberParent)
            //{
            //    //Debug.Log("count!");
            //    GameObject hurt = GameObject.Instantiate(hurtNumber, hurtNumberParent.transform);
            //    hurt.transform.position = damagePoint;
            //    hurt.GetComponent<HurtNumber>().init(finalDamage, damageType);


            //}

            return finalDamage;
        }

        public void StartLaser()
        {
            if (!isLasering)
            {
                LaserInstance = Instantiate(gameObject, LaserSocket);
                isLasering = true;
            }
        }

        public void StartLaser(Transform socket)
        {
            if (!isLasering)
            {
                PlayerSocket = socket;
                LaserInstance = Instantiate(gameObject, LaserSocket);
                isLasering = true;
            }
        }

        public void StopLaser()
        {
            if (isLasering)
            {
                DisablePrepare();
                Destroy(LaserInstance, 0.1f);
                isLasering = false;
            }
        }
    }
}
