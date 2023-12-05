using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using TMPro;
using Photon.Pun;
using hashTable = ExitGames.Client.Photon.Hashtable;
using UnityEditor;
using Unity.VisualScripting;

public class PlayerContaller : MonoBehaviourPunCallbacks
{   
    PlayerData playerData;
    Rigidbody rigidbody;
    Vector2 movevector;
    PlayerInput playerInput;
    [SerializeField]
    GameObject LiftPoint;

    public float MoveSpeed=5.0f;
    public float DashPower=5.0f;
    public float RotaSpeed=1;
    public float MaxSpeed=5.0f;
    public float DashCD=3.0f;
    public float FallCD=5.0f;
    public float beLiftCD=5;
    public float BouncePower=5.0f;
    public string defaultMap;
    public float throwPower=5.0f;
    private float movingSpeed;
    public GameObject enemy;
    public PhotonView _pv;
    BoxCollider boxCollider;
    GameSceneManager _gm;
    

    // Start is called before the first frame update
    void Start()
    {//初始化
        playerData = this.transform.GetComponent<PlayerData>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
        playerInput = this.transform.GetComponent<PlayerInput>();      
        defaultMap = playerInput.defaultActionMap;
        LiftPoint = this.transform.GetChild(0).gameObject;
        boxCollider = LiftPoint.GetComponent<BoxCollider>();
        _pv = this.transform.GetComponent<PhotonView>();
        _gm = GameObject.Find("GameManager").GetComponent<GameSceneManager>();
        // if (!_pv.IsMine)
        // {
        //     playerInput.SwitchCurrentActionMap("NotMe");
        //     playerData.DefaultColor=Color.red;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_pv.IsMine)
        {
            //一直將自己以外的玩家物件控制關掉，並將材質顏色設為紅色
            playerInput.SwitchCurrentActionMap("NotMe"); 
            playerData.DefaultColor=Color.red;
        }
        movingSpeed=rigidbody.velocity.magnitude;//取得角色移動速度
        if(movevector!=Vector2.zero){
            //
            if(movingSpeed<=MaxSpeed){//角色移動
                rigidbody.AddForce(movevector.x*MoveSpeed*Time.deltaTime,0,movevector.y*MoveSpeed*Time.deltaTime,ForceMode.Force);
            }
            if(movevector.x>0){//角色轉向右邊
                this.transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,90,0),RotaSpeed);
            }

            if(movevector.x<0){//角色轉向左邊
                this.transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,-90,0),RotaSpeed);
            }

            if(movevector.y>0){//角色轉向前面
                this.transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,0,0),RotaSpeed);
            }

            if(movevector.y<0){//角色轉向後面
                this.transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,180,0),RotaSpeed);
            }
            
        }
    } 

    IEnumerator DelayAction(float s){
        //延遲過後將角色控制設為預設Map，並將狀態改為Idle
        yield return new WaitForSecondsRealtime(s);
        playerInput.SwitchCurrentActionMap(defaultMap);
        playerData.SwitchState(PlayerData.PlayerState.Idle);
    }
//--------抬人--------
    public void Lift(InputAction.CallbackContext callback){
        if (callback.performed){
            //收到輸入訊號後
            DoLift();//自己執行抬人
            CallRpcDoLift();//呼叫其他玩家場景的自己執行抬人
        }
    }

    public void CallRpcDoLift(){
        //用RPC呼叫其他玩家場景的自己執行RpcDoLift
        _pv.RPC("RpcDoLift",RpcTarget.Others);
    }

    [PunRPC]
    void RpcDoLift(PhotonMessageInfo info){
        //Rpc接收
        print("Lift");
        DoLift();
    }

    public void DoLift(){
        if (PlayerDistance() && playerData.enemyList.Count > 0)
            {//如果和倒地的人距離夠近，且enemyList不是空的，
                enemy=playerData.enemyList[0];
                if (enemy.GetComponent<PlayerData>().Lifting == false)
                {
                    //如果距離夠進就可以把敵人列表裡第一個抬起來
                    playerData.SwitchState(PlayerData.PlayerState.Lift);
                    enemy.GetComponent<Rigidbody>().isKinematic=true;
                    enemy.transform.position=this.transform.Find("LiftPoint").transform.position;
                    enemy.transform.Rotate(-90f,90f,0f,Space.Self);
                    enemy.transform.parent=this.transform.Find("LiftPoint");
                }
            }
    }
