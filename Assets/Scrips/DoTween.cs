using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;


public class DoTween : MonoBehaviour
{
     Vector3 objScale;
     public Transform panelHome;
     public List<GameObject> TweenObjects = new List<GameObject>();
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
        
        foreach (var item in TweenObjects)
        {
            item.transform.localPosition = new Vector3(item.transform.localPosition.x,item.transform.localPosition.y-250,0);
        }
        yield return new WaitForSeconds(0.4f);
        foreach (var item in TweenObjects)
        {
            float targate = item.transform.localPosition.y+250;
            item.transform.DOMoveY(ButtonMove,0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    // IEnumerator LobbyPanleIn(){
    //     foreach (var item in TweenObjects)
    //     {
    //         item.transform.localPosition = TweenObjects[0].transform.position;
    //     }
    //     yield return new WaitForSeconds(0.4f);
    //     TweenObjects[1].transform.DOMoveY(9.2f,0.1f).SetEase(Ease.OutBounce);
    //     TweenObjects[2].transform.DOMoveY(-92.8f,0.3f).SetEase(Ease.OutBounce);
    // }

    public void LobbyPanleIn(){
        Vector3 StartPos = new Vector3(TweenObjects[0].transform.position.x,TweenObjects[0].transform.position.y+10,TweenObjects[0].transform.position.z);
        TweenObjects[3].transform.localScale = new Vector3(1,0,1);
        for (int i = 0; i < TweenObjects.Count; i++)
        {
            TweenObjects[i].transform.position = StartPos;
        }
        TweenObjects[0].transform.DOMoveY(730,1).SetEase(Ease.OutBounce);
        TweenObjects[1].transform.DOMoveY(563,1).SetEase(Ease.OutBounce);
        TweenObjects[2].transform.DOMoveY(319,1).SetEase(Ease.OutBounce);
        TweenObjects[3].transform.DOScaleY(1,1);
    }

    public void ScaleButton(float scaleTime){
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(new Vector3(1,1,1),scaleTime);
    }

    public void PanelIn(){
        this.transform.localScale = Vector3.zero;
        if (panelHome != null)
        {
            transform.position = panelHome.position;
        }else{
            panelHome.position = new Vector3(Screen.width/2,Screen.height/2,0);
        }
        transform.DOMove(new Vector3(Screen.width/2,Screen.height/2,0),0.2f);
        this.transform.DOScale(1,0.2f);
    }

    public void PanelOut(){
        this.transform.localScale = new Vector3(1,1,1);
        transform.DOMove(panelHome.position,0.2f);
        this.transform.DOScale(Vector3.zero,0.2f);
    }

    public void ConfirmIn(){
        this.transform.localScale = new Vector3(10,10,10);
        this.transform.DOScale(1,0.2f);
    }

    public void ButtonScaleIn(){
        transform.DOScale(objScale,0.2f);
    }

    public void ButtonScaleOut(){
        transform.DOScale(Vector3.zero,0.2f);
    }

    public void SelectGameMode(float scale){
        transform.DOScale(objScale*scale,0.2f);
    }

    public void DisSelectGameMode(){
        transform.DOScale(objScale,0.2f);
    }

    public void ReduceLife(){
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        transform.DOMoveY(transform.position.y-85,1);
    }

}
