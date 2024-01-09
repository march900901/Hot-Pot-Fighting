using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioSource> audios = new List<AudioSource>();
    public List<AudioClip> clips = new List<AudioClip>();
    AudioSource audio;
    
    

    // Start is called before the first frame update
    void Start()
    {
        audio = this.gameObject.GetComponent<AudioSource>();
        // for (int i = 0; i < 3; i++)
        // {
        //     var audio = this.gameObject.AddComponent<AudioSource>();
        //     audios.Add(audio);
        // }
    }

    public void PlayAudio(int index,string name,bool Loop){
        var clip = GetAudioClip(name);
        if (clip != null)
        {
            var audio = audios[index];
            audio.clip = clip;
            audio.loop = Loop;
            audio.Play();
        }
    }

    AudioClip GetAudioClip(string name){
        foreach (var item in clips)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        return null;
    }
    
    public void PlayAudio(int index){
        audio.clip = clips[index];
        audio.Play();
    }
}