//-------計算玩家距離-------
    public bool PlayerDistance(){
        //計算與其他玩家物件的距離
        bool canLift = false;
        float distance=0;
        if (playerData.enemyList != null && playerData.enemyList.Count > 0)
        {
            //計算自己與敵人列表中第一個的距離
            distance = Vector3.Distance(this.transform.position,playerData.enemyList[0].transform.position);
            if (distance<=1.5f)
            {
                canLift=true;
            }else{
                canLift=false;
            } 
        }
        return canLift;
    }

//-------丟-------
    public void Throw(InputAction.CallbackContext callback){
        if (callback.performed)
        {//收到輸入指令時
            DoThrow();
            CallRpcDoThrow();
        }
    }

    public void DoThrow(){
        if (enemy&&playerData._playerState==PlayerData.PlayerState.Lift)
            {//確認enemy不是空的且自己的狀態是Lift
                Rigidbody enemyRig=enemy.GetComponent<Rigidbody>();
                PlayerData enemyData = enemy.GetComponent<PlayerData>();
                Debug.Log("Throw");
                //把敵人的狀態改為CantMove
                enemyData.SwitchState(PlayerData.PlayerState.CantMove);
                //把對方的throwMe設為自己
                enemyData.throwMe = this.gameObject;
                enemyRig.isKinematic=false;
                //丟出去
                enemyRig.AddForce(new Vector3(0,throwPower,throwPower),ForceMode.Impulse);
                //把對方從子物件移出
                enemy.transform.parent=null;
                //自己狀態設為Idle
                playerData.SwitchState(PlayerData.PlayerState.Idle);
                //刪掉敵人列表第一項
                playerData.enemyList.RemoveAt(0);
                print("Throw!!!");
            }
    }

    public void CallRpcDoThrow(){
        _pv.RPC("RpcDoThrow",RpcTarget.Others);
    }

    [PunRPC]
    void RpcDoThrow(PhotonMessageInfo info){
        DoThrow();
    }

//-------衝刺-------
    public void Dash(InputAction.CallbackContext callback){
        if(callback.performed){//收到輸入訊號
            if (playerData._playerState==PlayerData.PlayerState.Idle)
            {//自己的狀態是Idle
                if (_pv.IsMine)
                {//限制只能控制自己
                    //向前衝刺一下
                    playerData.SwitchState(PlayerData.PlayerState.Dash);
                    rigidbody.AddForce(new Vector3(movevector.x,0,movevector.y)*DashPower,ForceMode.Impulse);
                    playerInput.SwitchCurrentActionMap("CD");//取消玩家控制
                    StartCoroutine(DelayAction(DashCD));//玩家進入CD時間
                }
            }             
        }
    }

    public void Move(InputAction.CallbackContext callback){
        movevector=callback.ReadValue<Vector2>();
    }

//-------逃脫機制-------
    public void Scaper(InputAction.CallbackContext callback){
        //計算逃脫按的次數
        if (callback.performed && playerData._playerState==PlayerData.PlayerState.CantMove)
        {
            DoScape();            
        }
    }

    public void CallRpcDoScaper(){
        //用RPC呼叫其他玩家場景的自己執行DoScape
        _pv.RPC("RpcDoScape",RpcTarget.Others);
    }

    [PunRPC]
    void RpcDoScape(PhotonMessageInfo info){
        //收到RPC訊號
        DoScape();
    }

    public void DoScape(){
        //計算ScapeCount並將playerData的ScapeCount數值上傳HashTable
        if (playerData.scapeCount>10)
            {
                hashTable table = new hashTable();
                playerData.scapeCount=0;
                table.Add("scapeCount",playerData.scapeCount);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }
            else{
                hashTable table = new hashTable();
                playerData.scapeCount++;
                table.Add("scapeCount",playerData.scapeCount);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }
    }

    public void Idle(){
        //boxCollider.enabled=false;
    }
}