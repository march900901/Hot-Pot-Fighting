using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Photon.Pun;

public class GameOverManager : MonoBehaviour
{
    public Transform GeneratPoint;
    // Start is called before the first frame update
    void Start()
    {
        SetWiner();
    }

    // Update is called once per frame

    public void SetWiner(){
        string winerName =  PlayerPrefs.GetString("WinerName");
        print(winerName);
        GameObject winer = Instantiate(Resources.Load<GameObject>(winerName),GeneratPoint.position,Quaternion.identity);
        Destroy(winer.GetComponent<PlayerContaller>());
        Destroy(winer.GetComponent<PlayerData>());
        Destroy(winer.GetComponent<PlayerInput>());
        winer.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void GoLobby(){
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }
}
