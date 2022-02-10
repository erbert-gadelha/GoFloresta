using UnityEngine;

[CreateAssetMenu(fileName = "planta_", menuName = "Plantas/Criar planta", order = 1)]

public class Tree : ScriptableObject
{
    [Header("Status")]
    public string name;
    public Sprite icon;
    public int age;
    public int level;
    public bool gosta_sombra;
    public Vector2Int size;
    public int tempo_crescer;
    public int tempo_fruto;
    public int colheitas;

    [Header("Stages")]
    public GameObject _broto;
    public GameObject _0_jovem;
    public GameObject _1_adulta;
    public GameObject _2_colhida;
    public GameObject _3_poldada;

}
