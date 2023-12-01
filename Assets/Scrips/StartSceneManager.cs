using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StartSceneManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    
    public void GoToGameScene(){
        //移到遊戲場景
        SceneManager.LoadScene("Game");
    }

    public void OnClickStart(){
        PhotonNetwork.AutomaticallySyncScene=true;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Click Start");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conneted!");
        SceneManager.LoadScene("Lobby");
    }
}
