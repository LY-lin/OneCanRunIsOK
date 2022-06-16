using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Backpack : MonoBehaviour
{
    [Tooltip("Image for E Skill")]
    public Image eSkill;

    [Tooltip("Image for Q Skill")]
    public Image qSkill;

    [Tooltip("Weapon 1 name")]
    public TextMeshProUGUI Weapon1Name;

    [Tooltip("text for Descriping weapon 1")]
    public TextMeshProUGUI txtWeapon1;

    [Tooltip("Weapon 2 name")]
    public TextMeshProUGUI Weapon2Name;

    [Tooltip("text for Descriping weapon 2")]
    public TextMeshProUGUI txtWeapon2;

    [Tooltip("Patient value")]
    public TextMeshProUGUI patient;

    [Tooltip("Power value")]
    public TextMeshProUGUI power;

    [Tooltip("Intelligence")]
    public TextMeshProUGUI intelligence;

    [Tooltip("Physical damge")]
    public TextMeshProUGUI physicalDamage;

    [Tooltip("Magical damge")]
    public TextMeshProUGUI magicalDamage;

    [Tooltip("Physical Defence")]
    public TextMeshProUGUI physicalDefence;

    [Tooltip("Magical Denfence")]
    public TextMeshProUGUI magicalDefence;

    [Tooltip("speed value")]
    public TextMeshProUGUI speed;

    [Tooltip("Recovery value")]
    public TextMeshProUGUI recouvery;

    [Tooltip("")]
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
