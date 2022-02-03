using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    public enum tipos { colisao, terreno, interacao }
    [SerializeField]
    public tipos tipo;


    public int rotation;
    public Tree tree;
    public bool colision;
    public bool plantable;
    public bool arado;
    public bool molhado;

    [HideInInspector]
    public MeshFilter mesh;

    public int currt_state;
    public Vector2Int position;

    public int curnt_age;
    [HideInInspector]
    public GameObject obj;
    [HideInInspector]
    public Tile parent;
    [HideInInspector]
    public Vector3 center;





    private void Awake()
    {
        mesh = transform.GetChild(0).gameObject.GetComponent<MeshFilter>();
    }

    void Start()
    {
        arado = false;
        curnt_age = 0;
        position = new Vector2Int((int)Mathf.Abs(transform.localPosition.x), (int)Mathf.Abs(transform.localPosition.z));
        Board.board.add(this, position.x, position.y);

        Debug.LogWarning("Temporario");
        Board.board.change_mat(position, Board.materials.grama);
    }

    public void Arar()
    {
        if (!arado)
            Board.board.change_mat(position, Board.materials.arado);
        else
            Board.board.change_mat(position, Board.materials.grama);
    }

    public bool Regar()
    {
        if (!arado)
            return false;

        Board.board.change_mat(position, Board.materials.molhado);
        molhado = true;
        return true;
    }

    public bool Destruir()
    {
        if (obj == null)
            return false;

        Tempo.tempo.Remove(this);
        Destroy(obj);

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].colision = tree.colisions[currt_state - 1];
            childs[i].plantable = true;

            Board.board.change_mat(childs[i].position, Board.materials.arado);
            childs[i].parent = null;
            childs[i].curnt_age = 0;
            childs[i].colision = false;
            childs[i].obj = null;
        }

        return true;
    }

    public void grow()
    {
        if (tree != null) {
            curnt_age++;
            if (curnt_age >= currt_state * tree.age) {
                currt_state++;
                Refresh();
            }
        } else {
            Tempo.tempo.Remove(this);
            currt_state = 0;
            curnt_age = 0;

            Destroy(obj);
            obj = null;
        }
    }

    public void grow_to(int to)
    {
        if (tree != null) {
            currt_state++;
            curnt_age = currt_state * tree.age;
        } else {
            currt_state = -1;
            curnt_age = 0;
        }
        Refresh();
    }

    [HideInInspector]
    public Tile [] childs;

    public void Refresh()
    {
        if (currt_state > tree.stages.Length)
            return;

        if (obj != null)
            Destroy(obj);

        if (tree != null)
        {
            /*
            Vector3 aux = Vector3.zero;
            if (tree.size != Vector2.zero)
            switch (rotation)
            {
                case 0: aux = new Vector3(tree.si); break;
                case 1: break;
                case 2: break;
                case 3: break;
            }*/

            obj = Instantiate(tree.stages[currt_state - 1], center, Quaternion.Euler(0, 90 * rotation, 0), transform);
            //obj = Instantiate(tree.stages[currt_state - 1], transform.position, Quaternion.Euler(0, 90 * rotation, 0), transform);
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].colision = tree.colisions[currt_state - 1];
                childs[i].plantable = false;
            }
        }
    }


}
