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
    // Start is called before the first frame update
    void Start()
    {
        SetWiner();
        PlayParticle();
    }

    // Update is called once per frame

    public void SetWiner(){
        string winerName =  PlayerPrefs.GetString("WinerName");
        winertext.text = winerName.ToUpper() +" " + "WIN !!!";
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
