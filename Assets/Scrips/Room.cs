using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Room : MonoBehaviour
{
    public Text RoomName;
    [SerializeField]
    GameObject WarningText;
    [SerializeField]
    AudioSource audio;
    LobbySceneManager _lbm;
    RoomList _roomList;


    private void Start() {
        //初始化
        _lbm = GameObject.Find("LobbySceneManager").GetComponent<LobbySceneManager>();
        _roomList = GameObject.Find("RoomList").GetComponent<RoomList>();
        WarningText = GameObject.Find("Text_Warning");
        RoomName.text = this.gameObject.name;
    }

    public void SetRoomName(string _roomName){
        RoomName.text = this.gameObject.name;
    }

    public void JoinRoom(){
        string playerName = _lbm.GetPlayerName();
        PlayerPrefs.SetString("JoinRoomName",RoomName.text);
        if (playerName.Length > 0 )
        {//如果有輸入PlayerName，就加入這個按鈕名稱的房間，並把玩家名字設為PlayerName
            audio.Play();
            GameObject.Find("LobbySceneManager").GetComponent<LobbySceneManager>().JoinRoomList(RoomName.text);
            PhotonNetwork.LocalPlayer.NickName = playerName ;
        }else{//不然就顯示警告字串
            WarningText.SetActive(true);
            _lbm.SetWarningText("Plece enter PlayerName");
        }
    }
}