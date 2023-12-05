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
    LobbySceneManager _lbm;

    private void Start() {
        //初始化
        _lbm = GameObject.Find("LobbySceneManager").GetComponent<LobbySceneManager>();
        WarningText = GameObject.Find("WarningText");
        RoomName.text = this.gameObject.name;
    }

    public void JoinRoom(){
        string playerName = _lbm.GetPlayerName();
        if (playerName.Length > 0 )
        {//如果有輸入PlayerName，就加入這個按鈕名稱的房間，並把玩家名字設為PlayerName
            GameObject.Find("LobbySceneManager").GetComponent<LobbySceneManager>().JoinRoomList(RoomName.text);
            PhotonNetwork.LocalPlayer.NickName = playerName ;
        }else{//不然就顯示警告字串
            WarningText.SetActive(true);
            _lbm.SetWarningText("Plece enter PlayerName");
        }
    }   
}