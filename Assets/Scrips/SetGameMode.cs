using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SetGameMode : MonoBehaviour
{
    public Text GameTimeText;
    public int GameMode;
    public int GameTime;
    void Start()
    {
        GameMode = 0;
        GameTime = 60;
    }

    public void SetTimeMode(){
        PlayerPrefs.SetInt("GameMode",0);
        GameMode = 0;
    }

    public void SetLifeMode(){
        PlayerPrefs.SetInt("GameMode",1);
        GameMode = 1;
    }

    public void SetGameTime(){
        string timeString = GameTimeText.text.ToString();
        if (timeString != "")
        {
            GameTime = int.Parse(timeString);
        }else
        {
            GameTime = 60;
        }
        PlayerPrefs.SetInt("GameTime",GameTime);
    }

}
