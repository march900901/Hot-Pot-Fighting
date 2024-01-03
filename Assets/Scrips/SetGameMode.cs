using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SetGameMode : MonoBehaviour
{
    public Text GameTime;
    public int GameMode;
    public PhotonView _pv;
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            print("IsMasterClient");
        }else{
            print("NotMasterClient");
        }
    }

    public void CallRpcSetTimeMode(){
        _pv.RPC("RpcSetTimeMode",RpcTarget.All);
        GameMode = 0;
        print("GameMode: 0");
    }

    [PunRPC]
    void RpcSetTimeMode(PhotonMessageInfo info){
        PlayerPrefs.SetInt("GameMode",0);
        
    }

    public void CallRpcSetLifeMode(){
        _pv.RPC("RpcSetLifeMode",RpcTarget.All);
        GameMode = 1;
    }

    [PunRPC]
    void RpcSetLifeMode(PhotonMessageInfo info){
        print("GameMode: 1");
        PlayerPrefs.SetInt("GameMode",1);
        
    }

    public void CallRpcSetGameTime(){
       _pv.RPC("RpcSetGameTime",RpcTarget.All);
    }

    [PunRPC]
    void RpcSetGameTime(PhotonMessageInfo info){
         if (GameTime != null)
        {
            string timeString = GameTime.text.ToString();
            PlayerPrefs.SetInt("GameTime",int.Parse(timeString));
        }else{
            PlayerPrefs.SetInt("GameTime",60);
        }
    }

}
