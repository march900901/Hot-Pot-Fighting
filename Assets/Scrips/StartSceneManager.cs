using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StartSceneManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject LoadingPanle;
    public List<GameObject>BGM = new List<GameObject>();

    private void Start() {
        LoadingPanle.SetActive(false);
        BGM.Add(GameObject.Find("BGM"));
        if (BGM.Count>1)
        {
            for (int i = 1; i < BGM.Count-1; i++)
            {
                Destroy(BGM[i]);
            }
        }
    }

    public void OnClickStart(){
        PhotonNetwork.AutomaticallySyncScene=true;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Click Start");
        //SceneManager.LoadScene("Story");
        LoadingPanle.SetActive(true);
        LoadingPanle.GetComponent<DoTween>().ScalePanel();

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conneted!");
        SceneManager.LoadScene("Story");
    }
}
