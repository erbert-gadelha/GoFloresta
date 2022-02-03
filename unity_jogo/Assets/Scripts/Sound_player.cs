using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_player : MonoBehaviour
{
    public static Sound_player player;
    AudioSource [] source;
    [SerializeField]
    AudioClip[] audios;

    // Start is called before the first frame update
    void Awake()
    {
        player = this;
        source = new AudioSource[audios.Length];

        for (int i = 0; i < source.Length; i++)
            source[i] = gameObject.AddComponent<AudioSource>();

        source[2].volume = 0.1f;
        source[0].volume = 0.5f;
    }


    public void play (int index)
    {
        source[index].Stop();
        source[index].clip = audios[index];
        source[index].Play();
    }
}
