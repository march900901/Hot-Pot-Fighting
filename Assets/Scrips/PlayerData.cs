using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using hashTable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;
using System.Text.RegularExpressions;
using System;


public class PlayerData : MonoBehaviourPunCallbacks
{
    public enum PlayerState{Idle,Dash,Fly,Lift,BeLift,Dead,CantMove}
    [Header("狀態")]
    public PlayerState _playerState;
    [Header("名稱")]
    public string Name;
    [SerializeField]public Text nameText;
    [Header("變數")]
    public int Point = 3;
    public int scapeCount=0;
    public bool Lifting=false;
    public bool CanLift;
    public string defaultMap;
    public Text PointText;
    [Header("物件")]
    public GameObject scapeEffect;
    public GameObject enemy;
    public GameObject throwMe;
    public GameObject Star;
    [Header("粒子特效")]
    public  ParticleSystem HitEffect;
    [Header("聲音")]
    public AudioSource hit;
    public AudioSource ScapeAudio;
    [Header("其他")]
    public Color DefaultColor;
    public LiftCollider LiftPoint;
    public PhotonView _pv;
    public GameManager _gm;
    public Final final;
    PlayerContaller playerContaller;
    Rigidbody rigidbody;
    PlayerInput playerInput;    
    
    hashTable table = new hashTable();
    
    
    

    


