using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dieCollider : MonoBehaviour
{
    public GameSceneManager gameManager;
    public string P1;
    public string P2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DelayPointCount(float s,Collider other){
        yield return new WaitForSecondsRealtime(s);
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            //StartCoroutine(DelayPointCount(1,other));
            print("DelayPointCount");
            print(other.name + "Dead!!");
            other.gameObject.GetComponent<PlayerData>().SwitchState(PlayerData.PlayerState.Dead);
            other.gameObject.GetComponent<PlayerData>().CountingPoint();
        }
    }
}
