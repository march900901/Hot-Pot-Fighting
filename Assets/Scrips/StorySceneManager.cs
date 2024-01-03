using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StorySceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected == false)
        {//如果沒連上伺服器的話就回到Lobby場景
            SceneManager.LoadScene("Start");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnKlickOK(){
        SceneManager.LoadScene("Lobby");
    }
}
