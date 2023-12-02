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
    {
        
       if (Time.time >= nextUpdateTime)
       {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdate;
       }
        
    }

    public void UpdateRoomList(List<RoomInfo> roomInfos){
        foreach (GameObject butten in buttenList)
        {
            Destroy(butten.gameObject);
        }
        buttenList.Clear();

        foreach(RoomInfo roomInfo in roomInfos)
        {
            ConparList(roomInfos);
            for (int i = 0; i < roomInfos.Count; i++)
            {
                print(roomInfos[i].Name);
                GameObject room = Instantiate(roomButten,Vector3.zero,Quaternion.identity,GameObject.Find("Content").transform);
                room.GetComponent<Room>().name.text = roomInfos[i].Name;
                room.name = roomInfos[i].Name;
                buttenList.Add(room);
            }
            if (roomInfo.PlayerCount <= 0)
            {
                Destroy(GameObject.Find(roomInfo.Name));
            }
        }
    }

    public void ConparList(List<RoomInfo> list){
        for (int i = 0; i < list.Count; i++)
        {
            for (int j=list.Count-1; j>i; j--)
            {
                if (list[i] == list[j])
                {
                    list.RemoveAt(j);
                }
            }
        }
    }
}
