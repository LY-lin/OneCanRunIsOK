using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game{

    public class SetVolume : MonoBehaviour{

        public UnityEngine.Audio.AudioMixer audioMixer;     
        
        public void setLevel(float sliderValue){

            audioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue)*20);

        }
    }
}
