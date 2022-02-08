using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    [SerializeField]
    GameObject[] hud;


    void FixedUpdate()
    {
        int w, h;
        w = Screen.width;
        h = Screen.height;


        hud[0].SetActive(h>w);
        hud[1].SetActive(w>h);
    }

    public void load_scene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void change_to(int to)
    {
        for (int i = 0; i < 3; i++)
        {
            hud[0].transform.GetChild(i).gameObject.SetActive(false);
            hud[1].transform.GetChild(i).gameObject.SetActive(false);
        }

        hud[0].transform.GetChild(to).gameObject.SetActive(true);
        hud[1].transform.GetChild(to).gameObject.SetActive(true);
    }
}
