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
    public Text text_Confirm;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {//如果沒連上伺服器的話就回到Lobby場景
            SceneManager.LoadScene("Lobby");
        }else{//如果有連上伺服器就把房間名稱UI設為房間名稱，並刷新玩家列表
            textRoomName.text=PhotonNetwork.CurrentRoom.Name;
            UpDatePlayerList();
        }
        buttonStartGame.interactable=PhotonNetwork.IsMasterClient;//讓只有房主才能按Start按鈕
        //在GeneratPoint生成角色列表第0個角色
        GameObject Character = Instantiate(CharacterList[0],GeneratPoint.position,Quaternion.identity);
        Character.name = CharacterList[0].name;//將生成的角色名字改成跟角色列表一樣，防止出現culon等字樣，方便後續操作
        ReSetCharacter(Character);//將角色控制、數據等腳本刪除，防止報錯
        selectCharacterName = Character.name;//將所selectCharcterName設為當前所選角色名字
        text_Confirm.enabled = false;//將UI初始化
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {//當房主切換的時候
        buttonStartGame.interactable=PhotonNetwork.IsMasterClient;
    }

    public void UpDatePlayerList(){
        //一個一個印出PlayerList的玩家名字
        StringBuilder sb = new StringBuilder();
        foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            sb.AppendLine("> " + kvp.Value.NickName);
        }
        textPlayerList.text=sb.ToString();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {//當玩家加入房間時更新玩家列表
        UpDatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {//當玩家離開房間時更新玩家列表
        UpDatePlayerList();
        print("LeftRoom");
    }

    public void OnClickStart(){
        //按下Start按紐時
        SceneManager.LoadScene("Game");
    }

    public void OnClickLeaveRoom(){
        //按下LeaveRoom按鈕時
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftRoom()
    {//當離開房間時
        SceneManager.LoadScene("Lobby");
        //PhotonNetwork.Disconnect();
    }
//---------選角區--------
    public void AddCharaterIndex(){
        //當CharacterIndex在角色列表範圍內時，CharacterIndex+1
        if (CharacterIndex <= CharacterList.Count-2)
        {
            CharacterIndex ++;
        }else{
            CharacterIndex = 0;
        }
        text_Confirm.enabled = false;
        UpdateCharacter();//刷新角色預覽
    }

    public void ReduceCharacterIndex(){
        //當CharacterIndex在角色列表範圍內時，CharacterIndex-1
        if (CharacterIndex >0)
        {
            CharacterIndex--;
        }else{
            CharacterIndex = CharacterList.Count-1;
        }
        text_Confirm.enabled = false;
        UpdateCharacter();//刷新角色預覽
    }

    public void ReSetCharacter(GameObject gameObject){
        //刪除角色物件中可能報錯的腳本
        Destroy(gameObject.GetComponent<PlayerContaller>());
        Destroy(gameObject.GetComponent<PlayerData>());
        Destroy(gameObject.GetComponent<PlayerInput>());
    }

    public void UpdateCharacter(){
        //刷新角色預覽
        Destroy(GameObject.FindGameObjectWithTag("Player"));//刪除原本的角色物件
        //依照CharacterIndex生成新的角色物件
        GameObject Character = Instantiate(CharacterList[CharacterIndex],GeneratPoint.position,Quaternion.identity);
        //將生成的角色物件名字改成跟角色列表的一樣，預防clone等字出現，方便之後操作
        Character.name = CharacterList[CharacterIndex].name;
        //重製角色物件
        ReSetCharacter(Character);
        //將所selectCharcterName設為當前所選角色名字
        selectCharacterName = Character.name;
    }

    public void ConfirmCharacter(){
        //確認所選角色
        PlayerPrefs.SetString("CharacterName",selectCharacterName);
        text_Confirm.enabled = true;
    }
}
