using UnityEngine;

[CreateAssetMenu(fileName = "item_", menuName = "Itens/Criar item", order = 2)]
public class Item : ScriptableObject
{
    [Header("Status")]
    public Sprite icon;
    public string name;
    [TextArea]
    public string description;
    public int id;
    [Space(15)]
    public int qntd;
    public int value;

    public Inventory.tipo tipo;
    public Tree seed;
}

