using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject roomButten;
    public List<GameObject> buttenList = new List<GameObject>();
    public float timeBetweenUpdate = 1.5f;
    float nextUpdateTime;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {//當房間列表更新時
        print("Update!!");
        UpdateRoomList(roomList);
       if (Time.time >= nextUpdateTime)
       {//讓房間列表更新時延遲1.5秒，以防出現兩個一樣的按鈕
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdate;
       }
    }

    public void UpdateRoomList(List<RoomInfo> roomInfos){
        //更新房間列表
        foreach (GameObject butten in buttenList)
        {//刪除房間列表所有按鈕
            Destroy(butten.gameObject);
        }
        buttenList.Clear();

        foreach(RoomInfo roomInfo in roomInfos)
        {//一個一個處理伺服器房間列表的房間
            //ConparList(roomInfos);
            for (int i = 0; i < roomInfos.Count; i++)
            {
                print(roomInfos[i].Name);
                //生成房間按鈕，位置在Content物件的(0,0)旋轉不便，並加入Comtent的子物件
                GameObject room = Instantiate(roomButten,Vector3.zero,Quaternion.identity,GameObject.Find("Content").transform);
                //將生成的按鈕名字改成這個房間名字
                room.name = roomInfos[i].Name;
                buttenList.Add(room);//再按鈕列表加入這個按鈕
            }
            if (roomInfo.PlayerCount <= 0)
            {//如果房間裡的人數為0以下的話就刪掉這個按鈕
                Destroy(GameObject.Find(roomInfo.Name));
            }
        }
    }

    // public void ConparList(List<RoomInfo> list){
    //     //過濾重複的房間名稱
    //     for (int i = 0; i < list.Count; i++)
    //     {
    //         for (int j=list.Count-1; j>i; j--)
    //         {
    //             if (list[i] == list[j])
    //             {
    //                 list.RemoveAt(j);
    //             }
    //         }
    //     }
    // }
}
