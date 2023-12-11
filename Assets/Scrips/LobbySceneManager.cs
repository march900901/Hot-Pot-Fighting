using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using System.Text;

public class LobbySceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    InputField inputRoomName;//輸入房間名的UI
    [SerializeField]
    InputField inputPlayerName;//輸入玩家名的UI
    [SerializeField]
    GameObject WarningText;//錯誤提示字串
    public GameObject roomButtenPrefab;
    public List<GameObject> roomButtenList = new List<GameObject>();
    public Transform contenObject;
    public float timeBettwinUpdate = 1.5f;
    float nextUpdateTime;
    void Start()
    {

        if (PhotonNetwork.IsConnected==false)
        {//如果執行時沒有連結到伺服器，就轉到Start場景
            SceneManager.LoadScene("Start");
        }else if(PhotonNetwork.CurrentLobby==null){
            PhotonNetwork.JoinLobby();
        }
        WarningText.GetComponent<Text>().text = null;//初始化WarningText
    }

    public override void OnConnectedToMaster()
    {
        //可以解決退出Room後，房間列表不顯示的Bug 
        print("Cenneted to Master!!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {//方便檢視進入photon的lobby
        Debug.Log("Lobby Joined!");
    }

    public string GetRoomName(){
        //過濾輸入的房間名稱
        string roomName=inputRoomName.text;
        return roomName.Trim();
    }

    public string GetPlayerName(){
        //過濾輸入的玩家名稱
        string PlayerName=inputPlayerName.text;
        return PlayerName.Trim();
    }

    public void OnClickCreatRoom(){
        string roomName=GetRoomName();
        string PlayerName=GetPlayerName();
        if (roomName.Length>0 && PlayerName.Length>0)
        {//確認房間名和玩家名不是空的，然後就創建房間並設定自己的名字
            PhotonNetwork.CreateRoom(roomName);
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
        }else{
            Debug.Log("Please enter Room name and Player name");
        }
    }

    public override void OnJoinedRoom()
    {//加入房間後轉到SelectCharacter場景
        Debug.Log("Room Joined!");
        SceneManager.LoadScene("SelectCharacter");
    }

    public void JoinRoomList(string roomName){
        //加入房間
        print("JoinRoomList");
        PhotonNetwork.JoinRoom(roomName);
    }

    public void SetWarningText(string Text){
        //設定WarningText的字，並5秒之後刪除
        WarningText.GetComponent<Text>().text = Text;
        StartCoroutine(Delay(5));
    }

    IEnumerator Delay(float s){
        yield return new WaitForSecondsRealtime(s);
        WarningText.GetComponent<Text>().text = null;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoom(roomList);
            nextUpdateTime = Time.time + timeBettwinUpdate;
        }
    }

    public void UpdateRoom(List<RoomInfo> list){
        // foreach (GameObject item in roomButtenList)
        // {
        //     Destroy(item.gameObject);
        // }
        roomButtenList.Clear();
        foreach (RoomInfo room in list)
        {
            if (room.PlayerCount > 0)
            {
                GameObject newButton = Instantiate(roomButtenPrefab,Vector3.zero,Quaternion.identity,GameObject.Find("Content").transform);
                newButton.gameObject.name = room.Name;
                roomButtenList.Add(newButton);
                print("新增 " + room.Name);
            }else if(room.PlayerCount<=0){
                string NullRoom = room.Name;
                Destroy(GameObject.Find(NullRoom));
            }
        }
    }
}
