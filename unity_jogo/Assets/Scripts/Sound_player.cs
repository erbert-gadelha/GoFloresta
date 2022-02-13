using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*          SOUND INDEXES
 *  0 - roe
 *  1 - dirt
 *  2 - correct
 *  3 - water
 *  4 - scisor
 *  5 - machado
 *  6 - colher
 */

public class Sound_player : MonoBehaviour
{
    public static Sound_player player;
    AudioSource [] source;
    [SerializeField]
    AudioClip[] audios;

    void Awake()
    {
        player = this;
        source = new AudioSource[audios.Length];

        for (int i = 0; i < source.Length; i++)
            source[i] = gameObject.AddComponent<AudioSource>();

        source[0].volume = 0.5f;
    }


    public void play (int index)
    {
        source[index].Stop();
        source[index].clip = audios[index];
        source[index].Play();
    }
}
