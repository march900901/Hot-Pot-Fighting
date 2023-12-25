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
        
    }

    public void PreviousMode(){
        
    }

}
