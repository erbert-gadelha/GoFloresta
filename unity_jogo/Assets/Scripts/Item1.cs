using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public int id;
    public int sound;
    public Vector2Int size;
    public abstract bool usar(Vector3 pos, int rot);

    public enum selecao {ARAR, CORTAR, PLANTAR, PODAR, REGAR };
    public selecao selec;

}

class Semente : Item
{
    Tree semente;
    public Semente(Tree semente)
    {
        this.semente = semente;
        selec = selecao.PLANTAR;
        size = semente.size;
        sound = 1;
        id = semente.level;
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
        selec = selecao.ARAR;
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

        if (tile.arado)
            return false;

        Debug.LogWarning("Habilitar/desabilitar \"arado\"");
        Board.board.change_mat(tile.position, 3, tile.na_sombra);
        tile.arado = true;

        return true;
    } 

}

class Regador : Item
{
    public Regador() {
        selec = selecao.REGAR;
        size = Vector2Int.one;
        sound = 3;
    }

    public override bool usar(Vector3 pos, int rot)
    {
        Tile tile = Board.board.get(pos);

        if (tile == null)
            return false;


        if (!tile.arado)
            return false;

        //Board.board.change_mat(position, Board.materials.molhado);
        Debug.LogWarning("mudar material e colocar tempo pra secar");

        tile.molhado = true;
        return true;
    }
}

class Tesoura : Item
{
    public Tesoura()
    {
        selec = selecao.PODAR;
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

        if (tile.tree._3_poldada == null)
            return false;

        if (tile.crescendo != 3)
            return false;



        tile.target_age = tile.tree.tempo_fruto;
        tile.current_age = 0;
        tile.current_state = 3;
        tile.Refresh();


        Debug.LogWarning("Ganhar folhas");

        Tempo.tempo.Remove(tile);
        Tempo.tempo.Add(tile);
        return true;
    }

}

class Machado : Item
{
    public Machado()
    {
        selec = selecao.CORTAR;
        size = Vector2Int.one;
        sound = 5;
    }

    public override bool usar(Vector3 pos, int rot)
    {
        Tile tile = Board.board.get(pos);

        if (tile == null)
            return false;

        if (tile.parent != null)
            tile = tile.parent;

        if (Board.board.destroy(tile.position)) {
            Debug.LogWarning("Habilitar/desabilitar \"arado\"");
            Debug.LogWarning("Dar pontos");

            return true;
        }
        
        return false;
    }

}