using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            other.GetComponent<PlayerData>().enemy = this.gameObject.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerData>().CanLift = false;
        }
    }
}
