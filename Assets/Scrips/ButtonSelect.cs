using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonSelect : MonoBehaviour
{
    public Vector3 Scale;

    void OnMouseEnter() {
        Scale = transform.localScale;
        print("MouseEnter");
        
    }

    void OnMouseExit() {
        transform.DOScale(Scale,0.2f);
    }

    public void SelectButton(){
        transform.DOScale(Scale*2f,0.2f);
    }
}
