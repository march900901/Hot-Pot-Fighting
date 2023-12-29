using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform targate;

    void Start() {
        targate = GameObject.FindWithTag("MainCamera").transform;    
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(targate,Vector3.up);
    }
}
