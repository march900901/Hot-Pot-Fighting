using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoGameOver : MonoBehaviour
{
    public void GoGameOverScene(){
        SceneManager.LoadScene("GameOver");
    }
}
