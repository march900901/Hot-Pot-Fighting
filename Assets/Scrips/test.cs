using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public List<string> strings = new List<string>{"0","5","4","5","0","7","9"};
    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        //ConparList(strings);
        ConparList2();
        foreach (var item in strings)
        {
            print(item);
        }
        Rotate90(cube);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        print(this.gameObject.name + "collistion");
    }

    public void ConparList(List<string> list){
        HashSet<string> hs = new HashSet<string>(list);
        foreach (string HashSet in hs)
        {
            print(HashSet);
        }
        
    }

    public void ConparList2(){
        for (int i = 0; i < strings.Count; i++)
        {
            for (int j = strings.Count-1; j > i; j--)
            {
                if (strings[i] == strings[j])
                {
                    strings.RemoveAt(j);
                }
            }
        }
    }
    public void Rotate90(GameObject gameObject){
        gameObject.transform.Rotate(-90f,90f,0f,Space.Self);
    }
}
