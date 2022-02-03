using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tempo : MonoBehaviour
{

    [SerializeField]
    Transform obj;

    public static Tempo tempo;
    [SerializeField]
    List<Tile> plantas;

    [SerializeField]
    public int hora = 8;
    [SerializeField]
    float delay = 1;


    void Start() {
        SetTo(hora);

        tempo = this;
        plantas = new List<Tile>();

        StartCoroutine( _clock());
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
        for(int i = obj.childCount-1; i >= 0; i--)
            obj.GetChild(i).gameObject.SetActive(false);


        if ((to > 6) & (to <= 18))         //MANHA
            obj.GetChild(1).gameObject.SetActive(true);
        else                             //NOITE
            obj.GetChild(3).gameObject.SetActive(true);

        //if ((to > 4) & (to <= 8))         //MANHA
        //    obj.GetChild(0).gameObject.SetActive(true);
        //else if ((to > 8) & (to <= 15))  //DIA
        //    obj.GetChild(1).gameObject.SetActive(true);
        //else if ((to > 15) & (to <= 20))  //TARDE
        //    obj.GetChild(2).gameObject.SetActive(true);
        //else                             //NOITE
        //    obj.GetChild(3).gameObject.SetActive(true);

    }

    void Refresh()
    {
        obj.GetChild(1).gameObject.SetActive(false);
        obj.GetChild(3).gameObject.SetActive(false);

        if ((hora > 6) & (hora <= 18))         //MANHA
            obj.GetChild(1).gameObject.SetActive(true);
        else                             //NOITE
            obj.GetChild(3).gameObject.SetActive(true);
        return;


        switch (hora)
        {
            case 5:
                obj.GetChild(3).gameObject.SetActive(false);
                obj.GetChild(0).gameObject.SetActive(true);
                break;
            case 8:
                obj.GetChild(0).gameObject.SetActive(false);
                obj.GetChild(1).gameObject.SetActive(true);
                break;
            case 15:
                obj.GetChild(1).gameObject.SetActive(false);
                obj.GetChild(2).gameObject.SetActive(true);
                break;
            case 20:
                obj.GetChild(2).gameObject.SetActive(false);
                obj.GetChild(3).gameObject.SetActive(true);
                break;
        }

    }


    

    [SerializeField]
    RectTransform hr, mn;
    IEnumerator _clock() {

        int mnt = 0;

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
        if(obj.GetChild(3).gameObject.active)
            if (GUI.Button(new Rect(50, 70, 80, 40), "Dormir"))
                SetTo(8);
    }
}
