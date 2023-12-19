using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sauceCollition : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            other.GetComponent<PlayerData>().SwitchState(PlayerData.PlayerState.CantMove);
            other.GetComponent<PlayerContaller>().StopAllCoroutines();
            print("Sauce");
        }
        
    }
}
