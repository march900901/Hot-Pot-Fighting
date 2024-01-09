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
        playerInput.SwitchCurrentActionMap("NotMe");
        
    }

    public void StartFinal(){
        playerInput.SwitchCurrentActionMap("Player1");
        _gm._gr = GameManager.GameRull.FINAL;
    }
}
