using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SponSauce : MonoBehaviour
{   
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Table")
        {
            PhotonNetwork.Instantiate("sauce",this.transform.position,Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
