using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using System.Text;
using UnityEngine.SceneManagement;

public class LobbySceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    InputField inputRoomName;
    [SerializeField]
    InputField inputPlayerName;
    [SerializeField]
    Text textRoomList;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected==false)
        {
            SceneManager.LoadScene("Start");
        }else{
            if (PhotonNetwork.CurrentLobby==null)
            {
                PhotonNetwork.JoinLobby();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        print("Cenneted to Master!!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined!");
    }

    public string GetRoomName(){
        string roomName=inputRoomName.text;
        return roomName.Trim();
    }

    public string GetPlayerName(){
        string PlayerName=inputPlayerName.text;
        return PlayerName.Trim();
    }

    public void OnClickCreatRoom(){
        string roomName=GetRoomName();
        string PlayerName=GetPlayerName();
        if (roomName.Length>0 && PlayerName.Length>0)
        {
            PhotonNetwork.CreateRoom(roomName);
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
        }else{
            Debug.Log("Please enter Room name and Player name");
        }
    }

    public void OnClickJoinRoom(){
        string roomName = GetRoomName();
        string PlayerName=GetPlayerName();
        if (roomName.Length > 0 && PlayerName.Length>0)
        {
            PhotonNetwork.JoinRoom(roomName);
            PhotonNetwork.LocalPlayer.NickName=PlayerName;
        }else{
            Debug.Log("Please enter Room name and Player name");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined!");
        SceneManager.LoadScene("SelectCharacter");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        StringBuilder sb = new StringBuilder();
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.PlayerCount>0)
            {
                sb.AppendLine("> "+roomInfo.Name);
            }
        }
        textRoomList.text = sb.ToString();
    }
}
