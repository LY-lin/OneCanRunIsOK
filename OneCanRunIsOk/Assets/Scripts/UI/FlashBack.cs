using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using OneCanRun.GamePlay;
using UnityEngine.UI;
public class FlashBack : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("References")]
    [Tooltip("Image component of the flash")]
    public Image FlashImage;

    [Tooltip("CanvasGroup to fade the damage flash, used when recieving damage end healing")]
    public CanvasGroup FlashCanvasGroup;

    /*[Tooltip("CanvasGroup to fade the critical health vignette")]
    public CanvasGroup VignetteCanvasGroup;*/

    [Header("Damage")]
    [Tooltip("Color of the damage flash")]
    public Color DamageFlashColor;

    [Tooltip("Duration of the damage flash")]
    public float DamageFlashDuration;

    [Tooltip("Max alpha of the damage flash")]
    public float DamageFlashMaxAlpha = 1f;


    [Header("Heal")]
    [Tooltip("Color of the heal flash")]
    public Color HealFlashColor;

    [Tooltip("Duration of the heal flash")]
    public float HealFlashDuration;

    [Tooltip("Max alpha of the heal flash")]
    public float HealFlashMaxAlpha = 1f;

    bool m_FlashActive;
    float m_LastTimeFlashStarted = Mathf.NegativeInfinity;
    Health m_PlayerHealth;
    GameFlowManager m_GameFlowManager;

    private float FlashDuration;

    void Start()
    {
        // Subscribe to player damage events
        PlayerCharacterController playerCharacterController = FindObjectOfType<PlayerCharacterController>();
        DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, FlashBack>(
            playerCharacterController, this);

        m_PlayerHealth = playerCharacterController.GetComponent<Health>();
        DebugUtility.HandleErrorIfNullGetComponent<Health, FlashBack>(m_PlayerHealth, this,
            playerCharacterController.gameObject);

        m_GameFlowManager = FindObjectOfType<GameFlowManager>();
        DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, FlashBack>(m_GameFlowManager, this);

        m_PlayerHealth.OnDamaged += OnTakeDamage;
        m_PlayerHealth.OnHealed += OnHealed;
        FlashCanvasGroup.alpha = 0;
        FlashDuration = DamageFlashDuration;
    }

    void Update()
    {
        /*if (m_PlayerHealth.IsCritical())
        {
            VignetteCanvasGroup.gameObject.SetActive(true);
            float vignetteAlpha =
                (1 - (m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth /
                      m_PlayerHealth.CriticalHealthRatio)) * CriticaHealthVignetteMaxAlpha;

            /*if (m_GameFlowManager.GameIsEnding)
                VignetteCanvasGroup.alpha = vignetteAlpha;
            else
                VignetteCanvasGroup.alpha =
                    ((Mathf.Sin(Time.time * PulsatingVignetteFrequency) / 2) + 0.5f) * vignetteAlpha;*/
       /* }
        else
        {
            //VignetteCanvasGroup.gameObject.SetActive(false);
        }*/


        if (m_FlashActive)
        {
            float normalizedTimeSinceDamage = (Time.time - m_LastTimeFlashStarted) / FlashDuration;

            if (normalizedTimeSinceDamage < 1f)
            {
                float flashAmount = DamageFlashMaxAlpha * (1f - normalizedTimeSinceDamage);
                FlashCanvasGroup.alpha = flashAmount;
            }
            else
            {
                FlashCanvasGroup.gameObject.SetActive(false);
                m_FlashActive = false;
            }
        }
    }

    void ResetFlash()
    {
        m_LastTimeFlashStarted = Time.time;
        m_FlashActive = true;
        FlashCanvasGroup.alpha = 0f;
        FlashCanvasGroup.gameObject.SetActive(true);
    }

    void OnTakeDamage(float dmg, GameObject damageSource)
    {
        FlashDuration = DamageFlashDuration;

        ResetFlash();
        FlashImage.color = DamageFlashColor;
        
    }

    void OnHealed(float amount)
    {
        FlashDuration = HealFlashDuration;
        ResetFlash();
        FlashImage.color = HealFlashColor;
        
    }
}

