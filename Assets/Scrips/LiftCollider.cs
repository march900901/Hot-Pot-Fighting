using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftCollider : MonoBehaviour
{
    public void UpdateCanLift(bool trigger){
        this.transform.GetComponent<BoxCollider>().enabled = trigger;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerData>().CanLift = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerData>().CanLift = false;
        }
    }
}
