using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tempo : MonoBehaviour
{

    [SerializeField]
    Animator obj;

    bool running = false;
    public static Tempo tempo;
    [SerializeField]
    List<Tile> plantas;

    [SerializeField]
    public int hora = 8, mnt = 0;
    [SerializeField]
    float delay = 1;

    private void Awake()
    {
        tempo = this;
    }

    void Start() {
        SetTo(hora);

        tempo = this;
        plantas = new List<Tile>();

        Running(true);
    }

    public void Add(Tile planta)
    {   plantas.Add(planta);    }
    public void Remove(Tile planta)
    {   plantas.Remove(planta); }
    public void Skip()
    {
        for (int i = 0; i < plantas.Count; i++)
            plantas[i].grow();

        HUD.hud.Refresh();
    }

    public void SetTo(int to)
    {
        hora = to;
        hr.eulerAngles = new Vector3(0, 0, -(hora * 720) / 24);


        if ((to > 4) & (to <= 8))         //MANHA
            obj.SetTrigger("dia");
        else if ((to >= 15) & (to <= 20))  //TARDE
            obj.SetTrigger("tarde");
        else                             //NOITE
            obj.SetTrigger("noite");

    }

    void Refresh()
    {
        switch (hora)
        {
            case 5:
                obj.SetTrigger("dia");
                break;
            case 18:
                obj.SetTrigger("tarde");
                break;
            case 20:
                obj.SetTrigger("noite");
                break;
        }

    }

    IEnumerator a;
    public void Running(bool param)
    {
        if (param == running)
            return;

        running = param;

        if (!running) {
            StopCoroutine(a);
        } else {
            a = _clock();
            StartCoroutine(a);
        }
    }





    
    [SerializeField]
    RectTransform hr, mn;
    IEnumerator _clock() {

        while (true)
        {
            mnt++;

            if(mnt>=12)
            {
                mnt = 0;
                hora++;

                if (hora >= 24)
                    hora = 0;

                hr.eulerAngles = new Vector3(0, 0, -(hora * 720) / 24);
                Sound_player.player.play(2);

                Skip();
                Refresh();
            }

            mn.eulerAngles = new Vector3(0, 0, -(mnt * 360) / 12);
            yield return new WaitForSeconds(delay);
        }

    }


    
    private void OnGUI()
    {
        if((hora > 18) | (hora<5))
            if (GUI.Button(new Rect(50, 70, 80, 40), "Dormir"))
                SetTo(8);
    }
}
