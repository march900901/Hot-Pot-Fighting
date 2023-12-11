using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject roomButten;
    public List<string> RoomNameList = new List<string>();
    public List<GameObject> buttonList = new List<GameObject>();
    public float timeBetweenUpdate = 1.5f;
    float nextUpdateTime;
    public int roomCount;
    public string JoinRoomName;

    // public override void OnRoomListUpdate(List<RoomInfo> roomList)
    // {//當房間列表更新時

    //     print("Update");
    //     List<string> strings = new List<string>();
    //     foreach (RoomInfo item in roomList)
    //     {print("Get" + item.Name);
    //         strings.Add(item.Name);
    //     }
        
    //     foreach (string item in strings)
    //     {
    //         if(RoomNameList.Contains(item)){ 
    //             roomCount--;
    //             RoomNameList.Remove(item);
    //             print("Remove " + item);
    //         }else{
    //             roomCount++;
    //             RoomNameList.Add(item);
    //             print("ADD " + item);
                
    //         }
    //     }
        
    //     if (Time.deltaTime >= nextUpdateTime)
    //     {
    //         UpdateRoomList(strings);
    //         nextUpdateTime = Time.deltaTime + timeBetweenUpdate;
    //     }

    //     foreach (RoomInfo roomInfo in roomList)
    //     {
    //         if (roomInfo.PlayerCount <= 0)
    //         {//如果房間裡的人數為0以下的話就刪掉這個按鈕
    //             Destroy(GameObject.Find(roomInfo.Name));
    //             RoomNameList.Remove(roomInfo.Name);
    //             print(roomInfo.Name + " 是空的");
    //         }
    //     }
    // }

    public void UpdateRoomList(List<string> strings){
        //一個一個處理伺服器房間列表的房間
        buttonList.Clear();
        Destroy(GameObject.FindWithTag("RoomButten"));
        print("更新房間列表");
        for (int i = 0; i < RoomNameList.Count; i++)
        {
            //print(roomInfos[i].Name);
            //生成房間按鈕，位置在Content物件的(0,0)旋轉不便，並加入Comtent的子物件
            GameObject room = Instantiate(roomButten,Vector3.zero,Quaternion.identity,GameObject.Find("Content").transform);
            //將生成的按鈕名字改成這個房間名字
            room.name = RoomNameList[i];
            buttonList.Add(room);//再按鈕列表加入這個按鈕
        }
            
    }

    public List<string> ConparList(List<string> list){//過濾重複的房間名稱
        List<string> List = list.Distinct().ToList();
        
        // HashSet<string> List = new HashSet<string>(list);
        // List<string> NewList = new List<string>(List);

        // for (int i = 0; i < list.Count; i++)
        // {
        //     for (int j=list.Count-1; j>i; j--)
        //     {
        //         if (list[i] == list[j])
        //         {
        //             list.RemoveAt(j);
        //         }
        //     }
        // } 
        // foreach (var item in List)
        // {
        //     print("Conpar" + item);
        // }
        return list;
    }
}