    // Start is called before the first frame update
    void Start()
    {//初始化
        
        _playerState=PlayerState.Idle;
        playerContaller=this.transform.GetComponent<PlayerContaller>();
        playerInput=this.transform.GetComponent<PlayerInput>();
        defaultMap=playerInput.defaultActionMap;
        rigidbody=this.transform.GetComponent<Rigidbody>();
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>(); 
        _gm.players.Add(GetComponent<PlayerData>());
        _pv = this.transform.GetComponent<PhotonView>();      
        nameText.text = _pv.Owner.NickName;
        final = GameObject.FindWithTag("Final").GetComponent<Final>();
        if (this.gameObject.name.EndsWith("(Clone)"))
        {
            Name = this.gameObject.name.TrimEnd("(Clone)");
            this.gameObject.name = Name;
        }else{Name = this.gameObject.name;}

        switch(_gm._gr){
            case GameManager.GameRull.TIME:
                Point = 0;
                //_gm.players.Add(this.gameObject.GetComponent<PlayerData>());
                // print("Add PlayerPoint");
                // foreach (var kvp in _gm.PlayerPoint)
                // {
                //     print(kvp.Key);
                // }
            break;

            case GameManager.GameRull.LIVE:
                Point = _gm.Life;
                if(_pv.IsMine){
                    _gm._pointUI.Life = Point;
                }
            break;
        }
        if (_pv.IsMine)
        {
            _gm.ui_PlayerName.text = nameText.text;
            _gm.MyCharacter = this.gameObject;
            final.playerInput = this.transform.GetComponent<PlayerInput>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        switch (_playerState)
        {
            case PlayerState.Idle:
                //狀態Idle時，將輸入設為預設Map角色顏色設為預設
                scapeCount=0;
                this.transform.localScale = new Vector3(1,1,1);
                playerInput.SwitchCurrentActionMap(defaultMap);
                this.gameObject.GetComponent<MeshRenderer>().material.color=DefaultColor;
                LiftPoint.UpdateCanLift(CanLift);
                Star.active = false;
                break;

            case PlayerState.CantMove:
                //進入不可動狀態，物件變白，操作的MAP改成CD
                this.gameObject.GetComponent<MeshRenderer>().material.color=Color.white;
                if (_pv.IsMine)
                {
                    playerInput.SwitchCurrentActionMap("CD");
                }
                LiftPoint.UpdateCanLift(true);
                Lifting = false;
                Star.active = true;
                playerContaller.StopAllCoroutines();
                if (scapeCount>=10)
                {
                    print(this.gameObject.name);
                    GameObject ScapeParticle = Instantiate(scapeEffect,this.transform);
                    ScapeAudio.Play();
                    ScapeParticle.transform.parent = null;
                    Destroy(ScapeParticle,5);
                    //如果達成逃脫條件，變回IDLE狀態，並從子物件中移出，再取消Kinematic
                    _playerState=PlayerState.Idle;
                    if (enemy && enemy.GetComponent<PlayerData>()._playerState == PlayerState.Lift)
                    {
                        enemy.GetComponent<PlayerData>()._playerState=PlayerState.Idle;
                        playerContaller.ScapeJumpe();
                        enemy=null;
                    }
                    
                    this.transform.parent=null;
                    rigidbody.isKinematic=false;
                }
                break;

            case PlayerState.Dash:
                LiftPoint.UpdateCanLift(CanLift);
                break;

            case PlayerState.Dead:
                //角色死亡時顏色變黑，將自己加入GameManager的deadPlayer
                this.gameObject.GetComponent<MeshRenderer>().material.color=Color.black;
                LiftPoint.UpdateCanLift(CanLift);
                //_gm.CallRpcPlayerDead(this.gameObject);
                
                // SwitchState(PlayerState.Idle);

                break;

            case PlayerState.Lift:
                LiftPoint.UpdateCanLift(CanLift);
                hashTable table = new hashTable();
                Lifting=true;
                table.Add("Lifting",Lifting);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
                break;

            case PlayerState.BeLift:
                
                break;

        }
        //Debug.Log(this.name+_playerState);
    }

    IEnumerator Delay(float s){
        yield return new WaitForSecondsRealtime(s);
        playerInput.SwitchCurrentActionMap(defaultMap);
        _playerState=PlayerState.Idle;
        
    }
//-------改變狀態-------
    public void SwitchState(PlayerState state){
        _playerState=state;
        //print(this.gameObject.name +" " + state);
        string stateText = state.ToString();
        //CallRpcStateSwitch(state,stateText);
    }

    public void CallRpcStateSwitch(PlayerData.PlayerState playerState){//呼叫RPC執行切換角色狀態
        _pv.RPC("RpcStateSwitch",RpcTarget.All,playerState);
    }

    [PunRPC]
    void RpcStateSwitch(PlayerData.PlayerState playerState,PhotonMessageInfo info){//RPC執行切換角色狀態
        
        SwitchState(playerState);
        //_playerState = playerState;
    }
    
    //-------撞到時-------
    public void OnHit(PlayerData other){//被撞到的時候
        if (other._playerState==PlayerState.Dash)
        {
            hit.Play();
            //_gm.CallRpcSendMessageToAll(other._pv.Owner.NickName + "撞到" + _pv.Owner.NickName);
            //_gm.CallRpcSendMessageToAll(_pv.Owner.NickName + "RCP Say Hello");
            HitEffect.Play();
            //如果碰撞時自己的狀態是衝刺，對方的tag是player，就把對方的狀態變成CantMove
            CallRpcStateSwitch(_playerState=PlayerState.CantMove);
            print("Hit!!");
        }
    }

    public void OnHitFace(PlayerData other){
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 playerDirection = new Vector3(h,0,v);
        // print(playerDirection);
        //PlayerData otherData=other.transform.GetComponent<PlayerData>();
        if (other._playerState==PlayerState.Dash)
        {//被撞到的時候
            //throwMe = other.gameObject;
            //_gm.CallRpcSendMessageToAll(other._pv.Owner.NickName + "撞到" + _pv.Owner.NickName);
            //_gm.CallRpcSendMessageToAll(_pv.Owner.NickName + "RCP Say Hello");
            //如果碰撞時自己的狀態是衝刺，對方的tag是player，就把對方的狀態變成CantMove
            //SwitchState(_playerState=PlayerState.CantMove);
            //enemyList.Add(other.gameObject);
            //other.enemyList.Add(this.gameObject);
            other.gameObject.GetComponent<Rigidbody>().AddForce(-playerDirection*playerContaller.BouncePower,ForceMode.Force);
        }
        print("Hit Face!!");
    }

    //-------計算分數-------
    public void CountingPoint(){//依規則計算分數
        switch(_gm._gr){
            case GameManager.GameRull.TIME://是時間規則時
                throwMe.GetComponent<PlayerData>().AddPoint();
                print(throwMe.gameObject.name + "+1");
                _gm.ReSetPlayer(this.gameObject);
            break;

            case GameManager.GameRull.LIVE://是生命數規則時
                if (Point > 0){//
                    Point -= 1;
                    if (_pv.IsMine)
                    {
                        _gm._pointUI.Life--;
                        _gm._pointUI.UpDatePointUI();
                    }
                    print(this.name + "-1");
                    _gm.ReSetPlayer(this.gameObject);
                }
                if(Point <= 0){           
                    _gm.PlayerCount--;
                    _gm.JudgeGameOver();
                    Destroy(this.gameObject);
                    
                    // string winerName = throwMe.gameObject.GetComponent<PlayerData>().nameText.text;
                    // string winerObj = throwMe.gameObject.name;
                    // _gm.CallRpcSetWinerName(winerName,winerObj);
                    // //print(throwMe.gameObject.GetComponent<PlayerData>().nameText.text + " Point!!");
                    // print("Set " + winerObj);
                }
            break;

            case GameManager.GameRull.FINAL:
                string winName = throwMe.gameObject.GetComponent<PlayerData>().nameText.text;
                _gm.CallRpcSetWinerName(winName,throwMe.gameObject.name);
            break;
        }
    }

    public void AddPoint(){//TIME模式下加分機制
        Point += 1;
        if (_pv.IsMine)
        {
            _gm._pointUI._point = Point;
            _gm._pointUI.UpDatePointUI();
        }
    }
}
