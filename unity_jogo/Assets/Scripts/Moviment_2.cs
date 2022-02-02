using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moviment_2 : MonoBehaviour
{
    public static Moviment_2 player;
    [HideInInspector]
    public Item item;
    [HideInInspector]
    public Tree seed;




    enum control { touch, teclado }
    [SerializeField]
    control controle = control.touch;


    bool able;
    HUD hud;
    Board board;

    [SerializeField]
    Vector3 initial, offset;
    Vector3[] axis;
    Vector2 input, last_inpt = Vector2.zero;
    Vector3 position = new Vector3();

    [SerializeField]
    Animator anim;

    [SerializeField]
    float vel, rot_vel;
    [SerializeField]
    int cam_rotate, rot;

    int _rot = 0;

    [SerializeField]
    Transform target, boneco, cam;

    private int rotation = 0;
    int fps;


    bool pode_plantar;


    private void Awake()
    {
        axis = new Vector3[2];
        cam = transform.GetChild(1);
        player = this;
        StartCoroutine(Cam_rotate(0));
    }

    void Start()
    {
        board = Board.board;
        hud = HUD.hud;

        if (Application.platform == RuntimePlatform.Android)
                controle = control.touch;
        position = initial+Vector3.up* transform.position.y;

        input = new Vector2();
        boneco = transform.GetChild(0);
        transform.position = position;
        able = true;
    }

    void Update()
    {
        fps = (int)(1 / Time.deltaTime);

        if (controle == control.teclado)
        {
            if (Input.GetAxisRaw("Horizontal") != 0) {
                input.x = ((Input.GetAxisRaw("Horizontal") > 0 ? 1 : -1));
                input.y = 0;
            } else if (Input.GetAxisRaw("Vertical") != 0) {
                input.y = ((Input.GetAxisRaw("Vertical") > 0 ? 1 : -1));
                input.x = 0;
            } else {
                input.x = 0;
                input.y = 0;
            }

            if (Input.GetKeyDown(KeyCode.Q))
                _cam(1);
            if (Input.GetKeyDown(KeyCode.E))
                _cam(-1);

            if (Input.GetKeyDown(KeyCode.Space))
                _roe();
        }
    }

    void FixedUpdate()
    {
        if (able)
        {
            if (input.x != 0 | input.y != 0)
            {
                able = false;
                StartCoroutine(Move());
            }
        }
    }

    IEnumerator Move()
    {
        able = false;
        
        Vector3 aux = (input.x * axis[0] + input.y * axis[1]);
        target.position = position + aux;

        if (rotate(input))
        {
            hud.change_to(target.position + offset, true);
            pode_plantar = board.select(target.position, size, (int)((boneco.localEulerAngles.y / 90)));
            yield return new WaitForSeconds(0.15f);
        }
        else
        {
            hud.change_to(target.position + offset, true);
            pode_plantar = board.select(target.position, size, (int)((boneco.localEulerAngles.y / 90)));
        }


        while (input != Vector2.zero)
        {
            rotate(input);

            aux = (input.x * axis[0] + input.y * axis[1]);
            Tile tile = board.get((int)(position.x + offset.x + aux.x), (int)(position.z + aux.z + offset.z));

            if (tile == null)
            {
                anim.SetBool("walk", false);
                target.position = position + aux;

                hud.status_visibility(false);
                board.undo();

                yield return new WaitForSeconds(0.15f);
                continue;
            }
            else if (tile.colision)
            {
                anim.SetBool("walk", false);
                target.position = position + aux;

                hud.change_to(target.position + offset, true);
                pode_plantar = board.select(target.position, size, rotation);

                yield return new WaitForSeconds(0.15f);
                continue;
            }
            board.undo();


            position += aux;
            target.position = position + aux;
            anim.SetBool("walk", true);

            hud.change_to(target.position, true);
            pode_plantar = board.select(target.position, size, rotation);

            while (transform.position != position)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, vel);
                yield return new WaitForFixedUpdate();
            }
            

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForFixedUpdate();
        pode_plantar = board.select(target.position, size, rotation);
        anim.SetBool("walk", false);
        able = true;
    }

    IEnumerator Cam_rotate(int dir)
    {
        able = false;

        cam_rotate += dir;
        if (cam_rotate < 0)
            cam_rotate = 3;
        if (cam_rotate > 3)
            cam_rotate = 0;


        switch (cam_rotate)
        {
            case 0:
                axis[0] = new Vector3(0, 0, -1);
                axis[1] = new Vector3(1, 0, 0);
                break;
            case 1:
                axis[0] = new Vector3(-1, 0, 0);
                axis[1] = new Vector3(0, 0, -1);
                break;
            case 2:
                axis[0] = new Vector3(0, 0, 1);
                axis[1] = new Vector3(-1, 0, 0);
                break;
            case 3:
                axis[0] = new Vector3(1, 0, 0);
                axis[1] = new Vector3(0, 0, 1);
                break;
        }

        rot = (90* cam_rotate);
        while (cam.localEulerAngles.y != rot)
        {
            cam.localEulerAngles = Vector3.up * Mathf.MoveTowardsAngle(cam.localEulerAngles.y, rot, rot_vel);
            yield return new WaitForFixedUpdate();
        }
        last_inpt = Vector2.zero;
        able = true;
    }

    bool rotate(Vector2 _inpt)
    {
        if (last_inpt != _inpt)
        {
            last_inpt = _inpt;
            Vector2 x2 = new Vector2(axis[0].x, axis[0].z), y2 = new Vector2(axis[1].x, axis[1].z);

            switch (((_inpt.x * x2) + _inpt.y * y2))
            {
                case Vector2 v when v.Equals(Vector2.up):
                    boneco.eulerAngles = new Vector2(0, 180);
                    rotation = 2;
                    break;
                case Vector2 v when v.Equals(Vector2.down):
                    boneco.eulerAngles = new Vector2(0, 0);
                    rotation = 0;
                    break;
                case Vector2 v when v.Equals(Vector2.left):
                    boneco.eulerAngles = new Vector2(0, 90);
                    rotation = 1;
                    break;
                default:
                    boneco.eulerAngles = new Vector2(0, 270);
                    rotation = 3;
                    break;
            }
            _rot = (int)boneco.localEulerAngles.y;
            return true;
        }
        return false;
    }
    

    public void _move(Vector2 direc)
    {
        input = direc;
    }
    public void _stop()
    {
        input = Vector2.zero;
    }
    public void _cam(int dir)
    {
        cam = transform.GetChild(1);
        if (!able)
            return;
        able = false;

        StartCoroutine(Cam_rotate(dir));
    }
    private void _roe()
    {
        Vector3 aux_ = target.position + offset;
        Tile aux = board.get( (int)Mathf.Abs(aux_.x), (int)Mathf.Abs(aux_.z));
        if (aux == null)
            return;

        if (aux.obj != null)
            return;

        aux.Arar();
    }
    private void _seed()
    {
        if (!pode_plantar)
            return;

        Vector3 aux_ = target.position + offset;
        board.plant(seed, aux_, rotation);

        pode_plantar = board.select(target.position, size, rotation);
        hud.change_to(target.position + offset, true);
    }
    private void _splash()
    {
        Vector3 aux_ = target.position + offset;
        Tile aux = board.get(aux_);
        if (aux == null)
            return;

         aux.Regar();
    }

    public void equipar(Item item)
    {
        if (item == null)
        {
            size = Vector2.zero;
            this.item = null;
            seed = null;

            return;
        }

        switch (item.tipo)
        {
            case Inventory.tipo.FERRAMENTA:
                size = Vector2.one;
                this.item = item;
                seed = null;
                break;
            case Inventory.tipo.SEMENTE:
                size = item.seed.size;
                this.item = null;
                seed = item.seed;
                break;
        }

        _select();
    }

    public Vector2 size;
    public void _use()
    {
        if (seed != null)
        {
            if (!pode_plantar)
                return;
            _seed();
        }

        else if (item != null)
        {

            Debug.LogWarning("Isso e temporario");
            if (!pode_plantar)
                return;

            switch (item.id)
            {
                case 0: //ENXADA
                    _roe();
                    break;
                case 1: //REGADOR
                    _splash();
                    break;
            }
        }

    }

    public void _select()
    {
        pode_plantar = board.select(target.position, size, rotation);
    }


    private void OnGUI()
    {
        GUI.skin.label.fontSize = 30;
        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, 10, 200, 50), "fps: " + fps);
    }
}
