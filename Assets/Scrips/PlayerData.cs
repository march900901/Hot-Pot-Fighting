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


public class PlayerData : MonoBehaviourPunCallbacks
{
    public enum PlayerState{Idle,Dash,Fly,Lift,BeLift,Dead,CantMove}
    public string Name;
    public int Point = 0;
    [SerializeField]
    public Text nameText;
    public  ParticleSystem HitEffect;
    public GameObject scapeEffect;
    public GameObject enemy;
    public PlayerState _playerState;
    public Color DefaultColor;
    public GameObject throwMe;
    public LiftCollider LiftPoint;
    public GameObject Star;
    public AudioSource hit;
    GameSceneManager _gm;
    PlayerContaller playerContaller;
    Rigidbody rigidbody;
    PlayerInput playerInput;    
    PhotonView _pv;
    hashTable table = new hashTable();
    public string defaultMap;
    public bool Lifting=false;
    public bool CanLift;
    public int scapeCount=0;

    


    // Start is called before the first frame update
    void Start()
    {//初始化
        if (this.gameObject.name.EndsWith("(Clone)"))
        {
            Name = this.gameObject.name.TrimEnd("(Clone)");
            this.gameObject.name = Name;
        }else{Name = this.gameObject.name;}
        Point = 0;
        _playerState=PlayerState.Idle;
        playerContaller=this.transform.GetComponent<PlayerContaller>();
        playerInput=this.transform.GetComponent<PlayerInput>();
        defaultMap=playerInput.defaultActionMap;
        rigidbody=this.transform.GetComponent<Rigidbody>();
        _gm = GameObject.Find("GameManager").GetComponent<GameSceneManager>(); 
        _pv = this.transform.GetComponent<PhotonView>();
        nameText.text = _pv.Owner.NickName;
        
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
                playerInput.SwitchCurrentActionMap("CD");
                LiftPoint.UpdateCanLift(true);
                Lifting = false;
                Star.active = true;
                playerContaller.StopAllCoroutines();
                if (scapeCount>=10)
                {
                    GameObject ScapeParticle = Instantiate(scapeEffect,this.transform);
                    Destroy(ScapeParticle,5);
                    //如果達成逃脫條件，變回IDLE狀態，並從子物件中移出，再取消Kinematic
                    _playerState=PlayerState.Idle;
                    if (enemy)
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
            _gm.CallRpcSendMessageToAll(other._pv.Owner.NickName + "撞到" + _pv.Owner.NickName);
            _gm.CallRpcSendMessageToAll(_pv.Owner.NickName + "RCP Say Hello");
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
            _gm.CallRpcSendMessageToAll(other._pv.Owner.NickName + "撞到" + _pv.Owner.NickName);
            _gm.CallRpcSendMessageToAll(_pv.Owner.NickName + "RCP Say Hello");
            //如果碰撞時自己的狀態是衝刺，對方的tag是player，就把對方的狀態變成CantMove
            //SwitchState(_playerState=PlayerState.CantMove);
            //enemyList.Add(other.gameObject);
            //other.enemyList.Add(this.gameObject);
            other.gameObject.GetComponent<Rigidbody>().AddForce(-playerDirection*playerContaller.BouncePower,ForceMode.Force);
        }
        print("Hit Face!!");
    }

    //-------加分-------
    public void CountingPoint(){//加分&&勝利
        if (throwMe && throwMe.GetComponent<PlayerData>().Point <= 2)
        {
            throwMe.GetComponent<PlayerData>().Point += 1;
            print(throwMe.name + "+1");
            _gm.ReSetPlayer(this.gameObject);
        }
        if(throwMe && throwMe.GetComponent<PlayerData>().Point >= 3){            
            string winerName = throwMe.gameObject.GetComponent<PlayerData>().nameText.text;
            string winerObj = throwMe.gameObject.name;
            _gm.CallRpcSetWinerName(winerName,winerObj);
            //print(throwMe.gameObject.GetComponent<PlayerData>().nameText.text + " Point!!");
            print("Set " + winerObj);
        }
    }
}
