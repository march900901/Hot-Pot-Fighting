using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Destroy(this.gameObject,300);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Start")
        {
            if (this.gameObject.name == "HotPotBGM")
            {
                this.gameObject.GetComponent<AudioSource>().Stop();
            }else{
                //this.gameObject.GetComponent<AudioSource>().Stop();
            }
        }
    }
}
