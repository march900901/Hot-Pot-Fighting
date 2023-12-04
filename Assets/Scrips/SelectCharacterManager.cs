using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class SelectCharacterManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Text textRoomName;
    [SerializeField]
    Text textPlayerList;
    [SerializeField]
    Button buttonStartGame;
    public int CharacterIndex = 0;
    public List<GameObject> CharacterList = new List<GameObject>();
    public Transform GeneratPoint;
    public string selectCharacterName;
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
        GameObject Character = Instantiate(CharacterList[0],GeneratPoint.position,Quaternion.identity);
        Character.name = CharacterList[0].name;
        ReSetCharacter(Character);
        selectCharacterName = Character.name;
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
        PlayerPrefs.SetString("CharacterName",selectCharacterName);
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
//---------選角區--------
    public void AddCharaterIndex(){
        if (CharacterIndex <= CharacterList.Count-2)
        {
            CharacterIndex ++;
        }else{
            CharacterIndex = 0;
        }
        UpdateCharacter();
    }

    public void ReduceCharacterIndex(){
        if (CharacterIndex >0)
        {
            CharacterIndex--;
        }else{
            CharacterIndex = CharacterList.Count-1;
        }
        UpdateCharacter();
    }

    public void ReSetCharacter(GameObject gameObject){
        Destroy(gameObject.GetComponent<PlayerContaller>());
        Destroy(gameObject.GetComponent<PlayerData>());
        Destroy(gameObject.GetComponent<PlayerInput>());
    }

    public void UpdateCharacter(){
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        GameObject Character = Instantiate(CharacterList[CharacterIndex],GeneratPoint.position,Quaternion.identity);
        Character.name = CharacterList[CharacterIndex].name;
        ReSetCharacter(Character);
        selectCharacterName = Character.name;
    }

    public void ConfirmCharacter(){
        PlayerPrefs.SetString("CharacterName",selectCharacterName);
        
    }
}
