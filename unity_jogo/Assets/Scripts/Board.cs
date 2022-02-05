using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board board;

    public enum materials { arado, grama, molhado, ocupado };

    [SerializeField]
    public enum state { ARADO, VAZIO, PLANTADO }


    [HideInInspector]
    public Tile[,] terrains;

    [SerializeField]
    GameObject [] prefs;

    [SerializeField]
    Vector3Int offset;

    [SerializeField]
    Mesh[] material;



    GameObject[] red;
    GameObject[] grn;
    GameObject[] wht;

    GameObject _hints;
    GameObject [,] hint;





    void Awake()
    {
        board = this;
        create();
        hints();
    }

    private void create()
    {        
        terrains = new Tile[20, 20];
    }

    public bool add(Tile terrain, int x, int y)
    {
        if (terrains == null)
            create();

        if (x < 0 | y < 0 | x > terrains.GetLength(0) | y > terrains.GetLength(1))
        {
            return false;
        }

        terrains[y, x] = terrain;
        return false;
    }

    public Tile get(int x, int y)
    {
        if (terrains == null)
            create();

        if (x < 0 | y < 0 | x > terrains.GetLength(0) | y > terrains.GetLength(1))
        {
            return null;
        }

        return terrains[y, x];
    }

    public Tile get(Vector3 pos)
    {
        return get((int)pos.x, (int)pos.z);
    }

    public bool plant(Tree tree, Vector3 pos, int rotation)
    {
        Tile aux = board.get(pos);
        if (!aux.arado | aux.obj != null)
            return false;

        aux.childs = new Tile[(tree.size.x * tree.size.y)];
        int i = 0;

        float _x = 0, _y = 0;
        for (int X = 0; X < tree.size.x; X++)
        {
            for (int Y = 0; Y < tree.size.y; Y++)
            {
                switch (rotation)
                {
                    case 0: //BAIXO
                        aux.childs[i] = board.get(aux.position.x - X, aux.position.y - Y);
                        break;
                    case 1: // ESQUERDA
                        aux.childs[i] = board.get(aux.position.x - Y, aux.position.y - X);
                        break;
                    case 2: // CIMA
                        aux.childs[i] = board.get(aux.position.x + X, aux.position.y + Y);
                        break;
                    case 3: // DIREITA
                        aux.childs[i] = board.get(aux.position.x + Y, aux.position.y + X);
                        break;
                }

                if (aux.childs[i] == null)
                {
                    print("nulo aqui");
                    return false;
                }

                _x += aux.childs[i].position.x;
                _y += aux.childs[i].position.y;

                change_mat(aux.childs[i].position, materials.ocupado);
                aux.childs[i].parent = aux;

                i++;
            }
        }

        aux.parent = null;
        aux.center = new Vector3( _x/(tree.size.x * tree.size.y), 0, _y/(tree.size.x * tree.size.y));
        change_mat(aux.position, materials.arado);

        aux.tree = tree;
        aux.curnt_age = -1;
        aux.currt_state = 1;
        aux.rotation = rotation;

        aux.madura = 0;
        Tempo.tempo.Add(aux);
        aux.Refresh();

        return true;
    }


    int random = 1;
    public void change_mat(Vector2Int pos, materials material)
    {
        Tile aux = get(pos.x, pos.y);

        switch (material)
        {
            case materials.arado:
                aux.arado = true;
                aux.mesh.mesh = this.material[0];
                break;
            case materials.grama:
                aux.arado = false;
                aux.mesh.mesh = this.material[random];

                random++;
                if (random >= this.material.Length)
                    random = 1;
                
                break;
            case materials.molhado:
                aux.arado = true;
                aux.mesh.mesh = this.material[0];
                break;
            case materials.ocupado:
                aux.arado = true;
                aux.mesh.mesh = this.material[0];
                break;
        }
    }

    public bool select(Vector3 pos, Item item, int rot)
    {
        if (item == null) {
            undo();
            return false;
        } else if (item.size == Vector2.zero) {
            undo();
            return false;
        }

        if (_hints.activeInHierarchy)
            undo();

        bool retorno = true;
        int index = 0, state = 0;
        Tile aux;

        /*  
         *  STATE
         * 0 - branco
         * 1 - verde
         * 2 - vermelho
         * 
         */

        Vector3 temp = Vector3.zero;
        for (int i = 0; i < item.size.x; i++)
        {
            for (int j = 0; j < item.size.y; j++)
            {
                switch (rot)
                {
                    case 0: //BAIXO
                        temp = pos - new Vector3(i, 0, j);
                        break;
                    case 1: // ESQUERDA
                        temp = pos - new Vector3(j, 0, i);
                        break;
                    case 2: // CIMA
                        temp = pos + new Vector3(i, 0, j);
                        break;
                    case 3: // DIREITA
                        temp = pos + new Vector3(j, 0, i);
                        break;
                }

                aux = get(temp + offset);

                if (aux == null) {
                    state = 2;
                    retorno = false;
                } else {
                    switch (item.selec)
                    {
                        case Item.selecao.ARADO:
                            if (aux.obj != null | aux.parent != null) {
                                state = 2;
                                retorno = false;
                            } else if (aux.arado) {
                                state = 1;
                            } else {
                                retorno = false;
                                state = 0;
                            }
                            break;
                        case Item.selecao.VAZIO:
                            if (aux.obj != null | aux.parent != null) {
                                state = 2;
                                retorno = false;
                            } else if (aux.arado) {
                                state = 0;
                                retorno = false;
                            } else {
                                state = 1;
                            }
                            break;
                        case Item.selecao.PLANTADO:
                            if (aux.obj != null | aux.parent != null) {
                                state = 1;
                            } else {
                                state = 2;
                                retorno = false;
                            }
                            break;
                    }
                }

                hint[state, index].transform.position = temp;
                hint[state, index].SetActive(true);

                index++;
            }
        }

        _hints.SetActive(true);
        return retorno;
    }

    public void undo()
    {
        _hints.SetActive(false);
        for (int i = 0; i < prefs.Length; i++)
        {
            for (int j = 0; j < hint.GetLength(1); j++)
                hint[i, j].SetActive(false);
        }
    }

    void hints()
    {
        int slots = 9;
        Debug.LogWarning("positions tem " +slots+ " slots");
        /*grn = new GameObject[slots];
        wht = new GameObject[slots];
        red = new GameObject[slots];*/


        _hints = new GameObject("hints");
        hint = new GameObject[3, slots];
        /*for (int i = 0; i < slots; i++)
        {
            wht[i] = Instantiate(prefs[0], _hints.transform);
            wht[i].SetActive(false);
            grn[i] = Instantiate(prefs[1], _hints.transform);
            grn[i].SetActive(false);
            red[i] = Instantiate(prefs[2], _hints.transform);
            red[i].SetActive(false);
            _hints.SetActive(false);
        }*/

        for (int i = 0; i < prefs.Length; i++)
        {
            for (int j = 0; j < slots; j++)
            {
                hint[i,j] = Instantiate(prefs[i], _hints.transform);
                hint[i, j].SetActive(false);
            }
        }
        _hints.SetActive(false);
    }
}
