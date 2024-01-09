using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Final : MonoBehaviour
{
    public GameManager _gm;
    public PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void GoFinal(){
        playerInput.enabled = false;
        
    }

    public void StartFinal(){
        _gm._gr = GameManager.GameRull.FINAL;
        playerInput.enabled = true;
    }
}
