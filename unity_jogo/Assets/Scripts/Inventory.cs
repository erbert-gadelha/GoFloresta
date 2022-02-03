using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Inventory : MonoBehaviour
{
    public enum tipo { FERRAMENTA, SEMENTE }
    public static Inventory inventory;

    [SerializeField]
    public Item[] itens_0 = new Item[0], itens_1 = new Item[0], itens_2 = new Item[0], itens_3 = new Item[0], itens_4 = new Item[0];

    Transform hand;
    int item;

    void Awake()
    {
        item = -1;
        inventory = this;
        hand = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        

        for (int i = 0; i < hand.childCount; i++)
            hand.GetChild(i).gameObject.SetActive(false);
    }


    [SerializeField]
    GameObject wasd;
    public void equipe(int param)
    {
        if (item >= 0)
            hand.GetChild(item).gameObject.SetActive(false);

        item = param;

        if (item >= 0)
            hand.GetChild(item).gameObject.SetActive(true);
    }

}
