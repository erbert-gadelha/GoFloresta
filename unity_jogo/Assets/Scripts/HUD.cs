using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD hud;

    [SerializeField]
    RectTransform[] huds;
    RectTransform[,] card;
    RectTransform descrpt;


    [SerializeField]
    Text[][] texts;
    [SerializeField]
    Image[][] images;

    [SerializeField]
    Text[] texts0, texts1;
    [SerializeField]
    Image[] images0, images1;

    int open_in = -1;
    bool inventory_opened = false;

    [SerializeField]
    DeviceOrientation orientacao, estilo = DeviceOrientation.Portrait;

    enum Plataforma { ANDROID, WINDOWS }
    [SerializeField]
    Plataforma plataforma = Plataforma.ANDROID;

    [SerializeField]
    int orientation;


    void Awake()
    {
        estilo = DeviceOrientation.Portrait;
        orientation = 0;

        hud = this;
        get_components();
    }

    void Start()
    {
        change_hand(null);
        Moviment_2.player.equipar(null);
    }

    private void FixedUpdate()
    {
        check_orientation();
    }

    void get_components()
    {
        texts = new Text[2][];
        texts[0] = new Text[15];
        texts[1] = new Text[15];

        images = new Image[2][];
        images[0] = new Image[15];
        images[1] = new Image[15];

        _status = new RectTransform[2];

        for (int i = 0; i < 2; i++)
        {
            int txt = 0, img = 0;
            //STATUS
             texts[i][txt++] = huds[i].GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>();
            images[i][img++] = huds[i].GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>();

            //EQUIPADO - MAO
            images[i][img++] = huds[i].GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetComponent<Image>();
            images[i][img++] = huds[i].GetChild(0).GetChild(4).GetChild(0).GetChild(1).GetComponent<Image>();
             texts[i][txt++] = huds[i].GetChild(0).GetChild(4).GetChild(0).GetChild(2).GetComponent<Text>();

            //EQUIPADO - MENU
            images[i][img++] = huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
             texts[i][txt++] = huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>();
             texts[i][txt++] = huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>();

            _status[i] = huds[i].GetChild(0).GetChild(3).GetComponent<RectTransform>();
        }

        images0 = images[0];
        images1 = images[1];
        texts0 = texts[0];
        texts1 = texts[1];

        descrpt = huds[orientation].GetChild(1).GetChild(0).GetChild(1).GetComponent<RectTransform>();
        Transform aux0 = huds[0].GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0);
        Transform aux1 = huds[1].GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0);
        int size = aux0.childCount;

        card = new RectTransform[2, size];
        for (int i = 0; i < size; i++)
        {
            card[0, i] = aux0.GetChild(i).GetComponent<RectTransform>();
            card[1, i] = aux1.GetChild(i).GetComponent<RectTransform>();
        }

        open_in = -1;

        Debug.LogWarning("Precisa mexer no fill");
        _fill_cards(0);
        _close();
    }

    public void check_orientation()
    {
        if (plataforma == Plataforma.WINDOWS) {
            if (orientacao != estilo)
            {
                orientation = -1;
                switch (estilo)
                {
                    case DeviceOrientation.Portrait:
                        change_orientation(0);
                        break;
                    case DeviceOrientation.LandscapeLeft:
                        change_orientation(1);
                        break;
                    case DeviceOrientation.LandscapeRight:
                        change_orientation(1);
                        break;
                }

                orientacao = estilo;
            }

        } else if (plataforma == Plataforma.ANDROID) {

            if (orientacao != Input.deviceOrientation)
            {
                orientacao = Input.deviceOrientation;

                switch (orientacao)
                {
                    case DeviceOrientation.Portrait:
                        estilo = DeviceOrientation.Portrait;
                        change_orientation(0);
                        break;

                    case DeviceOrientation.PortraitUpsideDown:
                        estilo = DeviceOrientation.Portrait;
                        change_orientation(0);
                        break;

                    case DeviceOrientation.LandscapeLeft:
                        estilo = DeviceOrientation.LandscapeRight;
                        change_orientation(1);
                        break;

                    case DeviceOrientation.LandscapeRight:
                        estilo = DeviceOrientation.LandscapeRight;
                        change_orientation(1);
                        break;

                    default:
                        // NADA ACONTECE
                        break;
                }


            }
        }
    }
    
    public void change_orientation(int orientation)
    {
        if (this.orientation == orientation)
            return;

        this.orientation = orientation;
        int aux = open_in;

        open_in = -1;
        _fill_cards(aux);

        switch (orientation)
        {
            case 0:
                huds[0].gameObject.SetActive(true);
                huds[1].gameObject.SetActive(false);
                break;
            case 1:
                huds[0].gameObject.SetActive(false);
                huds[1].gameObject.SetActive(true);
                break;
        }
    }

    public void _open()
    {
        huds[0].GetChild(0).gameObject.SetActive(false);
        huds[1].GetChild(0).gameObject.SetActive(false);
        huds[0].GetChild(1).gameObject.SetActive(true);
        huds[1].GetChild(1).gameObject.SetActive(true);

        inventory_opened = true;
        Debug.LogWarning("Para tempo");
    }
    public void _close()
    {
        huds[0].GetChild(0).gameObject.SetActive(true);
        huds[0].GetChild(1).gameObject.SetActive(false);
        huds[1].GetChild(0).gameObject.SetActive(true);
        huds[1].GetChild(1).gameObject.SetActive(false);

        inventory_opened = false;
    }

    public void _pressed(int index)
    {
        Item aux;
        switch (open_in)
        {
            case 0:     aux = Inventory.inventory.itens_0[index]; break;   //FERRAMENTAS
            case 1:     aux = Inventory.inventory.itens_1[index]; break;   //ITENS 0
            case 2:     aux = Inventory.inventory.itens_2[index]; break;   //ITENS 1
            case 3:     aux = Inventory.inventory.itens_3[index]; break;   //ITENS 2
            default:    aux = Inventory.inventory.itens_4[index]; break;   //LOJA
        }

        change_hand(aux);
        Moviment_2.player.equipar(aux);
    }







    Vector3 _status_pos;
    bool _status_visibility;
    [SerializeField]
    RectTransform [] _status;


    public void status_visibility(bool param)
    {
        _status_visibility = param;
        _status[0].gameObject.SetActive(false);
        _status[1].gameObject.SetActive(false);
    }

    public void change_to(Vector3 pos, bool visible)
    {
        _status_pos = pos;

        Tile tile = Board.board.get(pos);

        if (tile == null)
        {
            status_visibility(false);
            return;
        }

        if (tile.parent != null)
            tile = tile.parent;

        Tree aux = tile.tree;
        if (aux == null)
        {
            status_visibility(false);
            return;
        }

        float aux1 = tile.curnt_age, aux2 = aux.age * aux.stages.Length;

        //name.text = aux.name;
        texts[0][0].text = aux.name;
        //slide.value = (aux1 / aux2);
        //img.sprite = aux.icon;
        images[0][0].sprite = aux.icon;


        status_visibility(visible);
    }


    public void Refresh()
    {
        if (!_status_visibility)
            return;

        Tile tile = Board.board.get(_status_pos);
        if (tile == null)
        {
            status_visibility(false);
            return;
        }

        if (tile.parent != null)
            tile = tile.parent;

        Tree aux = tile.tree;
        if (aux == null)
        {
            status_visibility(false);
            return;
        }

        float aux1 = tile.curnt_age, aux2 = aux.age * aux.stages.Length;


        //name.text = aux.name;
        texts[0][0].text = aux.name;
        //slide.value = (aux1 / aux2);
        //img.sprite = aux.icon;
        images[0][0].sprite = aux.icon;

        status_visibility(true);
    }













    bool equipado;
    public void change_hand(Item item)
    {
        if (item == null)
        {
            equipado = false;

            for (int i = 0; i < 2; i++)
            {
                images[i][1].enabled = false;
                images[i][2].enabled = false;
                images[i][3].enabled = false;
                 texts[i][1].enabled = false;
                 texts[i][2].enabled = false;
                 texts[i][3].enabled = false;
            }

            return;
        }

        if(!equipado)
        {
            equipado = true;
            for (int i = 0; i < 2; i++)
            {
                images[i][1].enabled = true;
                images[i][2].enabled = true;
                images[i][3].enabled = true;
                 texts[i][1].enabled = true;
                 texts[i][2].enabled = true;
                 texts[i][3].enabled = true;
            }

        }

        for (int i = 0; i < 2; i++)
        {
             texts[i][2].text = item.name;
             texts[i][3].text = item.description;
            images[i][2].sprite = item.icon;
            images[i][3].sprite = item.icon;
        }


    }

    public void _fill_cards(int tab)
    {
        if (tab == open_in)
            return;

        Item[] aux;
        open_in = tab;

        switch (tab)
        {
            case 0:     aux = Inventory.inventory.itens_0; break;   //FERRAMENTAS
            case 1:     aux = Inventory.inventory.itens_1; break;   //ITENS 0
            case 2:     aux = Inventory.inventory.itens_2; break;   //ITENS 1
            case 3:     aux = Inventory.inventory.itens_3; break;   //ITENS 2
            default:    aux = Inventory.inventory.itens_4; break;   //LOJA
        }

        int i = 0;
        while (i < aux.Length)
        {
            card[orientation, i].GetChild(0).GetChild(0).GetComponent<Image>().sprite = aux[i].icon; // icon
            card[orientation, i].GetChild(1).GetComponent<Text>().text = aux[i].name;                // title
            card[orientation, i].GetChild(2).GetComponent<Text>().text = aux[i].description;         // description

            card[orientation, i].gameObject.SetActive(true);
            i++;
        }

        while (i < card.GetLength(1))
        {
            card[orientation, i].gameObject.SetActive(false);
            i++;
        }
    }
}
