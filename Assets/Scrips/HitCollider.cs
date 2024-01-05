using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    public PlayerData _pd;
    //public GameManager _gm;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            _pd.OnHit(other.gameObject.GetComponent<PlayerData>());
        }

        if (other.tag == "Soup")
        {
            Instantiate(_pd.HitEffect,other.transform);
        }
        
    }
}
