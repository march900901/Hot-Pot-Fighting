using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.IO;
using UnityEngine.UI;
using Photon.Realtime;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> PlayerList = new List<GameObject>();
    public List<Transform> startTransform = new List<Transform>();
    public GameObject DeadPlayer;
    PhotonView _pv;
    [SerializeField]
    List<string> messageList;
    [SerializeField]
    Text messageText;
    public Dictionary<Player, bool> alivePlayerMap = new Dictionary<Player, bool>();
    public string CharacterName;
    // Start is called before the first frame update
    void Start()
    {
        CharacterName = PlayerPrefs.GetString("CharacterName");//取得玩家選擇的角色名
        _pv = this.transform.GetComponent<PhotonView>();
        if (PhotonNetwork.CurrentRoom==null)//如果運行時沒有創建的房間，就會轉到Lobby場景
        {
            SceneManager.LoadScene("Lobby");
        }
        InisGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {//玩家加入房間的時候，把玩家加入alivePlayerMap裡，並設為true
        alivePlayerMap[newPlayer] = true;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (alivePlayerMap.ContainsKey(otherPlayer))
        {
            alivePlayerMap.Remove(otherPlayer);
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {//當玩家離開房間時，檢查房間玩家人數，如果<=1就離開房間
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {//離開房間的時候將場景換到Lobby
        SceneManager.LoadScene("LObby");
    }

    public void InisGame(){
        foreach(var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            alivePlayerMap[kvp.Value] = true;
        }
        //--------初始隨機生成玩家-------
        SponPlayer(CharacterName);
    }

    public void SponPlayer(string _caracterName){
        //遊戲開始時生成玩家在遊戲場景
        int randomNum=0;
        List<Transform> startPions=new List<Transform>();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {//隨機取生成點
            randomNum=Random.Range(0,startTransform.Count);
            startPions.Add(startTransform[randomNum]);
        }
        //在所有玩家的畫面中生成玩家物件
        GameObject Player = PhotonNetwork.Instantiate(_caracterName,startPions[Random.Range(0,startPions.Count)].position,Quaternion.identity);
        Player.name = _caracterName.ToString(); 
    }

    // public void revivalPlayer(string PlayerName){
    //     int randomNum=0;
    //     randomNum=Random.Range(0,startTransform.Count);
    //     PhotonNetwork.Instantiate(CharacterName,startPions[Random.Range(0,startPions.Count)].position,Quaternion.identity);
    //     revivalObj.name=PlayerName;
    // }
    //-------玩家死亡-------
    public void CallRpcPlayerDead(){
        //用RPC呼叫所有玩家場景中的"RpcPlayerDead"方法
        _pv.RPC("RpcPlayerDead",RpcTarget.All);
    }
    [PunRPC]
    void RpcPlayerDead(PhotonMessageInfo info){
        //執行玩家死亡
        if (DeadPlayer)
        {
            SponPlayer(DeadPlayer.name);
            PhotonNetwork.Destroy(DeadPlayer);
            
        }

        // if(alivePlayerMap.ContainsKey(info.Sender)){
        //     alivePlayerMap[info.Sender] = false;
        // }
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     //檢查玩家存活狀態，顯示結果或轉換場景
        // }
    }
//-------用Rpc向其他玩家發送訊息-------
    public void CallRpcSendMessageToAll(string message){//用photon的RPC功能發送訊息
        _pv.RPC("RpcSendMessage",RpcTarget.All,message);//對所有人發送"RpcSendMessage"，呼叫RpcSendMessage這個方法
    }

    [PunRPC]
    void RpcSendMessage(string message,PhotonMessageInfo info){//RPC接到訊息時會收到一個字串訊息和一個傳輸者資訊
        if (messageList.Count >= 10)
        {//如果訊息列表超過10行就把第一行刪除
            messageList.RemoveAt(0);
        }
        messageList.Add(message);//將RPC收到的訊息加入訊息列表
        UpdateMessage();
    }

    void UpdateMessage(){//更新messageList的訊息到message
        messageText.text = string.Join("\n",messageList);
    }
//-------離開遊戲-------
    public void OnClickLeaveGame(){
        PhotonNetwork.LeaveRoom();
    }
}
