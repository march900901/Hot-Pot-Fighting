using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dieCollider : MonoBehaviour
{
    public GameManager gameManager;
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

    private void OnCollisionEnter(Collision other) {
        other.gameObject.GetComponent<PlayerData>()._playerState=PlayerData.PlayerState.Die;
        gameManager.CountPoint(other.gameObject.name,1);
        // if (other.gameObject.name==P1)
        // {
            
        // }
    }
}
