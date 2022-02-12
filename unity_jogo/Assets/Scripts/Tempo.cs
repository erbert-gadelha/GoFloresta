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


    [SerializeField]
    GameObject[] polinizadores;


    private void Awake()
    {
        tempo = this;
    }

    void Start() {
        //Debug.LogWarning("No CastingShadow");
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

        Board.board.castShadows(obj.transform.forward * (-1));
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

    public float vel_polinsz = 0.1f;

    IEnumerator _polinizador(Vector2Int pos) {
        GameObject aux = polinizadores[0];

        Vector2 rdm2 = Random.insideUnitCircle * 5;
        Vector3 rdm3 = new Vector3(rdm2.x, 0, rdm2.y);

        aux = Instantiate(aux, rdm3, Quaternion.identity);
        ParticleSystem particle = aux.transform.GetChild(0).GetComponent<ParticleSystem>();
        particle.Play();


        Vector3 _pos = new Vector3(pos.x, 0, pos.y);
        aux.transform.LookAt(_pos);

        //aux.transform.localEulerAngles = new Vector3(0, Mathf.Atan2(_pos.x - rdm3.x, _pos.z - rdm3.z) * -Mathf.Rad2Deg, 0);

        while (aux.transform.position != _pos) {
            aux.transform.position = Vector3.MoveTowards(aux.transform.position, _pos, vel_polinsz);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.45f);
        Debug.Log("Coisar a planta");
        particle.Stop();


        rdm2 = Random.insideUnitCircle * 5;
        rdm3 = new Vector3(rdm2.x, 0, rdm2.y);

        aux.transform.LookAt(rdm3);
        //aux.transform.localEulerAngles = new Vector3(0, Mathf.Atan2(_pos.x - rdm3.x, _pos.z - rdm3.z) * -Mathf.Rad2Deg, 0);


        while (aux.transform.position != rdm3) {
            aux.transform.position = Vector3.MoveTowards(aux.transform.position, rdm3, vel_polinsz);
            yield return new WaitForFixedUpdate();
        }


        //particle.Stop();
        //Destroy(aux);

        yield return null;
    }


    void Update()
    {
        if (Input.GetKeyDown("e"))
            StartCoroutine(_polinizador(new Vector2Int( (int)(Random.value * 14), (int)(Random.value * 19) )));
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
