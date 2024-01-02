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
using DG.Tweening;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameRull{TIME,LIVE,FINAL}
    public GameRull _gr;
    public int PlayerCount;
    public int Life;
    public int GameTime;
    public List<Transform> startTransform = new List<Transform>();
    public List<PlayerData> players = new List<PlayerData>();
    PhotonView _pv;
    [SerializeField]List<string> messageList;
    [SerializeField]Text messageText;
    public Dictionary<Player, bool> alivePlayerMap = new Dictionary<Player, bool>();
    public string CharacterName;
    public GameObject GameOverPanlel;
    public GameObject BT_Leav;
    public AudioManager _am;
    public AudioSource Leav;
    public AudioSource StartGame;
    public AudioSource BGM;
    public DoTween instruction;
    public GameObject Timer;
    // Start is called before the first frame update
    void Start()
    {
        _gr = (GameRull)PlayerPrefs.GetInt("GameMode");                                                                                                                                               
        CharacterName = PlayerPrefs.GetString("CharacterName");//取得玩家選擇的角色名
        _pv = this.transform.GetComponent<PhotonView>();
        if (PhotonNetwork.CurrentRoom==null)//如果運行時沒有創建的房間，就會轉到Lobby場景
        {
            SceneManager.LoadScene("Lobby");
        }
        PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        //instruction.gameObject.SetActive(true);
        //instruction.PanelIn();
        GameOverPanlel.SetActive(false);
        InisGame();
        PhotonNetwork.SendRate = 1000;
        PhotonNetwork.SerializationRate = 1000;
        print("SendRate: " + PhotonNetwork.SendRate);
        print("SerializaationRate: " + PhotonNetwork.SerializationRate);
        //StartGame.PlayDelayed(1.5f);
        _am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Timer = GameObject.Find("Timer");
        StartGame.Play();
        BT_Leav.transform.localScale = Vector3.zero;
        switch (_gr)
        {
            case GameRull.TIME:
                Timer.SetActive(true);
                Timer.GetComponent<Timer>().reminingTime = GameTime;
            break;

            case GameRull.LIVE:
                Timer.SetActive(false);
            break;

            case GameRull.FINAL:
                Timer.SetActive(true);
                Timer.GetComponent<Timer>().reminingTime = 60;
            break;
        }
        
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
        SceneManager.LoadScene("Lobby");
    }

    public void InisGame(){
        foreach(var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            alivePlayerMap[kvp.Value] = true;
        }
        //--------初始隨機生成玩家-------
        SponPlayer(CharacterName);
    }
//-------初始生成玩家------
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
        GameObject Player = PhotonNetwork.Instantiate(_caracterName,startPions[Random.Range(0,startPions.Count)].position,new Quaternion(0,180,0,0));
        Player.name =  _caracterName.ToString();
        ReSetPlayerData(Player.GetComponent<PlayerData>());
        print("Spoon");
    }

//-------玩家復活-------
    public void ReSetPlayer(GameObject Player){
        int randomNum=0;
        List<Transform> startPions=new List<Transform>();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {//隨機取生成點
            randomNum=Random.Range(0,startTransform.Count);
            startPions.Add(startTransform[randomNum]);
        }
        randomNum = Random.Range(0,startPions.Count);
        // GameObject Player = GameObject.Find(PlayerName);
        //Player.GetComponent<PlayerData>()._playerState = PlayerData.PlayerState.Idle;
        Player.transform.position =startPions[randomNum].transform.position;
        Player.transform.rotation = Quaternion.Euler(0,180,0);
        ReSetPlayerData(Player.GetComponent<PlayerData>());
        //Player.name = PlayerName;
        
        print(Player.name +" revival");
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
    public void OnClickLeaveGame(){//按下離開按鈕
        //Leav.Play();
        foreach (PlayerData item in players)
        {
            if(item._pv.IsMine){
                PlayerPrefs.SetString("PlayerName",item.nameText.text);
            }
        }
        _am.PlayAudio(4);
        SceneManager.LoadScene("selectCharacter");
        //PhotonNetwork.LeaveRoom();
    }

//-------遊戲結束-------
    public void GameOver(){//執行遊戲結束
        GameOverPanlel.SetActive(true);
        _am.PlayAudio(4);
        BGM.Stop();
        print("GameOver");
    }

    public void CallRpcGameOver(){//呼叫RPC執行遊戲結束
        _pv.RPC("RpcGameOver",RpcTarget.All);
    }

    [PunRPC]
    void RpcGameOver(PhotonMessageInfo info){//RPC執行遊戲結束
        GameOver();
    }
//-------設定贏家名字及角色-------
    public void SetWinerName(string winerName,string winerObj){
        PlayerPrefs.SetString("WinerName",winerName);
        PlayerPrefs.SetString("WinerObj",winerObj);
        CallRpcGameOver();
    }

    public void CallRpcSetWinerName(string winerName,string winerObj){//呼叫RPC傳送贏家名字
        _pv.RPC("RpcSetWinerName",RpcTarget.All,winerName,winerObj);
    }

    [PunRPC]
    void RpcSetWinerName(string winerName,string winerObj,PhotonMessageInfo info){//RPC執行贏家名字
        PlayerPrefs.SetString("WinerName",winerName);
        PlayerPrefs.SetString("WinerObj",winerObj);
        GameOver();
    }

//-------重設玩家數值-------
    public void ReSetPlayerData(PlayerData player){

        player.enemy = null;
        player._playerState = PlayerData.PlayerState.Idle;
        //player.throwMe = null;
        player.Lifting = false;
        player.CanLift = false;
    }

//-------判斷遊戲結束
    public void JudgeGameOver(){
        switch(_gr){
            case GameRull.TIME:
                List<PlayerData> winDatas = new List<PlayerData>();
                winDatas.Add(players[0]);
                foreach (PlayerData item in players)
                {
                    print(item.Point);
                }

                for (int i = 1; i < players.Count; i++)
                {
                    if (winDatas[0].Point < players[i].Point)
                    {
                        if (winDatas.Count==1)
                        {
                            winDatas[0] = players[i];
                        }else{
                            winDatas.Clear();
                            winDatas.Add(players[i]);
                        }
                    }else if(winDatas[0].Point == players[i].Point){
                        winDatas.Add(players[i]);
                    }
                }
                if (winDatas.Count == 1)
                {
                    CallRpcSetWinerName(winDatas[0].nameText.text,winDatas[0].gameObject.name);
                }else{
                    _gr = GameRull.FINAL;
                    List<string> winName = new List<string>();
                    foreach (var item in winDatas)
                    {//將分數最高的玩家物件名加入list
                        winName.Add(item.gameObject.name);
                    }
                    foreach (var item in winDatas)
                    {//將分數最高的玩家data從players列表移除
                        players.Remove(item);
                    }
                    foreach (var item in players)
                    {
                        Destroy(item.gameObject);
                    }
                    foreach (var item in winDatas)
                    {//生成分數最高的玩家
                        item._gm.ReSetPlayer(item.gameObject);
                    }
                }
                
            break;

            case GameRull.LIVE:
                if (PlayerCount <= 1)
                {
                    print("Judge!!");
                    GameObject winer = GameObject.FindWithTag("Player");
                    string winerName = winer.GetComponent<PlayerData>().nameText.text;
                    string winerobj = winer.gameObject.name;
                    CallRpcSetWinerName(winerName,winerobj);
                }
            break;

            case GameRull.FINAL:

            break;
            
        }
    }

}
