using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class DoTween : MonoBehaviour
{
     Vector3 objScale;
     public List<GameObject> Button = new List<GameObject>();
     public float ButtonMove = 0;
    private void Start() {
        objScale = this.transform.localScale;
    }
    public void ClickButton(float scale){//按下按鈕的動畫
        transform.DOScale(objScale*scale,0.2f);
        transform.DOScale(objScale,0.2f).SetDelay(0.1f);
    }

    public void ScalePanel(){//縮放視窗動畫
        this.transform.localScale = Vector3.zero;
        transform.DOScale(1,1f).SetEase(Ease.OutElastic);
    }

    public void ShackRoomList(){//搖晃RoomList動畫
        // this.transform.rotation = new Vector3(0,0,-26);
        this.transform.DORotate(Vector3.zero,1f).SetEase(Ease.OutElastic);
    }

    public void PlayerListAni(){//PlayerList動畫
        this.transform.localScale = new Vector3(10,10,10);
        this.transform.DOScale(1 , 0.2f);
        this.transform.DORotate(Vector3.zero,1f).SetEase(Ease.OutElastic).SetDelay(0.2f);
    }

    IEnumerator ButtonMoveIn(){//按鈕進場動畫
        
        foreach (var item in Button)
        {
            item.transform.localPosition = new Vector3(item.transform.localPosition.x,item.transform.localPosition.y-250,0);
        }
        yield return new WaitForSeconds(0.4f);
        foreach (var item in Button)
        {
            float targate = item.transform.localPosition.y+250;
            item.transform.DOMoveY(ButtonMove,0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    public void ScaleButton(float scaleTime){
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(new Vector3(1,1,1),scaleTime);
    }

    public void PanelIn(){
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(1,0.2f);
    }

    public void PanelOut(){
        this.transform.localScale = new Vector3(1,1,1);
        this.transform.DOScale(Vector3.zero,0.2f);
    }

    public void ConfirmIn(){
        this.transform.localScale = new Vector3(10,10,10);
        this.transform.DOScale(1,0.2f);
    }

}
