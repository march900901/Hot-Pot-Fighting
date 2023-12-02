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

public class GameManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> PlayerList = new List<GameObject>();
    public List<Transform> startTransform = new List<Transform>();
    [SerializeField]TextMeshProUGUI P1pointText;
    [SerializeField]TextMeshProUGUI P2pointText;
    public List<GameObject> deadPlayer = new List<GameObject>();
    public GameObject P1;
    public GameObject P2;
    public Transform P1Start;
    public Transform P2Start;
    PhotonView _pv;
    [SerializeField]
    List<string> messageList;
    [SerializeField]
    Text messageText;
    public Dictionary<Player, bool> alivePlayerMap = new Dictionary<Player, bool>();

    public int P1point=0;
    public int P2point=0;
    // Start is called before the first frame update
    void Start()
    {
        _pv = this.transform.GetComponent<PhotonView>();
        if (PhotonNetwork.CurrentRoom==null)
        {
            SceneManager.LoadScene("Lobby");
        }else{
            
        }
        //SponPlayer();
        InisGame();
    }

    // Update is called once per frame
    void Update()
    {   //計分
        P1pointText.text=P1point.ToString();
        P2pointText.text=P2point.ToString();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        alivePlayerMap[newPlayer] = true;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (alivePlayerMap.ContainsKey(otherPlayer))
        {
            alivePlayerMap.Remove(otherPlayer);
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LObby");
    }

    public void InisGame(){
        foreach(var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            alivePlayerMap[kvp.Value] = true;
        }
        int randomNum=0;
        List<Transform> startPions=new List<Transform>();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {//隨機取生成點
            randomNum=Random.Range(0,startTransform.Count);
            startPions.Add(startTransform[randomNum]);
            //print(startPions[i].gameObject.name);
        }
        PhotonNetwork.Instantiate("P1",startPions[Random.Range(0,startPions.Count)].position,Quaternion.identity);
        // if (PlayerList.Count!=0)
        // {
        //     for (int i = 0; i < PlayerList.Count; i++)
        //     {
        //         randomNum=Random.Range(0,startTransform.Count);
        //         //GameObject sponObj = Instantiate(PlayerList[i],startTransform[randomNum].position,Quaternion.identity);
        //         //sponObj.name=PlayerList[i].name;
                
        //     }
        // }else{

        // }
    }

    public void SponPlayer(){
        //遊戲開始時生成玩家在遊戲場景
        int randomNum=0;
        if (PlayerList.Count!=0)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                randomNum=Random.Range(0,startTransform.Count);
                GameObject sponObj = Instantiate(PlayerList[i],startTransform[randomNum].position,Quaternion.identity);
                sponObj.name=PlayerList[i].name;
            }
        }else{

        }
    }

    public void revivalPlayer(string revivalPlayerName){
        int randomNum=0;
        if (deadPlayer.Count!=0)
        {
            for (int i = 0; i < deadPlayer.Count; i++)
            {
                randomNum=Random.Range(0,startTransform.Count);
                GameObject revivalObj = Instantiate(deadPlayer[i],startTransform[randomNum].position,Quaternion.identity);
                revivalObj.name=revivalPlayerName;
                deadPlayer.RemoveAt(i);
            }
        }else{

        }
    }

    public void CountPoint(string PlayerName,int point){
        if (PlayerName=="P1")
        {
            P2point+=point;   
        }
        else if (PlayerName=="P2")
        {
            P1point+=point;
        }
        else{
            
        }
    }

    public void CallRpcPlayerDead(){
        _pv.RPC("RpcPlayerDead",RpcTarget.All);
    }
    [PunRPC]
    void RpcPlayerDead(PhotonMessageInfo info){
        if(alivePlayerMap.ContainsKey(info.Sender)){
            alivePlayerMap[info.Sender] = false;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            //檢查玩家存活狀態，顯示結果或轉換場景
        }
    }

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
}
