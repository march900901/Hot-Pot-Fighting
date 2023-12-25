using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float reminingTime;
    public GameManager _gm;

    void Start() {
        _gm= GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Update() {
        if (reminingTime>0)
        {
            reminingTime -= Time.deltaTime;
        }else if(reminingTime<0){
            reminingTime = 0;
            timerText.color = Color.red;
            _gm.JudgeGameOver();
        }
        
        int minutes = Mathf.FloorToInt(reminingTime / 60);
        int seconds = Mathf.FloorToInt(reminingTime  % 60);
        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds); 
    }
}
