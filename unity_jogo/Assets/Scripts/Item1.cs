using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    item item;

    public int sound;
    public Vector2Int size;
    public abstract bool usar(Vector3 pos, int rot);

    public enum selecao {ARADO, VAZIO, PLANTADO};
    public selecao selec;

}

class Semente : Item
{
    Tree semente;
    public Semente(Tree semente)
    {
        this.semente = semente;
        selec = selecao.ARADO;
        size = semente.size;
        sound = 1;
    }


    public override bool usar(Vector3 pos, int rot)
    {
        return Board.board.plant(semente, pos, rot);
    }

}

class Enxada : Item
{
    public Enxada()
    {
        selec = selecao.VAZIO;
        size = Vector2Int.one;
        sound = 0;
    }

    public override bool usar(Vector3 pos, int rot)
    {
        Tile tile = Board.board.get((int)Mathf.Abs(pos.x), (int)Mathf.Abs(pos.z));
        if (tile == null)
            return false;

        if (tile.obj != null | tile.parent != null)
            return false;

        Sound_player.player.play(0);
        tile.Arar();

        return true;
    } 

}

class Regador : Item
{
    public Regador() {
        selec = selecao.PLANTADO;
        size = Vector2Int.one;
        sound = 3;
    }

    public override bool usar(Vector3 pos, int rot)
    {
        Tile tile = Board.board.get(pos);

        if (tile == null)
            return false;

        return tile.Regar();
    }
}

class Tesoura : Item
{
    public Tesoura()
    {
        selec = selecao.PLANTADO;
        size = Vector2Int.one;
        sound = 4;
    }

    public override bool usar(Vector3 pos, int rot)
    {
        Tile tile = Board.board.get(pos);

        if (tile == null)
            return false;

        if (tile.parent != null)
            tile = tile.parent;

        if (tile.madura == 0)
            return false;

        if (tile.tree.poldada_ == null)
            return false;

        if (tile.Podar())
        {
            Debug.Log("Inventory.inventory - adicionar composto");
            return true;
        }

        return false;
    }

}