
using UnityEngine.SceneManagement;
using UnityEngine;


public class Persistencia : MonoBehaviour
{

    [SerializeField]
    Tree [] trees;

    [SerializeField]
    Dados save_carregado;

    public bool carregar;
    private void Start()
    {
        if (carregar)
            Carregar();
    }


    public void Reiniciar() {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(1);
    }

    public void Salvar()
    {
        print("salvar");
        int _x = 20, _y = 20;
        Dados dados = new Dados(_x, _y);


        ////////////////////////////    ITENS    /////////////////////////////////
        dados.itens_1 = "";
        for (int i = 0; i < 4; i++)
            dados.itens_1 += Inventory.inventory.bolso[1, i] + ",";

        dados.itens_2 = "";
        for (int i = 0; i < 5; i++)
            dados.itens_2 += Inventory.inventory.bolso[2, i] + ",";

        dados.itens_3 = "";
        for (int i = 0; i < 4; i++)
            dados.itens_3 += Inventory.inventory.bolso[3, i] + ",";


        ///////////////////////////    STATUS    /////////////////////////////////
        dados.saldo = Inventory.inventory.saldo;
        dados.hora = Tempo.tempo.hora;



        ///////////////////////////    SOLO    /////////////////////////////////
        for (int y = 0; y < dados.y; y++)
        {
            string solo_lvl = "";
            for (int x = 0; x < dados.x; x++)
            {
                Tile tile = Board.board.get(x, y);
                if (tile == null)
                    continue;

                solo_lvl += tile.current_level.ToString();
            }
            dados.solo_nvl += solo_lvl;
        }

        //////////////////////////    PLANTAS    /////////////////////////////////
        if(Tempo.tempo.plantas.Count > 0)
        {
            for (int i = 0; i < Tempo.tempo.plantas.Count; i++)
            {
                Tile planta = Tempo.tempo.plantas[i];
                //                |          id          |          pos          |          rot          |        current_age       |        target_age       |        current_state       |        colheitas       |_  
                dados.plantas +=       planta.tree.id + "|" + planta.position + "|" + planta.rotation + "|" + planta.current_age + "|" + planta.target_age + "|" + planta.current_state + "|" + planta.colheitas + "|_";
            }
        }   



        string json = JsonUtility.ToJson(dados);
        PlayerPrefs.SetString("level_1", json);
    }

    public void Carregar()
    {
        print("carregar");
        string json = PlayerPrefs.GetString("level_1");
        if (json.Length == 0)
            return;

        Dados dados = JsonUtility.FromJson<Dados>(json);
        save_carregado = dados;

        string[] itens;
        ////////////////////////////    ITENS    /////////////////////////////////

        itens = dados.itens_1.Split(',');
        for (int i = 0; i < 3; i++)
            Inventory.inventory.bolso[1, i] = int.Parse(itens[i]);

        itens = dados.itens_2.Split(',');
        for (int i = 0; i < 4; i++)
            Inventory.inventory.bolso[2, i] = int.Parse(itens[i]);

        itens = dados.itens_3.Split(',');
        for (int i = 0; i < 4; i++)
            Inventory.inventory.bolso[3, i] = int.Parse(itens[i]);


        ////////////////////////////    STATUS    /////////////////////////////////
        Inventory.inventory.saldo = dados.saldo;
        Tempo.tempo.SetTo(dados.hora);
        HUD.hud.att_saldo();

        int index = 0;
        ////////////////////////////    SOLO    /////////////////////////////////
        for (int y = 0; y < dados.y; y++)
        {
            for (int x = 0; x < dados.x; x++) {
                Tile tile = Board.board.get(x, y);

                if (tile == null)
                    continue;

                tile.current_level = (dados.solo_nvl[index] - '0');
                Board.board.change_mat(tile.position, tile.current_level, false);

                index++;
            }
        }

        //////////////////////////    PLANTAS    /////////////////////////////////
        if (dados.plantas.Length > 0)
        {
            string[] plantas = dados.plantas.Split('_');

            for (int i = 0; i < plantas.Length; i++) {

                string[] planta = plantas[i].Split('|');
                if (planta.Length == 0)
                    continue;

                if (planta[0].Length == 0)
                    continue;

                int idx = int.Parse(planta[0]);

                Tree tree = trees[ idx ];
                Vector2 pos = stringToVector2(planta[1]);
                int rot = int.Parse(planta[2]);
                int c_age = int.Parse(planta[3]);
                int t_age = int.Parse(planta[4]);
                int c_stt = int.Parse(planta[5]);
                int clht = int.Parse(planta[6]);

                Board.board.plant_at(tree, pos, rot, c_age, t_age, c_stt, clht);
            }
        }



        Vector2 stringToVector2(string rString)
        {
            string[] temp = rString.Substring(1, rString.Length - 2).Split(',');

            float x = System.Convert.ToSingle(temp[0]);
            float y = System.Convert.ToSingle(temp[1]);

            Vector2 value = new Vector2(x, y);
            return value;
        }

    }


}

[System.Serializable]
public class Dados {

    public int pos_x, pos_y, x, y;
    public int saldo;
    public int hora;

    public string plantas;
    public string solo_nvl;
    public string itens_1;
    public string itens_2;
    public string itens_3;

    public Dados(int x, int y) {
        saldo = 10;
        hora = 7;
        plantas = "";
        solo_nvl = "";
        itens_1 = "0,0,0,0";
        itens_2 = "0,0,0,0,0";
        itens_3 = "0,0,0,0";

        this.x = x;
        this.y = y;
    }

}





