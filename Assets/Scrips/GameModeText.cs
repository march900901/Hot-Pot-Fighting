using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeText : MonoBehaviour
{
    public Text GameMode_Text;
    public int index;
    public List<string> GameMode = new List<string>();

    void Update() {
        GameMode_Text.text = GameMode[index];
    }
    
    public void NextMode(){
        if (index < GameMode.Count-1)
        {
            index++;
        }else{index = 0;}
    }

    public void PreviousMode(){
        if (index > 0)
        {
            index--;
        }else if(index <= 0){
            index = GameMode.Count-1;
        }
    }

}
