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


public class PlayerData : MonoBehaviourPunCallbacks
{
    public enum PlayerState{Idle,Dash,Fly,Lift,BeLift,Dead,CantMove}
    public string Name;
    public int Point = 0;
    [SerializeField]
    public Text nameText;
    public GameObject enemy;
    public PlayerState _playerState;
    public Color DefaultColor;
    public GameObject throwMe;
    public LiftCollider LiftPoint;
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
        this.gameObject.name = Name;
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
                playerInput.SwitchCurrentActionMap(defaultMap);
                this.gameObject.GetComponent<MeshRenderer>().material.color=DefaultColor;
                LiftPoint.UpdateCanLift(CanLift);
                break;

            case PlayerState.CantMove:
                //進入不可動狀態，物件變白，操作的MAP改成CD
                this.gameObject.GetComponent<MeshRenderer>().material.color=Color.white;
                playerInput.SwitchCurrentActionMap("CD");
                LiftPoint.UpdateCanLift(true);
                if (scapeCount>=10)
                {
                    //如果達成逃脫條件，變回IDLE狀態，並從子物件中移出，再取消Kinematic
                    _playerState=PlayerState.Idle;
                    enemy.GetComponent<PlayerData>()._playerState=PlayerState.Idle;
                    enemy=null;
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
                //_gm.DeadPlayer = this.gameObject;
                //_gm.SponPlayer(Name);
                _gm.CallRpcPlayerDead(this.gameObject.name);
                // PhotonNetwork.Destroy(this.gameObject);
                //print("Daed!!");
                SwitchState(PlayerState.Idle);

                break;

            // case PlayerState.Fly:
            //     if (!_pv.IsMine)
            //     {
            //         rigidbody.isKinematic=true;
            //     }
            //     break;

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
        //hashTable table = new hashTable();
        _playerState=state;
        //table.Add("_playerState",_playerState);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(table);
        string stateText = state.ToString();
        CallRpcStateSwitch(state,stateText);
    }
    
    //-------撞到時-------
    public void OnHit(PlayerData other){
        //PlayerData otherData=other.transform.GetComponent<PlayerData>();
        if (other._playerState==PlayerState.Dash)
        {//被撞到的時候
            //throwMe = other.gameObject;
            _gm.CallRpcSendMessageToAll(other._pv.Owner.NickName + "撞到" + _pv.Owner.NickName);
            _gm.CallRpcSendMessageToAll(_pv.Owner.NickName + "RCP Say Hello");
            //如果碰撞時自己的狀態是衝刺，對方的tag是player，就把對方的狀態變成CantMove
            SwitchState(_playerState=PlayerState.CantMove);
            //enemyList.Add(other.gameObject);
            other.enemy = this.gameObject;
            //other.gameObject.GetComponent<Rigidbody>().AddForce(-playerDirection*playerContaller.BouncePower,ForceMode.Force);
            print("Hit!!");
        }
    }

    public void OnHitFace(PlayerData other){
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 playerDirection = new Vector3(h,0,v);
        print(playerDirection);
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

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Player")
        {
            //OnHitFace(other.gameObject.GetComponent<PlayerData>());
        }
    }

    // public override void OnPlayerPropertiesUpdate(Player targetPlayer, hashTable changedProps)
    // {//接收數值更新
    //     if (targetPlayer != null && changedProps != null)
    //     {
    //         if (targetPlayer == _pv.Owner)
    //         {
    //             //_playerState = (PlayerState)changedProps["_playerState"]; 
    //             if (scapeCount != null && changedProps != null)
    //             {
    //                 scapeCount = (int)changedProps["scapeCount"];
    //             }
    //         }
    //     }
    // }

//-------同步角色狀態-------
    public void CallRpcStateSwitch(PlayerData.PlayerState playerState,string playerStateText){
        _pv.RPC("RpcStateSwitch",RpcTarget.All,playerState,playerStateText);
    }

    [PunRPC]
    void RpcStateSwitch(PlayerData.PlayerState playerState,string text,PhotonMessageInfo info){
        print(playerState);
        _playerState = playerState;
    }

    //-------加分-------
    public void CountingPoint(){
        throwMe.GetComponent<PlayerData>().Point += 1;
        print("+1");
        if(throwMe.GetComponent<PlayerData>().Point >= 3){
            _gm.GameOver();
            _gm.CallRpcGameOver();
            print("Point!!");
        }
    }
}
