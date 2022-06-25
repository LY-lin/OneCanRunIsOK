using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.GamePlay;
using OneCanRun.Game;
using UnityEngine.UI;

namespace OneCanRun.UI
{
    public class StandHud : MonoBehaviour
    {
        [Tooltip("Image component for the stance sprites")]
        public Image StanceImage;

        [Tooltip("Sprite to display when standing")]
        public Sprite StandingSprite;

        [Tooltip("Sprite to display when crouching")]
        public Sprite CrouchingSprite;

        void Start()
        {
            PlayerCharacterController character = FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, StandHud>(character, this);
            character.OnStanceChanged += OnStanceChanged;

            OnStanceChanged(character.IsCrouching);
        }

        void OnStanceChanged(bool crouched)
        {
            StanceImage.sprite = crouched ? CrouchingSprite : StandingSprite;
        }
    }
}