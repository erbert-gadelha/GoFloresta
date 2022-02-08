using UnityEngine;

[CreateAssetMenu(fileName = "planta_", menuName = "Plantas/Criar planta", order = 1)]

public class Tree : ScriptableObject
{
    [Header("Status")]
    public string name;
    public Sprite icon;
    public int age;
    public Vector2Int size;

    [Header("Stages")]
    public GameObject[] stages = { null };
    public GameObject carregada_;
    public GameObject carregada_poldada;
    public GameObject poldada_;
    public GameObject poldada_carregada;
    public bool[] colisions = { false };

    public float ph_ideal;
}
