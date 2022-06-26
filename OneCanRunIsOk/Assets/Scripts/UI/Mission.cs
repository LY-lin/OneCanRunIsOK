using UnityEngine.UI;
using UnityEngine;
using TMPro;
using OneCanRun.GamePlay;

public class Mission : MonoBehaviour
{
    // 当前波次
    public int waveNum = 0;

    [Tooltip("波次提示")]
    public TextMeshProUGUI MissionHint;

    private MonsterFresh monsterFresh;

    void updateWaveNum()
    {
        waveNum++;
    }


    // Start is called before the first frame update
    void Start()
    {
        monsterFresh = GameObject.Find("God").GetComponent<MonsterFresh>();
        monsterFresh.newWave += updateWaveNum;
    }

    // Update is called once per frame
    void Update()
    {
        MissionHint.text = "The current wave of enemies: " + waveNum;
    }
}
