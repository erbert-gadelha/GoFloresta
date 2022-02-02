using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board board;

    public enum materials { arado, grama, molhado, ocupado };

    [SerializeField]
    public enum estados { grama, arado_seco, arado_molhado }

    [HideInInspector]
    public Tile[,] terrains;

    [SerializeField]
    GameObject [] prefs;

    [SerializeField]
    Vector3Int offset;

    [SerializeField]
    Material[] material;

    GameObject[] ocupado;
    GameObject[] livre;

    GameObject _hints;







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
        Tile aux = Board.board.get(pos);
        if (!aux.arado | aux.obj != null)
            return false;


        aux.childs = new Tile[(tree.size.x * tree.size.y)];
        int i = 0;

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
                change_mat(aux.childs[i].position, materials.ocupado);
                aux.childs[i].parent = aux;
                i++;
            }
        }

        aux.parent = null;
        change_mat(aux.position, materials.arado);

        aux.tree = tree;
        aux.curnt_age = -1;
        aux.currt_state = 1;
        aux.rotation = rotation;

        Tempo.tempo.Add(aux);
        aux.Refresh();

        return true;
    }

    public void change_mat(Vector2Int pos, materials material)
    {
        Tile aux = get(pos.x, pos.y);

        switch (material)
        {
            case materials.arado:
                aux.arado = true;
                aux.mesh.sharedMaterial = this.material[0];
                break;
            case materials.grama:
                aux.arado = false;
                aux.mesh.sharedMaterial = this.material[1];
                break;
            case materials.molhado:
                aux.arado = true;
                aux.mesh.sharedMaterial = this.material[2];
                break;
            case materials.ocupado:
                aux.arado = true;
                aux.mesh.sharedMaterial = this.material[3];
                break;
        }
    }

    public bool select(Vector3 pos, Vector2 size, int rot)
    {
        if (size == Vector2.zero)
            return false;

        if (_hints.activeInHierarchy)
            undo();

        bool empty, retrn = true;
        int index = 0;
        Tile aux;


        Vector3 temp = Vector3.zero;
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
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

                if (aux == null)
                    empty = false;
                else if(aux.obj != null | aux.parent != null)
                    empty = false;
                else
                    empty = true;

                if (empty)
                {
                    livre[index].transform.position = temp;
                    livre[index].SetActive(true);
                }
                else
                {
                    ocupado[index].transform.position = temp;
                    ocupado[index].SetActive(true);

                    retrn = false;
                }

                index++;
            }
        }

        _hints.SetActive(true);
        return retrn;
    }

    public void undo()
    {
        _hints.SetActive(false);
        for (int i = 0; i < ocupado.Length; i++)
        {
            ocupado[i].SetActive(false);
            livre[i].SetActive(false);
        }
    }

    void hints()
    {
        int slots = 9;
        Debug.LogWarning("positions tem " +slots+ " slots");
        ocupado = new GameObject[slots];
        livre = new GameObject[slots];

        _hints = new GameObject("hints");
        for (int i = 0; i < slots; i++)
        {
            livre[i] = Instantiate(prefs[0], _hints.transform);
            livre[i].SetActive(false);
            ocupado[i] = Instantiate(prefs[1], _hints.transform);
            ocupado[i].SetActive(false);
            _hints.SetActive(false);
        }
    }
}
