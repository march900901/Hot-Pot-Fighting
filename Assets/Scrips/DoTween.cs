using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class DoTween : MonoBehaviour
{
    public Vector3 objScale;
    private void Start() {
        objScale = this.transform.localScale;
    }
    public void ScaleButton(float scale){
        //scale.PlayForward();
        transform.DOScale(objScale*scale,0.2f);
        transform.DOScale(objScale,0.2f).SetDelay(0.1f);
    }

    public void ScalePanel(){
        this.transform.localScale = Vector3.zero;
        transform.DOScale(0.5f,0.7f).SetEase(Ease.OutCubic);
    }

    public void ShackRoomList(){
        // this.transform.rotation = new Vector3(0,0,-26);
        this.transform.DORotate(Vector3.zero,1f).SetEase(Ease.OutElastic);
    }

}
