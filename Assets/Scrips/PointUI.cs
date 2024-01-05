using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointUI : MonoBehaviour
{
    public int Life;
    //public List<GameObject> Icon = new List<GameObject>();
    public GameObject[] Icon;
    public Text pointText;
    public PlayerData _pd;
    public GameManager _gm;
    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Icon = GameObject.FindGameObjectsWithTag("Life");
    }

    public void UpDatePointUI(){
        switch(_gm._gr){
            case GameManager.GameRull.TIME:
                
            break;

            case GameManager.GameRull.LIVE:
                //Life += point;
                Icon[Mathf.Abs(Life-2)].SetActive(false);
            break;
        }

        
        //Icon[Life].GetComponent<DoTween>();
    }
}
