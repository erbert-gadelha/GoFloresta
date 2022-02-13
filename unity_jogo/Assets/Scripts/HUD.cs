using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public static HUD hud;

    [SerializeField]
    RectTransform[] huds;
    RectTransform[,] card;

    [SerializeField]
    Text[][] texts;
    [SerializeField]
    Image[][] images;

    [SerializeField]
    Slider[] sliders;

    Vector3 _status_pos;
    bool _status_visibility;
    [SerializeField]
    RectTransform[] _status;

    int open_in = -1;







    [SerializeField]
    DeviceOrientation orientacao, estilo = DeviceOrientation.Portrait;

    enum Plataforma { ANDROID, WINDOWS }
    [SerializeField]
    Plataforma plataforma = Plataforma.ANDROID;

    [SerializeField]
    int orientation;


    void Awake()
    {
        hud = this;
        get_components();
        orientation = -1;
    }

    void Start()
    {
        change_hand(null);
    }

    private void FixedUpdate()
    {
        int w, h;
        w = Screen.width;
        h = Screen.height;

        change_orientation((h>w)?0:1);
    }
    
    [SerializeField]
    Image[] imgs1, imgs2;
    [SerializeField]
    Text [] txts1, txts2;

    Button[] colher;
    void get_components()
    {
        texts = new Text[2][];
        texts[0] = new Text[15];
        texts[1] = new Text[15];

        images = new Image[2][];
        images[0] = new Image[15];
        images[1] = new Image[15];

        _status = new RectTransform[2];
        sliders = new Slider[2];
        colher = new Button[2];

        for (int i = 0; i < 2; i++)
        {
            int txt = 0, img = 0;
            //STATUS
             texts[i][txt++] = huds[i].GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>();
            images[i][img++] = huds[i].GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>();
             texts[i][5]     = huds[i].GetChild(0).GetChild(3).GetChild(4).GetChild(0).GetComponent<Text>();

            //EQUIPADO - MAO
            images[i][img++] = huds[i].GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetComponent<Image>();
            images[i][img++] = huds[i].GetChild(0).GetChild(4).GetChild(0).GetChild(1).GetComponent<Image>();
             texts[i][txt++] = huds[i].GetChild(0).GetChild(4).GetChild(0).GetChild(2).GetComponent<Text>();

            //EQUIPADO - MENU
            images[i][img++] = huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
             texts[i][txt++] = huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>();
             texts[i][txt++] = huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>();

            images[i][img++] = huds[i].GetChild(0).GetChild(4).GetChild(1).GetChild(0).GetComponent<Image>();
            images[i][img++] = huds[i].GetChild(0).GetChild(4).GetChild(1).GetChild(1).GetComponent<Image>();
             texts[i][txt++] = huds[i].GetChild(0).GetChild(4).GetChild(1).GetChild(2).GetComponent<Text>();

            //PRECO
             texts[i][10] = huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(2).GetChild(0).GetComponent<Text>();
             texts[i][11] = huds[i].GetChild(0).GetChild(6).GetChild(1).GetComponent<Text>();
             texts[i][12] = huds[i].GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetChild(1).GetComponent<Text>();
             texts[i][13] = huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(2).GetChild(1).GetComponent<Text>();

            texts[i][11].text = Inventory.inventory.saldo.ToString();
            texts[i][12].text = Inventory.inventory.saldo.ToString();


            colher[i] = huds[i].GetChild(0).GetChild(7).GetComponent<Button>();
            _status[i] = huds[i].GetChild(0).GetChild(3).GetComponent<RectTransform>();
            sliders[i] = huds[i].GetChild(0).GetChild(3).GetChild(2).GetComponent<Slider>();
        }

        imgs1 = images[0];
        imgs2 = images[1];

        txts1 = texts[0];
        txts2 = texts[1];

        Transform aux0 = huds[0].GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0);
        Transform aux1 = huds[1].GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0);
        int size = aux0.childCount;

        cards1 = new RectTransform[size];
        cards2 = new RectTransform[size];

        card = new RectTransform[2, size];
        for (int i = 0; i < size; i++)
        {
            card[0, i] = aux0.GetChild(i).GetComponent<RectTransform>();
            card[1, i] = aux1.GetChild(i).GetComponent<RectTransform>();

            cards1[i] = card[0, i];
            cards2[i] = card[1, i];
        }



        open_in = -1;

        Debug.LogWarning("Precisa mexer no fill");
        _fill_cards(0);
        _close();
    }

    [SerializeField]
    RectTransform[] cards1;
    [SerializeField]
    RectTransform[] cards2;

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

        _fill_cards(open_in);
        Tempo.tempo.Running(false);
    }
    public void _close()
    {
        huds[0].GetChild(0).gameObject.SetActive(true);
        huds[0].GetChild(1).gameObject.SetActive(false);
        huds[1].GetChild(0).gameObject.SetActive(true);
        huds[1].GetChild(1).gameObject.SetActive(false);

        Tempo.tempo.Running(true);
    }

    public void _pressed(int index)
    {
        opened_item = new Vector2Int(open_in, index);

        item aux;
        switch (open_in)
        {
            case 0:     aux = Inventory.inventory.itens_0[index]; break;   //FERRAMENTAS
            case 1:     aux = Inventory.inventory.itens_1[index]; break;   //ITENS 0
            case 2:     aux = Inventory.inventory.itens_2[index]; break;   //ITENS 1
            default:     aux = Inventory.inventory.itens_3[index]; break;  //ITENS 2
        }

        change_hand(aux);
    }

    Vector2Int opened_item;

    public void _pause(bool open) {
        huds[orientation].gameObject.SetActive(!open);
        Tempo.tempo.Running(!open);

        huds[2].gameObject.SetActive(open);
    }

    public void _load_scene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void _colher()    {

        Board.board.colher(_status_pos);

        colher[0].interactable = false;
        colher[1].interactable = false;
        texts[0][11].text = Inventory.inventory.saldo.ToString();
        texts[0][12].text = Inventory.inventory.saldo.ToString();
        texts[1][11].text = Inventory.inventory.saldo.ToString();
        texts[1][12].text = Inventory.inventory.saldo.ToString();
    }

    public void _comprar() {

        item item = get_item();
        if (item.tipo == Inventory.tipo.FERRAMENTA)
            return;


        int valor = item.value;
        if (valor <= Inventory.inventory.saldo) {
            Inventory.inventory.bolso[opened_item.x, opened_item.y]++;
            Inventory.inventory.saldo -= valor;
        }


        Debug.Log("Comprar item por: " + valor);
        for (int i = 0; i < 2; i++) {
             huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().interactable = (valor <= Inventory.inventory.saldo);
            texts[i][4].text = ("x" + Inventory.inventory.bolso[opened_item.x, opened_item.y]);
            texts[i][13].text = ("x" + Inventory.inventory.bolso[opened_item.x, opened_item.y]);
            texts[i][11].text = Inventory.inventory.saldo.ToString();
            texts[i][12].text = Inventory.inventory.saldo.ToString();
        }

    }

    item get_item() {
        switch (opened_item.x) {
            case 0:
                return Inventory.inventory.itens_0[opened_item.y];
            case 1:
                return Inventory.inventory.itens_1[opened_item.y];
            case 2:
                return Inventory.inventory.itens_2[opened_item.y];
            default:
                return Inventory.inventory.itens_3[opened_item.y];
        }
    }



    public void status_visibility(bool param) {
        _status_visibility = param;

        _status[0].gameObject.SetActive(param);
        _status[1].gameObject.SetActive(param);

        
        colher[0].gameObject.SetActive(param);
        colher[1].gameObject.SetActive(param);
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

        Tree tree = tile.tree;
        if (tree == null)
        {
            status_visibility(false);
            return;
        }

        float value;
        if (tile.current_state < 0)
            value = 1;
        else {
            float aux1 = tile.current_age, aux2 = tile.target_age;
            value = aux1 / aux2;
        }

        texts[0][0].text = tree.name;
        texts[1][0].text = tree.name;

        texts[0][5].text = ("SOLO: " + tile.current_level.ToString());
        texts[1][5].text = ("SOLO: " + tile.current_level.ToString());


        status_visibility(true);
        sliders[0].value = value;
        sliders[1].value = value;

        colher[0].interactable = tile.com_fruto;
        colher[1].interactable = tile.com_fruto;

        images[0][0].sprite = tree.icon;
        images[1][0].sprite = tree.icon;

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

        Tree tree = tile.tree;
        if (tree == null)
        {
            status_visibility(false);
            return;
        }

        float value;
        if (tile.current_state < 0)
            value = 1;
        else
        {
            float aux1 = tile.current_age, aux2 = tile.target_age;
            value = aux1 / aux2;
        }

        texts[0][0].text = tree.name;
        texts[1][0].text = tree.name;

        texts[0][5].text = ("SOLO: " + tile.current_level.ToString());
        texts[1][5].text = ("SOLO: " + tile.current_level.ToString());


        status_visibility(true);
        sliders[0].value = value;
        sliders[1].value = value;



        colher[0].interactable = tile.com_fruto;
        colher[1].interactable = tile.com_fruto;


        images[0][0].sprite = tree.icon;
        images[1][0].sprite = tree.icon;

    }












    public void change_hand(item item)
    {

        if (item == null)
        {
            for (int i = 0; i < 2; i++)
            {
                images[i][1].enabled = false;
                images[i][2].enabled = false;
                images[i][3].enabled = false;
                images[i][4].enabled = false;
                images[i][5].enabled = false;

                 texts[i][1].enabled = false;
                 texts[i][2].enabled = false;
                 texts[i][3].enabled = false;
                 texts[i][4].enabled = false;

                huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);
            }

            return;
        }

        for (int i = 0; i < 2; i++)
        {
             texts[i][2].text = item.name;
            images[i][3].sprite = item.icon;

             texts[i][2].enabled = true;
            images[i][3].enabled = true;


            Item aux;
            if (item.tipo == Inventory.tipo.FERRAMENTA) {
                switch (item.id)
                {
                    case 0:
                        aux = new Enxada();
                        break;
                    case 1:
                        aux = new Regador();
                        break;
                    case 2:
                        aux = new Tesoura();
                        break;
                    default:
                        aux = new Machado();
                        break;
                }

                Inventory.inventory.change_tool(aux, item.id);
                //FERRAMENTA
                images[i][2].sprite = item.icon;
                images[i][1].enabled = true;
                images[i][2].enabled = true;

                texts[i][3].text = item.description;
                texts[i][3].enabled = true;


                huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);


            } else {

                Inventory.inventory.change_seed(item.seed);
                //SEMENTE


                images[i][5].sprite = item.icon;
                images[i][4].enabled = true;
                images[i][5].enabled = true;
                 texts[i][4].enabled = true;
                texts[i][4].text = ("x"+Inventory.inventory.bolso[opened_item.y, opened_item.x]);

                texts[i][3].text = item.description;
                texts[i][3].enabled = true;

                texts[i][10].text = item.value.ToString();
                texts[i][13].text = "x" + Inventory.inventory.bolso[opened_item.x, opened_item.y];
                huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(true);
                huds[i].GetChild(1).GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().interactable = (item.value <= Inventory.inventory.saldo);
            }
                
        }


    }

    public void _fill_cards(int tab)
    {

        huds[0].transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        huds[1].transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        if (tab == open_in)
            return;

        item[] aux;
        open_in = tab;

        switch (tab)
        {
            case 0:     aux = Inventory.inventory.itens_0; break;   //FERRAMENTAS
            case 1:     aux = Inventory.inventory.itens_1; break;   //ITENS 0
            case 2:     aux = Inventory.inventory.itens_2; break;   //ITENS 1
            default:     aux = Inventory.inventory.itens_3; break;  //ITENS 2
        }

        int i;
        for (int j = 0; j < 2; j++)
        {
            i = 0;

            while (i < aux.Length)
            {
                card[j, i].GetChild(0).GetChild(0).GetComponent<Image>().sprite = aux[i].icon; // icon
                card[j, i].GetChild(1).GetComponent<Text>().text = aux[i].name;                // title

                if (tab == 0) {
                    card[j, i].GetChild(2).GetComponent<Text>().text = aux[i].description;
                    card[j, i].GetChild(2).gameObject.SetActive(true);
                    card[j, i].GetChild(3).gameObject.SetActive(false);
                }
                else 
                {
                    card[j, i].GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().text = aux[i].name;
                    card[j, i].GetChild(3).GetChild(1).GetChild(0).GetComponent<Text>().text = aux[i].name;
                    card[j, i].GetChild(3).GetChild(2).GetChild(0).GetComponent<Text>().text = aux[i].name;
                    card[j, i].GetChild(3).GetChild(3).GetChild(0).GetComponent<Text>().text = aux[i].name;

                    card[j, i].GetChild(2).gameObject.SetActive(false);
                    card[j, i].GetChild(3).gameObject.SetActive(true);
                }

                card[j, i].gameObject.SetActive(true);
                i++;
            }


            RectTransform _aux;

             _aux = huds[0].transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();
            _aux.sizeDelta = new Vector2(_aux.sizeDelta.x, 226 * (i + 0.5f));

            _aux = huds[1].transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();
            _aux.sizeDelta = new Vector2(_aux.sizeDelta.x, 271 * (i + 0.5f));



            while (i < card.GetLength(1)) {
                card[j, i].gameObject.SetActive(false);
                i++;
            }
        }

        huds[0].transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        huds[1].transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

    }
}
