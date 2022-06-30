using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;

namespace OneCanRun.GamePlay
{
    public class CgManager : MonoBehaviour
    {
        [Header("GameObject References")]
        public GameObject player;
        public GameObject playCamera;
        public GameObject cgCamera;

        [Header("Transform and Vfx")]
        public Transform TP_begin;
        public Transform TP_end;
        public GameObject TP_Vfx;

        [Header("cg durations and other parameters")]
        public float dragonCgDuration= 15.0f;
        public float dragonCgRotationTime = 8.0f;

        public float firstPersonCgDuration = 5.0f;
        public float firstPersonCgDisplaceTime = 2.5f;
        public float firstPersonCgRotationTime = 1.0f;

        public float cameraRotateSpeed = 1.0f;
        public float orientationSpeed = 5.0f;
        public float displacementSpeed = 2.0f;
        public float VfxExistingTime = 5.0f;

        private Camera playCamera_camera;
        private Camera cgCamera_camera;
        private AudioListener playCamera_audioListener;
        private AudioListener cgCamera_audioListener;

        private PlayerInputHandler playerInputHandler;

        private bool isPlaying = false;
        private bool isDragonCg = false;
        private bool isFirstPersonCg = false;
        private bool createVfx = false;

        private float playingTime;
        private Quaternion targetRotation = Quaternion.Euler(-60, 90, 0);

        private GameObject tpBeginInstance;
        private GameObject tpEndInstance;

        // Start is called before the first frame update
        void Start()
        {
            playCamera_camera = playCamera.GetComponent<Camera>();
            DebugUtility.HandleErrorIfNullGetComponent<Camera, CgManager>(playCamera_camera, this, gameObject);

            cgCamera_camera = cgCamera.GetComponent<Camera>();
            DebugUtility.HandleErrorIfNullGetComponent<Camera, CgManager>(cgCamera_camera, this, gameObject);

            playCamera_audioListener = playCamera.GetComponent<AudioListener>();
            DebugUtility.HandleErrorIfNullGetComponent<AudioListener, CgManager>(playCamera_audioListener, this, gameObject);

            cgCamera_audioListener = cgCamera.GetComponent<AudioListener>();
            DebugUtility.HandleErrorIfNullGetComponent<AudioListener, CgManager>(cgCamera_audioListener, this, gameObject);

            playerInputHandler = player.GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, CgManager>(playerInputHandler, this, gameObject);


        }

        // Update is called once per frame
        void Update()
        {
            if (isPlaying)
            {
                if (isDragonCg)
                {
                    if (playingTime + dragonCgDuration < Time.time)
                    {
                        ChangeToPlayerCamera();
                        isDragonCg = false;
                        isFirstPersonCg = true;
                        //playCamera.transform.rotation = Quaternion.Euler(89, 0, 0);
                        playingTime = Time.time;

                    }
                    else if (playingTime + dragonCgRotationTime < Time.time)
                    {
                        cgCamera.transform.rotation = Quaternion.Slerp(cgCamera.transform.rotation, targetRotation, Time.deltaTime * cameraRotateSpeed);
                    }
                }

                else if (isFirstPersonCg)
                {
                    if (playingTime + firstPersonCgDuration < Time.time)
                    {
                        tpEndInstance = Instantiate(TP_Vfx, TP_end);
                        player.transform.position = TP_end.position + TP_end.forward;
                        Destroy(tpEndInstance, VfxExistingTime);
                        isFirstPersonCg = false;
                        StopCG();
                    }
                    else if(playingTime + firstPersonCgDisplaceTime < Time.time)
                    {
                        if (!createVfx)
                        {
                            createVfx = true;
                            //playCamera.transform.rotation = Quaternion.Euler(-20, 0, 0);
                            tpBeginInstance = Instantiate(TP_Vfx, TP_begin);
                            Destroy(tpBeginInstance, firstPersonCgDuration - firstPersonCgDisplaceTime + VfxExistingTime);
                        }
                        player.gameObject.transform.position = Vector3.MoveTowards(player.gameObject.transform.position, TP_begin.position, displacementSpeed * Time.deltaTime);
                        //playCamera.transform.rotation = Quaternion.Euler(-20, 0, 0);
                    }
                    else if(playingTime + firstPersonCgRotationTime < Time.time)
                    {
                        OrientTowards(TP_begin.position);
                    }
                }
                
            }
        }

        void ChangeToCgCamera()
        { 
            playCamera_camera.enabled = false;
            playCamera_audioListener.enabled = false;

            cgCamera_camera.enabled = true;
            cgCamera_audioListener.enabled = true;
        }

        void ChangeToPlayerCamera()
        {
            cgCamera_camera.enabled = false;
            cgCamera_audioListener.enabled = false;

            playCamera_camera.enabled = true;
            playCamera_audioListener.enabled = true;
        }

        public void PlayDragonCG()
        {
            playerInputHandler.lockInput = true;
            playingTime = Time.time;
            isPlaying = true;
            isDragonCg = true;
            ChangeToCgCamera();
        }

        void StopCG()
        {
            playerInputHandler.lockInput = false;
            isPlaying = false;
        }

        void OrientTowards(Vector3 lookPosition)
        {
            //计算射线方向
            Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - player.transform.position, Vector3.up).normalized;
            if (lookDirection.sqrMagnitude != 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                player.transform.rotation =
                    Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * orientationSpeed);
            }
        }
    }
}
