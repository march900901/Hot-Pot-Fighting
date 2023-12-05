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
    public int Point = 0;
    [SerializeField]
    public Text nameText;
    public List<GameObject> enemyList=new List<GameObject>();
    public PlayerState _playerState;
    public Color DefaultColor;
    public GameObject throwMe;
    GameSceneManager _gm;
    PlayerContaller playerContaller;
    Rigidbody rigidbody;
    PlayerInput playerInput;    
    PhotonView _pv;
    hashTable table = new hashTable();
    public string defaultMap;
    public bool Lifting=false;
    public int scapeCount=0;

    


    // Start is called before the first frame update
    void Start()
    {//初始化
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
                //playerContaller.Idle();
                this.gameObject.GetComponent<MeshRenderer>().material.color=DefaultColor;
                break;

            case PlayerState.CantMove:
                //進入不可動狀態，物件變白，操作的MAP改成CD
                this.gameObject.GetComponent<MeshRenderer>().material.color=Color.white;
                playerInput.SwitchCurrentActionMap("CD");
                
                if (scapeCount>=10)
                {
                    //如果達成逃脫條件，變回IDLE狀態，並從子物件中移出，再取消Kinematic
                    _playerState=PlayerState.Idle;
                    playerContaller.enemy=null;
                    this.transform.parent.transform.parent.gameObject.GetComponent<PlayerData>()._playerState=PlayerState.Idle;
                    this.transform.parent=null;
                    rigidbody.isKinematic=false;
                }
                break;

            case PlayerState.Dash:

                break;

            case PlayerState.Dead:
                //角色死亡時顏色變黑，將自己加入GameManager的deadPlayer
                GameObject mySlef = this.gameObject;
                this.gameObject.GetComponent<MeshRenderer>().material.color=Color.black;
                StartCoroutine(Delay(3));
                _gm.deadPlayer.Add(mySlef);
                _gm.SponPlayer();
                Dead();
                PhotonNetwork.Destroy(this.gameObject);
                break;

            case PlayerState.Fly:
                if (!_pv.IsMine)
                {
                    rigidbody.isKinematic=true;
                }
                break;

            case PlayerState.Lift:
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

    public void Dead(){
         //PhotonNetwork.Destroy(this.gameObject);
         _gm.CallRpcPlayerDead();
    }

    IEnumerator Delay(float s){
        yield return new WaitForSecondsRealtime(s);
        playerInput.SwitchCurrentActionMap(defaultMap);
        _playerState=PlayerState.Idle;
        
    }

    public void SwitchState(PlayerState state){
        //hashTable table = new hashTable();
        _playerState=state;
        //table.Add("_playerState",_playerState);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(table);
        string stateText = state.ToString();
        CallRpcStateSwitch(state,stateText);
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Player")
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 playerDirection = new Vector3(h,0,v);
            PlayerData otherData=other.transform.GetComponent<PlayerData>();
            if (otherData._playerState==PlayerState.Dash)
            {//被撞到的時候
                throwMe = other.gameObject;
                _gm.CallRpcSendMessageToAll(otherData._pv.Owner.NickName + "撞到" + _pv.Owner.NickName);
                _gm.CallRpcSendMessageToAll(_pv.Owner.NickName + "RCP Say Hello");
                //如果碰撞時自己的狀態是衝刺，對方的tag是player，就把對方的狀態變成CantMove
                SwitchState(_playerState=PlayerState.CantMove);
                //enemyList.Add(other.gameObject);
                otherData.enemyList.Add(this.gameObject);
                other.gameObject.GetComponent<Rigidbody>().AddForce(-playerDirection*playerContaller.BouncePower,ForceMode.Force);
            }else{}
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
    }
}
