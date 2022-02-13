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
    //public bool arado;
    public bool molhado;

    [HideInInspector]
    public MeshRenderer mesh;

    public int current_state;
    public Vector2Int position;

    public int current_age;
    //[HideInInspector]
    public GameObject obj;
    //[HideInInspector]
    public Tile parent_;
    [HideInInspector]
    public Vector3 center;

    public int colheitas;
    public int current_level;
    public int target_age;

    //[HideInInspector]
    public Tile[] childs;


    public bool na_sombra = false;


    private void Awake()
    {
        mesh = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
    }

    void Start()
    {
        
        arado = null;
        current_age = 0;
        current_level = 0;
        position = new Vector2Int((int)Mathf.Abs(transform.localPosition.x), (int)Mathf.Abs(transform.localPosition.z));
        Board.board.add(this, position.x, position.y);

        //Debug.LogWarning("Temporario");
        //arado = true;
        //current_level = 1;
        //Board.board.change_mat(position, 3, na_sombra);
        Board.board.change_mat(position, 0, false);
    }

    public bool com_fruto = false;

    //  0-crescendo | 1-dando_fruto  | 2-poldada | 3-parada
    public int crescendo;

    public void grow(bool dia) {
        if (!tree.gosta_sombra)
            if (na_sombra | !dia)
                return;

        if (tree == null) {
            Tempo.tempo.Remove(this);
            current_state = -1;
            current_age = -1;

            Board.board.destroy(position);
            return;
        }

        if (current_state < 0)
            return;

        int temp;

        //  0-crescendo | 1-dando_fruto  | 2-poldada | 3-parada
        switch (crescendo)
        {
            case 0: //CRESCENDO
                temp = current_state;
                current_age++;

                if (current_age == 0)
                    current_state = 0;
                else
                    current_state = (int)(2 * ((current_age * 1f) / (target_age * 1f)));

                if (temp != current_state)
                    Refresh();
                break;

            case 1: //REPONDO FRUTO
                current_age++;

                if (current_age >= target_age)
                {
                    crescendo = 3;
                    current_state = 2;
                    Refresh();
                    print("repor fruto");
                }

                break;
            case 2: //REPONDO FOLHAS
                current_age++;

                if (current_age >= target_age)
                {
                    crescendo = 3;
                    current_state = 3;
                    Refresh();
                    print("repor folhas");
                }

                break;
        }

    }



    public void Refresh()
    {
        if (current_state < 0)
            return;

        if (tree != null)
        {
            GameObject aux = null;
            bool colisao = false;

            switch (current_state)
            {
                case 0:
                    aux = tree._broto;
                    break;
                case 1:
                    aux = tree._0_jovem;
                    colisao = true;
                    break;
                case 2:
                    aux = tree._1_adulta;
                    colisao = true;
                    current_state = 2;

                    com_fruto = true;
                    Tempo.tempo.Remove(this);

                    current_level = tree.level + 1;
                    if (current_level >= 2) current_level = 2;
                    Board.board.change_mat(position, current_level, na_sombra);

                    for (int i = 0; i < childs.Length; i++) {
                        Tile child = childs[i];

                        child.current_level = current_level;
                        Board.board.change_mat(child.position, current_level, child.na_sombra);
                    }

                    break;
                case 3:
                    aux = tree._2_colhida;

                    colisao = true;
                    com_fruto = false;

                    colheitas++;
                    if (colheitas >= tree.colheitas) {
                        Debug.Log("nomore colheiras");
                        Tempo.tempo.Remove(this);
                        crescendo = 3;
                    } else {
                        Tempo.tempo.Remove(this);
                        Tempo.tempo.Add(this);
                        target_age = tree.tempo_fruto;
                        current_age = 0;
                        crescendo = 1;
                    }

                    break;
                case 4:
                    aux = tree._3_poldada;
                    colisao = true;

                    //colheitas++;
                    if (colheitas >= tree.colheitas) {
                        Debug.Log("nomore colheiras");
                        Tempo.tempo.Remove(this);
                        crescendo = 2;
                    }
                    else
                    {
                        Tempo.tempo.Remove(this);
                        Tempo.tempo.Add(this);
                        target_age = tree.tempo_fruto;
                        current_age = 0;
                        crescendo = 1;
                    }

                    break;
            }

            if (aux == null)
                return;



            colision = colisao;
            plantable = false;
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].colision = colisao;
                childs[i].plantable = false;
            }

            if (aux != null)
            {
                if (obj != null)
                    Destroy(obj);
                obj = Instantiate(aux, center, Quaternion.Euler(0, 90 * rotation, 0), transform);
            }

        }

    }

    public GameObject arado;

    public void Arar(bool arar) {

        if (arado != null)
            Destroy(arado);

        if (arar)
            arado = Instantiate(Board.board.arado, transform);

    }


    public GameObject criar(GameObject param) {
        return Instantiate(param, center, Quaternion.Euler(0, 90 * rotation, 0), transform);
    }

    public void apagar() {
        if (obj != null)
            Destroy(obj);
    }

}
