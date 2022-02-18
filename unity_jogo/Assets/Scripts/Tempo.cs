using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tempo : MonoBehaviour
{

    [SerializeField]
    Animator obj, cortina;

    bool running = false;
    public static Tempo tempo;
    [SerializeField]
    public List<Tile> plantas;

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

        _transicao_ = _color_to(cores[0]);
        StartCoroutine(_transicao_);
        SetTo(hora);

        tempo = this;
        plantas = new List<Tile>();

        Invoke("polinizar", Random.Range(5, 10));
        Running(true);
    }

    public void Add(Tile planta)
    {   plantas.Add(planta);    }
    public void Remove(Tile planta)
    {   plantas.Remove(planta); }
    
    

    public void Skip()
    {
        for (int i = 0; i < plantas.Count; i++)
            plantas[i].grow(!noite);

        HUD.hud.Refresh();

        Board.board.castShadows(obj.transform.forward * (-1));
    }

    public void SetTo(int to)
    {
        hora = to;
        hr0.eulerAngles = new Vector3(0, 0, -(hora * 720) / 24);
        hr1.eulerAngles = new Vector3(0, 0, -(hora * 720) / 24);


        noite = (to<=4)|(to>20);

        if ((to > 4) & (to <= 8))
        {         //MANHA
            obj.SetTrigger("dia");
            StopCoroutine(_transicao_);
            _transicao_ = _color_to(cores[0]);
            StartCoroutine(_transicao_);

            if (plantas.Count > 0)
            {
                for (int i = 0; i < plantas.Count; i++)
                    plantas[i].secar();

                print("alo mocada1");
            }

        }
        else if ((to >= 15) & (to <= 20))
        {  //TARDE
            obj.SetTrigger("tarde");
            StopCoroutine(_transicao_);
            _transicao_ = _color_to(cores[1]);
            StartCoroutine(_transicao_);
        }
        else
        {                            //NOITE
            obj.SetTrigger("noite");
            StopCoroutine(_transicao_);
            _transicao_ = _color_to(cores[2]);
            StartCoroutine(_transicao_);
        }

    }
    public IEnumerator _SetTo(int to) {
        cortina.SetTrigger("fechar");
        yield return new WaitForSeconds(0.5f);
        SetTo(to);
        yield return new WaitForSeconds(0.5f);
        cortina.SetTrigger("abrir");
    }

    public Color[] cores;
    void Refresh()
    {
        switch (hora)
        {
            case 5:
                obj.SetTrigger("dia");
                StopCoroutine(_transicao_);
                _transicao_ = _color_to(cores[0]);
                StartCoroutine(_transicao_);
                noite = false;


                if (plantas.Count > 0)
                {
                    for (int i = 0; i < plantas.Count; i++)
                        plantas[i].secar();

                    print("alo mocada2");
                }
                break;
            case 18:
                obj.SetTrigger("tarde");
                StopCoroutine(_transicao_);
                _transicao_ = _color_to(cores[1]);
                StartCoroutine(_transicao_);
                noite = true;
                break;
            case 20:
                obj.SetTrigger("noite");
                StopCoroutine(_transicao_);
                _transicao_ = _color_to(cores[2]);
                StartCoroutine(_transicao_);
                break;
        }

    }

    public float vel;
    IEnumerator _color_to(Color to) {

        while (Camera.main.backgroundColor != to) {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, to, vel);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator _relogio_, _transicao_;
    public void Running(bool param)
    {
        if (param == running)
            return;

        running = param;

        if (!running) {
            StopCoroutine(_relogio_);
        } else {
            _relogio_ = _clock();
            StartCoroutine(_relogio_);
        }
    }

    public float vel_polinsz = 0.1f;

    void polinizar()
    {
        print("no invoke");

        if (plantas.Count > 0)
        {
            int randon = Random.Range(0, plantas.Count - 1);
            StartCoroutine(_polinizador( plantas[randon].position));
        }
        else
        {

            Invoke("polinizar", Random.Range(5, 10));
            print("no invoke:sem planta");
        }
    }

    IEnumerator _polinizador(Vector2Int pos) {
        if (noite)
        {
            Invoke("polinizar", Random.Range(5, 20));
            print("no invoke:noite");
            yield break;
        }

        // pitanga_7, cajá_4, acerola_9, coco_11, caju_5
        int aux;
        Tile tile = Board.board.get(pos.x, pos.y);
        switch (tile.tree.id)
        {
            case 4:     aux = 0; break;
            case 5:     aux = 0; break;
            case 7:     aux = 0; break;
            case 9:     aux = 0; break;
            case 11:    aux = 0; break;
            case 12:    aux = 1; break;
            default:    aux = -1; break;
        }

        if (aux < 0)
        {
            Invoke("polinizar", Random.Range(5, 20));
            print("no invoke:sem polinizador");
            yield break;
        }



        GameObject gO = polinizadores[aux];

        Vector2 rdm2 = Random.insideUnitCircle * 5;
        Vector3 rdm3 = new Vector3(rdm2.x, 0, rdm2.y);

        gO = Instantiate(gO, rdm3, Quaternion.identity);
        ParticleSystem particle = gO.transform.GetChild(0).GetComponent<ParticleSystem>();
        particle.Play();


        Vector3 _pos = new Vector3(pos.x, 0, pos.y);
        gO.transform.LookAt(_pos);


        while (gO.transform.position != _pos) {
            gO.transform.position = Vector3.MoveTowards(gO.transform.position, _pos, vel_polinsz);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.45f);


        //Tile tile = Board.board.get(pos.x, pos.y);
        //if (tile.parent_ != null)
         //   tile = tile.parent_;

        if (tile != null)
        {
            Invoke("polinizar", Random.Range(20, 40));
            for (int i = 0; i < 10; i++)
                tile.grow(true);
        }
        else
        {
            Invoke("polinizar", Random.Range(5, 10));
            print("no invoke:nulo");
        }

        Debug.Log("Coisar a planta");
        Destroy(particle);


        rdm2 = Random.insideUnitCircle * 5;
        rdm3 = new Vector3(rdm2.x, 0, rdm2.y);
        gO.transform.LookAt(rdm3);



        while (gO.transform.position != rdm3) {
            gO.transform.position = Vector3.MoveTowards(gO.transform.position, rdm3, vel_polinsz);
            yield return new WaitForFixedUpdate();
        }

        Destroy(gO);
        yield return null;
    }

    public bool noite;

    [SerializeField]
    RectTransform hr0, mn0;
    [SerializeField]
    RectTransform hr1, mn1;
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

                hr0.eulerAngles = new Vector3(0, 0, -(hora * 720) / 24);
                hr1.eulerAngles = new Vector3(0, 0, -(hora * 720) / 24);
                Sound_player.player.play(2);

                Skip();
                Refresh();
            }

            mn0.eulerAngles = new Vector3(0, 0, -(mnt * 360) / 12);
            mn1.eulerAngles = new Vector3(0, 0, -(mnt * 360) / 12);
            yield return new WaitForSeconds(delay);
        }

    }

}
