using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> PlayerList = new List<GameObject>();
    public List<Transform> startTransform = new List<Transform>();
    [SerializeField]TextMeshProUGUI P1pointText;
    [SerializeField]TextMeshProUGUI P2pointText;
    public List<GameObject> deadPlayer = new List<GameObject>();
    public GameObject P1;
    public GameObject P2;
    public Transform P1Start;
    public Transform P2Start;

    public int P1point=0;
    public int P2point=0;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom==null)
        {
            SceneManager.LoadScene("Lobby");
        }else{
            
        }
        //SponPlayer();
        InisGame();
    }

    // Update is called once per frame
    void Update()
    {   //計分
        P1pointText.text=P1point.ToString();
        P2pointText.text=P2point.ToString();
    }

    public void InisGame(){
        int randomNum=0;
        List<Transform> startPions=new List<Transform>();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {//隨機取生成點
            randomNum=Random.Range(0,startTransform.Count);
            startPions.Add(startTransform[randomNum]);
            //print(startPions[i].gameObject.name);
        }
        PhotonNetwork.Instantiate("P1",startPions[Random.Range(0,startPions.Count)].position,Quaternion.identity);
        // if (PlayerList.Count!=0)
        // {
        //     for (int i = 0; i < PlayerList.Count; i++)
        //     {
        //         randomNum=Random.Range(0,startTransform.Count);
        //         //GameObject sponObj = Instantiate(PlayerList[i],startTransform[randomNum].position,Quaternion.identity);
        //         //sponObj.name=PlayerList[i].name;
                
        //     }
        // }else{

        // }
    }

    public void SponPlayer(){
        //遊戲開始時生成玩家在遊戲場景
        int randomNum=0;
        if (PlayerList.Count!=0)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                randomNum=Random.Range(0,startTransform.Count);
                GameObject sponObj = Instantiate(PlayerList[i],startTransform[randomNum].position,Quaternion.identity);
                sponObj.name=PlayerList[i].name;
            }
        }else{

        }
    }

    public void revivalPlayer(string revivalPlayerName){
        int randomNum=0;
        if (deadPlayer.Count!=0)
        {
            for (int i = 0; i < deadPlayer.Count; i++)
            {
                randomNum=Random.Range(0,startTransform.Count);
                GameObject revivalObj = Instantiate(deadPlayer[i],startTransform[randomNum].position,Quaternion.identity);
                revivalObj.name=revivalPlayerName;
                deadPlayer.RemoveAt(i);
            }
        }else{

        }
    }

    public void CountPoint(string PlayerName,int point){
        if (PlayerName=="P1")
        {
            P2point+=point;   
        }
        else if (PlayerName=="P2")
        {
            P1point+=point;
        }
        else{
            
        }
    }
}
