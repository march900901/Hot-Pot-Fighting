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


public class PlayerData : MonoBehaviourPunCallbacks
{
    public enum PlayerState{Idle,Dash,Fly,Lift,BeLift,Dead,CantMove}
    [SerializeField]
    public Text nameText;
    public List<GameObject> enemyList=new List<GameObject>();
    public PlayerState _playerState;
    public Color DefaultColor;
    public GameObject throwMe;
    GameManager _gm;
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
    {
        _playerState=PlayerState.Idle;
        playerContaller=this.transform.GetComponent<PlayerContaller>();
        playerInput=this.transform.GetComponent<PlayerInput>();
        defaultMap=playerInput.defaultActionMap;
        rigidbody=this.transform.GetComponent<Rigidbody>();
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>(); 
        _pv = this.transform.GetComponent<PhotonView>();
        nameText.text = _pv.Owner.NickName;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (_playerState)
        {
            case PlayerState.Idle:
                scapeCount=0;
                playerInput.SwitchCurrentActionMap(defaultMap);
                playerContaller.Idle();
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
                GameObject mySlef = this.gameObject;
                this.gameObject.GetComponent<MeshRenderer>().material.color=Color.black;
                StartCoroutine(Delay(3));
                _gm.deadPlayer.Add(mySlef);
                _gm.revivalPlayer(this.gameObject.name);
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
         PhotonNetwork.Destroy(this.gameObject);
         //告訴RPC呼叫告訴MesterClient自己被消滅了
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
            PlayerData otherData=other.transform.GetComponent<PlayerData>();
            if (otherData._playerState==PlayerState.Dash)
            {
                _gm.CallRpcSendMessageToAll(otherData._pv.Owner.NickName + "撞到" + _pv.Owner.NickName);
                _gm.CallRpcSendMessageToAll(_pv.Owner.NickName + "RCP Say Hello");
                //如果碰撞時自己的狀態是衝刺，對方的tag是player，就把對方的狀態變成CantMove
                SwitchState(_playerState=PlayerState.CantMove);
                enemyList.Add(other.gameObject);
            }else{}
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, hashTable changedProps)
    {
        if (targetPlayer == _pv.Owner)
        {
            _playerState = (PlayerState)changedProps["_playerState"];
            scapeCount = (int)changedProps["scapeCount"];
        }
    }

    public void CallRpcStateSwitch(PlayerData.PlayerState playerState,string playerStateText){
        _pv.RPC("RpcStateSwitch",RpcTarget.All,playerState,playerStateText);
    }

    [PunRPC]
    void RpcStateSwitch(PlayerData.PlayerState playerState,string text,PhotonMessageInfo info){
        print(playerState);
        _playerState = playerState;
    }
}
