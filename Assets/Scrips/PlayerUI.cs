using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image playerImage;
    public List<Sprite> playerImages;
    public PlayerData _pd;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpDatePlayerImage(){
        foreach (var item in playerImages)
        {
            // print(_pd.name);
            if (item.name == _pd.name)
            {
                playerImage.sprite = item;
            }
        }
    }
}
