using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// for display value in UI
public class PropertyPlane : MonoBehaviour{
    private GameObject plane;
    private string planeName;

    // UI GameObject
    private GameObject addButton;
    private GameObject subtractButton;
    private GameObject pointInfo;
    private OneCanRun.Game.Share.ActorConfig config;

    public void init(GameObject planeGameObject, string _planeName){
        plane = planeGameObject;
        planeName = _planeName;
        //addButton = this.gameObject.GetComponentInChildren<TMPro.>
    }

    private void Start(){
        //addButton
        
    }

}
