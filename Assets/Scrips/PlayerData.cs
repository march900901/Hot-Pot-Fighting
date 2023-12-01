using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public enum PlayerState{Idle,Desh,Fall,Lift,BeLift,Die,CantMove}
    public List<GameObject> enemyList=new List<GameObject>();
    public PlayerState _playerState;
    public GameManager gameManager;
    public GameObject mySlef;
    public Color DefaultColor;
    PlayerContaller playerContaller;
    Rigidbody rigidbody;
    PlayerInput playerInput;    
    PhotonView _pv;
    public string defaultMap;
    public bool Lifting=false;
    public int scapeCount=0;
    


    // Start is called before the first frame update
    void Start()
    {
        _playerState=PlayerState.Idle;
        mySlef=this.gameObject;
        playerContaller=this.transform.GetComponent<PlayerContaller>();
        playerInput=this.transform.GetComponent<PlayerInput>();
        defaultMap=playerInput.defaultActionMap;
        rigidbody=this.transform.GetComponent<Rigidbody>();
        gameManager=FindObjectOfType<GameManager>();
        _pv = this.transform.GetComponent<PhotonView>();
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
                if (!_pv.IsMine)
                {
                    playerContaller.enabled=false;
                }
                this.gameObject.GetComponent<MeshRenderer>().material.color=DefaultColor;
                break;

            case PlayerState.CantMove:
                //進入不可動狀態，物件變白，操作的MAP改成CD
                this.gameObject.GetComponent<MeshRenderer>().material.color=Color.white;
                playerInput.SwitchCurrentActionMap("CD");
                if (!_pv.IsMine)
                {
                    rigidbody.isKinematic=true;
                }
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

            case PlayerState.Desh:
                
                break;

            case PlayerState.Die:
                this.gameObject.GetComponent<MeshRenderer>().material.color=Color.black;
                StartCoroutine(Delay(3));
                gameManager.deadPlayer.Add(mySlef);
                //this.gameObject.SetActive(false);
                gameManager.revivalPlayer(this.gameObject.name);
                Destroy(this.gameObject);
                break;

            case PlayerState.Fall:
                
                break;

            case PlayerState.Lift:
                Lifting=true;
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

    public void SwitchState(PlayerState state){
        _playerState=state;
    }
    private void OnCollisionEnter(Collision other) {
        PlayerData otherData=other.transform.GetComponent<PlayerData>();
        if (_playerState==PlayerState.Desh)
        {
            if (other.transform.tag=="Player")
            {
                //如果碰撞時自己的狀態是衝刺，對方的tag是player，就把對方的狀態變成CantMove
                other.gameObject.GetComponent<PlayerData>()._playerState=PlayerState.CantMove;
                enemyList.Add(other.gameObject);
            }
        }
    }
}
