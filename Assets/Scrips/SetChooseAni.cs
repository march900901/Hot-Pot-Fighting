using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetChooseAni : MonoBehaviour
{
    Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name == "SelectCharacter")
        {
            _animator.SetTrigger("choose");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
