using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Room : MonoBehaviour
{
    public Text name;
    [SerializeField]
    GameObject WarningText;
    LobbySceneManager _lbm;

    private void Start() {
        _lbm = GameObject.Find("LobbySceneManager").GetComponent<LobbySceneManager>();
        WarningText = GameObject.Find("WarningText");
    }

    public void JoinRoom(){
        string playerName = _lbm.GetPlayerName();
        //string roomName = _lbm.GetRoomName();
        if (playerName.Length > 0 )
        {
            GameObject.Find("LobbySceneManager").GetComponent<LobbySceneManager>().JoinRoomList(name.text);
            PhotonNetwork.LocalPlayer.NickName = playerName ;
        }else{
            WarningText.SetActive(true);
            _lbm.SetWarningText("Plece enter PlayerName");
            print("Plece enter PlayerName");
        }
    }   
}
