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


    void Awake()
    {
        inventory = this;
    }



}
