using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SauceGeneerate : MonoBehaviour
{
    public float generateCount; 
    public float betweenGenerate;

    // Update is called once per frame
    private void Start() {
        generateCount = betweenGenerate;
    }
    void Update()
    {
        generateCount -= Time.deltaTime;
        if (generateCount <= 0)
        {
            SponSauceGenerate();
            generateCount = betweenGenerate;
        }
    }

    public void SponSauceGenerate(){
        Vector3 generateRang = new Vector3(Random.Range(-5,5),this.transform.position.y,Random.Range(-5,5));
        GameObject Garlic = PhotonNetwork.Instantiate("SuaceGrenerate",generateRang,Quaternion.identity);
        Destroy(Garlic,10 );
    }
}
