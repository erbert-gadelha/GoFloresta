using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Inventory : MonoBehaviour
{
    public enum tipo { FERRAMENTA, SEMENTE }
    public static Inventory inventory;

    [SerializeField]
    public item[] itens_0 = new item[0], itens_1 = new item[0], itens_2 = new item[0], itens_3 = new item[0];

    Transform hand;
    public int saldo;

    public int[,] bolso;
    int tool;

    Item[] _item;

    void Awake()
    {
        _item = new Item[2];

        bolso = new int[4, 5];

        on_use = -1;
        tool = -1;
        inventory = this;
        hand = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);

        for (int i = 0; i < hand.childCount; i++)
            hand.GetChild(i).gameObject.SetActive(false);
    }


    [SerializeField]
    GameObject wasd;
    public void change_tool(Item tool, int id) {
        if (this.tool >= 0)
             hand.GetChild(this.tool).gameObject.SetActive(false);

        this.tool = id;

        if (this.tool >= 0)
            hand.GetChild(this.tool).gameObject.SetActive(true);

        hand.gameObject.SetActive(true);

        on_use = 0;

        _item[0] = tool;
        Moviment_2.player.equipar(_item[0]);
    }

    public void change_seed(Tree seed) {
        on_use = 1;

        hand.gameObject.SetActive(false);

        _item[1] = new Semente(seed);
        Moviment_2.player.equipar(_item[1]);
    }

    [SerializeField]
    int on_use;

    public void _tool() {
        if (_item[0] == null)
            return;

        hand.gameObject.SetActive(true);

        if (on_use == 0) { 
            Moviment_2.player._use();
        } else {
            Moviment_2.player.equipar(_item[0]);
            Moviment_2.player._use();
            on_use = 0;
        }
    }

    public void _seed() {
        if (_item[1] == null)
            return;

        if (bolso[HUD.hud.opened_item.x, HUD.hud.opened_item.y] > 0)
            bolso[HUD.hud.opened_item.x, HUD.hud.opened_item.y]--;
        else
            return;

        hand.gameObject.SetActive(false);

        if (on_use == 1) {
            Moviment_2.player._use();
        } else
        {
            Moviment_2.player.equipar(_item[1]);
            Moviment_2.player._use();
            on_use = 1;
        }
    }

}
