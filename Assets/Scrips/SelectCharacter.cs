using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using Photon.Realtime;

public class SelectCharacterManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Text textRoomName;
    [SerializeField]
    Text textPlayerList;
    [SerializeField]
    Button buttonStartGame;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            SceneManager.LoadScene("Lobby");
        }else{
            textRoomName.text=PhotonNetwork.CurrentRoom.Name;
            UpDatePlayerList();
        }
        buttonStartGame.interactable=PhotonNetwork.IsMasterClient;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        buttonStartGame.interactable=PhotonNetwork.IsMasterClient;
    }

    public void UpDatePlayerList(){
        StringBuilder sb = new StringBuilder();
        foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            sb.AppendLine("> " + kvp.Value.NickName);
        }
        textPlayerList.text=sb.ToString();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpDatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpDatePlayerList();
    }

    public void OnClickStart(){
        SceneManager.LoadScene("Game");
    }

    public void OnClickLeaveRoom(){
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
}
