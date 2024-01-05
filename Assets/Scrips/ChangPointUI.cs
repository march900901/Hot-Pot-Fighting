using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangPointUI : MonoBehaviour
{
    public List<GameObject> ui = new List<GameObject>();
    public GameManager _gm;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        int gameMode = PlayerPrefs.GetInt("GameMode");
        GameObject pointUI = Instantiate(ui[gameMode],this.transform);//按照模式生成不同UI
        _gm._pointUI = pointUI.GetComponent<PointUI>();//將生成的UI傳給gm

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
