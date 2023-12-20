using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StartSceneManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject LoadingPanle;

    private void Start() {
        LoadingPanle.SetActive(false);
    }
    public void GoToGameScene(){
        //移到遊戲場景
        SceneManager.LoadScene("Game");
    }

    public void OnClickStart(){
        PhotonNetwork.AutomaticallySyncScene=true;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Click Start");
        LoadingPanle.SetActive(true);
        LoadingPanle.GetComponent<DoTween>().ScalePanel();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conneted!");
        SceneManager.LoadScene("Lobby");
    }
}
