using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class dieCollider : MonoBehaviour
{
    public GameManager _gm;
    public GameObject particle;
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
        print("DelayPointCount");
        print(other.name + "Dead!!");
        other.gameObject.GetComponent<PlayerData>().SwitchState(PlayerData.PlayerState.Dead);
        other.gameObject.GetComponent<PlayerData>().CountingPoint();
}

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            GameObject smoke = Instantiate(particle,other.transform);
            smoke.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DelayPointCount(1,other));
            
        }
    }
}
