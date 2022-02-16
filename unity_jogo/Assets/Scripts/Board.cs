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
    Material[] material;
    [SerializeField]
    Material[] material_sombra;

    GameObject _hints;
    GameObject [,] hint;


    public GameObject broto;
    public GameObject arado;



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

        int size = tree.size.x * tree.size.y;
        if (size > 1) {
            aux.childs = new Tile[4];
            switch (rotation)
            {
                case 0: //BAIXO         aux.x - X, aux.y - Y
                    pos += new Vector3(-0.5f, 0, -0.5f);
                    break;
                case 1: // ESQUERDA     aux.x - Y, aux.y - X
                    pos += new Vector3(-0.5f, 0, -0.5f);
                    break;
                case 2: // CIMA         aux.x + X, aux.y + Y
                    pos += new Vector3(0.5f, 0, 0.5f);
                    break;
                case 3: // DIREITA      aux.x + Y, aux.y + X
                    pos += new Vector3(0.5f, 0, 0.5f);
                    break;
            }

            aux.childs[0] = get((int)(pos.x + 0.5f), (int)(pos.z + 0.5f));
            aux.childs[1] = get((int)(pos.x - 0.5f), (int)(pos.z + 0.5f));
            aux.childs[2] = get((int)(pos.x - 0.5f), (int)(pos.z - 0.5f));
            aux.childs[3] = get((int)(pos.x + 0.5f), (int)(pos.z - 0.5f));

            for (int i = 0; i < aux.childs.Length; i++)
                if (aux.childs[i] == null)
                {
                    aux.childs = null;
                    return false;
                }

        } else {
            aux.childs = new Tile[0];
        }


        for (int i = 0; i < aux.childs.Length; i++)
        {
            if (aux.childs[i] == null)
                continue;


            //change_mat(aux.childs[i].position, tree.level, aux.childs[i].na_sombra);
            aux.childs[i].Arar(false);
            aux.childs[i].parent_ = aux;
        }

        //change_mat(aux.position, tree.level, aux.na_sombra);
        aux.Arar(false);
        aux.parent_ = null;
        aux.center = pos;

        aux.tree = tree;
        aux.current_age = -1;
        aux.target_age = tree.tempo_crescer;
        aux.current_state = 0;
        aux.rotation = rotation;

        aux.colheitas = 0;
        Tempo.tempo.Add(aux);
        aux.Refresh();

        return true;
    }

    public bool destroy(Vector2Int pos)
    {
        Tile tile = get(pos.x, pos.y);

        if (tile.obj == null)
            return false;

        Tempo.tempo.Remove(tile);
        Destroy(tile.obj);

        if (tile.childs != null)
        { 
            if(tile.childs.Length > 0)
            for (int i = 0; i < tile.childs.Length; i++) {
                Tile aux = tile.childs[i];
                if (aux == tile)
                    continue;

                Zerar(aux);
            }
        }

        Zerar(tile);
        return true;
    }
    public void Zerar(Tile tile) {
        if (tile == null)
            return;

        change_mat(tile.position, tile.current_level, tile.na_sombra);

        tile.obj = null;
        tile.tree = null;
        tile.childs = null;
        tile.parent_ = null;

        tile.com_fruto = false;
        tile.current_state = 0;
        tile.current_age = 0;
        tile.crescendo = 0;
        tile.plantable = true;
        tile.colision = false;
        tile.Arar(false);
        tile.nivel_agua = 0;
    }


    public void colher(Vector3 position)
    {
        Tile tile = get(position);
        if (tile == null)
            return;

        if (tile.parent_ != null)
            tile = tile.parent_;

        Sound_player.player.play(6);
        Inventory.inventory.saldo += tile.tree.ganho_colheita;
        tile.current_state = 3;
        tile.Refresh();
    }

    public void change_mat(Vector2Int pos, int index, bool sombra)
    {
        Tile aux = get(pos.x, pos.y);

        if (!sombra)
            aux.mesh.sharedMaterial = material[index];
        else
            aux.mesh.sharedMaterial = material_sombra[index];
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
                    switch ((int) item.selec)//{ARAR, CORTAR, PLANTAR, PODAR, REGAR};
                    {
                        // ARAR
                        case 0:    // [-n.ARADO -VAZIO]
                            if (aux.obj != null | aux.parent_ != null | !aux.plantable) {
                                state = 2;
                                retorno = false;
                            } else if (aux.arado) {
                                state = 0;
                                retorno = false;
                            } else {
                                state = 1;
                            }
                            break;

                        // CORTAR
                        case 1:    // [-PLANTADO]
                            if (aux.obj != null | aux.parent_ != null) {
                                state = 1;
                            } else {
                                state = 2;
                                retorno = false;
                            }
                            break;

                        // PLANTAR
                        case 2:    // [-ARADO -VAZIO]
                            if (aux.obj != null | aux.parent_ != null) {//PLANTADO
                                state = 2;
                                retorno = false;
                            } else if (aux.arado != null) {//ARADO
                                if (aux.current_level < item.id)    //NIVEL ERRADO
                                {
                                    retorno = false;
                                    state = 0;
                                }
                                else                //NIVEL CERTO
                                    state = 1;
                            } else {//NAO ARADO
                                retorno = false;
                                state = 2;
                            }
                            break;

                        // PODAR
                        case 3:    // [-PLANTADO]
                            if (aux.obj != null | aux.parent_ != null) {

                                if (aux.parent_ != null)
                                    aux = aux.parent_;

                                if (aux.crescendo == 3)
                                    state = 1;
                                else {
                                    state = 2;
                                    retorno = false;
                                }
                            } else {
                                state = 2;
                                retorno = false;
                            }
                            break;

                        // REGAR
                        case 4:    // [-PLANTADO]
                            if (aux.obj != null | aux.parent_ != null) {
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
        int slots = 4;

        _hints = new GameObject("hints");
        hint = new GameObject[3, slots];

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


    public void plant_at(Tree tree, Vector2 pos_, int rot, int c_age, int t_age, int c_stt, int colheitas)
    {
        Tile[] childs;
        Tile aux;

        int size = tree.size.x * tree.size.y;
        if (size > 1)
        {
            childs = new Tile[4];

            childs[0] = get((int)(pos_.x + 0.5f), (int)(pos_.y + 0.5f));
            childs[1] = get((int)(pos_.x - 0.5f), (int)(pos_.y + 0.5f));
            childs[2] = get((int)(pos_.x - 0.5f), (int)(pos_.y - 0.5f));
            childs[3] = get((int)(pos_.x + 0.5f), (int)(pos_.y - 0.5f));

            childs[1].parent_ = childs[0];
            childs[2].parent_ = childs[0];
            childs[3].parent_ = childs[0];
            aux = childs[0];
            aux.center = new Vector3(pos_.x, 0, pos_.y);
        }
        else
        {
            childs = new Tile[0];
            aux = get((int)pos_.x, (int)pos_.y);
            aux.center = new Vector3(pos_.x, 0, pos_.y);
        }

        aux.parent_ = null;
        aux.childs = childs;
        aux.rotation = rot;
        aux.current_age = c_age;
        aux.target_age = t_age;
        aux.current_state = c_stt;
        aux.colheitas = colheitas;
        aux.tree = tree;

        aux.Refresh();
        Tempo.tempo.Add(aux);
    }

    public void castShadows(Vector3 direc)
    {

        /*
        print(direc);
        Debug.Log("Ta no cast");
        */
        Tile aux;
        for (int i = 0; i < terrains.GetLength(0); i++)
            for (int j = 0; j < terrains.GetLength(1); j++)
            {
                aux = get(i, j);
                if (aux == null)
                    continue;

                bool na_sombra = Physics.Raycast(new Vector3(i, 0, j), direc);
                if (na_sombra != aux.na_sombra)
                {
                    aux.na_sombra = na_sombra;
                    change_mat(aux.position, aux.current_level, aux.na_sombra);

                    Debug.Log("mandou mudar mat");
                }

                /*
                if (na_sombra)
                    Debug.Log("sombra");
                else
                    Debug.Log("sol");*/
            }

    }

}
