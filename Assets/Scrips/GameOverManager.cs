using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public Transform GeneratPoint;
    public Text winertext;
    public List<ParticleSystem> particles = new List<ParticleSystem>();
    public AudioManager _am;
    PhotonView _pv;
    // Start is called before the first frame update
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        SetWiner();
        PlayParticle();
    }

    // Update is called once per frame

    public void SetWiner(){
        string winerName =  PlayerPrefs.GetString("WinerName");
        string winerObj = PlayerPrefs.GetString("WinerObj");
        winertext.text = winerName.ToUpper() +" " + "WIN !!!";//將顯示的字設為大寫
        print(winerObj);
        GameObject winer = Instantiate(Resources.Load<GameObject>(winerObj),GeneratPoint.position,Quaternion.identity);
        winer.GetComponent<PlayerData>().nameText.text = winerName;
        Animator _animator = winer.GetComponentInChildren<Animator>();
        _animator.SetTrigger("Choose");
        Destroy(winer.GetComponent<PlayerContaller>());
        Destroy(winer.GetComponent<PlayerData>());
        Destroy(winer.GetComponent<PlayerInput>());
        winer.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void GoRoom(){
        //PhotonNetwork.LeaveRoom();
        CallRpcGoRoom();
    }

    public void ReStart(){
        CallRpcReStart();
        _am.PlayAudio(2);
    }

    public void CallRpcReStart(){
        _pv.RPC("RpcReStart",RpcTarget.All);
    }

    [PunRPC]
    void RpcReStart(PhotonMessageInfo info){
        SceneManager.LoadScene("Game");
    }

    public void CallRpcGoRoom(){
        _pv.RPC("RpcGoRoom",RpcTarget.All);
    }

    [PunRPC]
    void RpcGoRoom(PhotonMessageInfo info){
        PhotonNetwork.CurrentRoom.IsVisible = true;
        SceneManager.LoadScene("SelectCharacter");
    }
    public void PlayParticle(){
        float beetwinDelayTime = 0.5f;
        float delayTime = 0;
        
        foreach (ParticleSystem item in particles)
        {item.Play();
            if (Time.deltaTime>=delayTime)
            {
                
                delayTime = Time.deltaTime + beetwinDelayTime;
            }

        }
    }
}
