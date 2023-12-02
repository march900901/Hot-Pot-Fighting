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
    GameManager _gm;
    

    // Start is called before the first frame update
    void Start()
    {
        playerData = this.transform.GetComponent<PlayerData>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
        playerInput = this.transform.GetComponent<PlayerInput>();      
        defaultMap = playerInput.defaultActionMap;
        LiftPoint = this.transform.GetChild(0).gameObject;
        boxCollider = LiftPoint.GetComponent<BoxCollider>();
        _pv = this.transform.GetComponent<PhotonView>();
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            //this.GetComponent<PlayerContaller>().enabled=false;
            playerInput.SwitchCurrentActionMap("NotMe"); 
            playerData.DefaultColor=Color.red;
        }
        movingSpeed=rigidbody.velocity.magnitude;
        if(movevector!=Vector2.zero){
            //this.transform.position+=new Vector3(movevector.x,0,movevector.y)*MoveSpeed*Time.deltaTime;
            if(movingSpeed<=MaxSpeed){
                rigidbody.AddForce(movevector.x*MoveSpeed*Time.deltaTime,0,movevector.y*MoveSpeed*Time.deltaTime,ForceMode.Force);
            }
            if(movevector.x>0){
                    this.transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,90,0),RotaSpeed);
            }

            if(movevector.x<0){
                this.transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,-90,0),RotaSpeed);
            }

            if(movevector.y>0){
                this.transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,0,0),RotaSpeed);
            }

            if(movevector.y<0){
                this.transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,180,0),RotaSpeed);
            }
            
        }
    } 

    IEnumerator DelayAction(float s){
        yield return new WaitForSecondsRealtime(s);
        playerInput.SwitchCurrentActionMap(defaultMap);
        playerData.SwitchState(PlayerData.PlayerState.Idle);
    }

    public void Lift(InputAction.CallbackContext callback){
        if (callback.performed){
            if (PlayerDistance() && playerData.enemyList.Count > 0)
            {//如果和倒地的人距離夠近，且enemyList不是空的，
                enemy=playerData.enemyList[0];
                if (enemy.GetComponent<PlayerData>().Lifting == false)
                {
                    //如果距離夠進就可以把敵人列表裡第一個抬起來
                    playerData.SwitchState(PlayerData.PlayerState.Lift);
                    enemy.GetComponent<Rigidbody>().isKinematic=true;
                    enemy.transform.position=this.transform.Find("LiftPoint").transform.position;
                    enemy.transform.parent=this.transform.Find("LiftPoint");
                }
            }       
            
        }
    }

    public bool PlayerDistance(){
        bool canLift;
        float distance=0;
        //計算自己與敵人列表中第一個的距離
        distance = Vector3.Distance(this.transform.position,playerData.enemyList[0].transform.position);
        if (distance<=1.5f)
        {
            canLift=true;
        }else{
            canLift=false;
        }
        return canLift;
    }

    public void Throw(InputAction.CallbackContext callback){
        if (callback.performed)
        {
            if (enemy&&playerData._playerState==PlayerData.PlayerState.Lift)
            {
                Rigidbody enemyRig=enemy.GetComponent<Rigidbody>();
                PlayerData enemyData = enemy.GetComponent<PlayerData>();
                Debug.Log("Throw");
                enemyData.SwitchState(PlayerData.PlayerState.CantMove);
                enemyData.throwMe = this.gameObject;
                enemyRig.isKinematic=false;
                enemyRig.AddForce(new Vector3(0,throwPower,throwPower),ForceMode.Impulse);
                enemy.transform.parent=null;
                playerData.SwitchState(PlayerData.PlayerState.Idle);
                playerData.enemyList.RemoveAt(0);
            }
        }
    }

    public void Dash(InputAction.CallbackContext callback){
        if(callback.performed){
            if (playerData._playerState==PlayerData.PlayerState.Idle)
            {
                if (_pv.IsMine)
                {
                    //如果按下desh按鍵且自己狀態是idle的話，向前衝刺一下
                    playerData.SwitchState(PlayerData.PlayerState.Dash);
                    rigidbody.AddForce(new Vector3(movevector.x,0,movevector.y)*DashPower,ForceMode.Impulse);
                    playerInput.SwitchCurrentActionMap("CD");
                    StartCoroutine(DelayAction(DashCD));
                }
            }             
        }
    }

    public void Move(InputAction.CallbackContext callback){
        movevector=callback.ReadValue<Vector2>();
    }

    public void Scaper(InputAction.CallbackContext callback){
        //計算逃脫按的次數
        if (callback.performed && playerData._playerState==PlayerData.PlayerState.CantMove)
        {
            hashTable table = new hashTable();
            if (playerData.scapeCount>10)
            {
                playerData.scapeCount=0;
                table.Add("scapeCount",playerData.scapeCount);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }
            else{
                playerData.scapeCount++;
                table.Add("scapeCount",playerData.scapeCount);
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }
        }
    }

    public void Idle(){
        boxCollider.enabled=false;
    }
}