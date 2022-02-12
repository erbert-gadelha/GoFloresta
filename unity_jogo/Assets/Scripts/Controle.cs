using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using static UnityEngine.UI.Image;

public class Controle : MonoBehaviour
{
    [SerializeField]
    Moviment_2 player;

    [SerializeField]
    FixedJoystick joystick;

    bool press = false;


    void FixedUpdate()
    {
        Vector2 aux = joystick.Direction;
        if (aux == Vector2.zero)
        {
            if (press)
            {
                player._stop();
                press = false;
            }
        }
        else
        {
            press = true;

            if (aux.x > 0)
                if (aux.y > 0)
                    player._move(Vector2.up);
                else
                    player._move(Vector2.right);
            else
                if (aux.y > 0)
                player._move(Vector2.left);
            else
                player._move(Vector2.down);
        }
    }
}

/*
public class Controle : MonoBehaviour
{
    [SerializeField]
    Moviment_2 player;
    [SerializeField]
    Image[] img;

    [SerializeField]
    FixedJoystick joystick;

    bool press = false;

    Color vanila;
    Color pressed;

    private void Awake()
    {
        //vanila = new Color(212, 175, 99, 255);
        vanila = new Color(0.83f, 0.68f, 0.388f, 1);
        //pressed = new Color(140, 112, 55, 192);
        pressed = new Color(0.549f, 0.439f, 0.215f, 0.75f);
    }

    //Color preset = new Color(A4884E);
    void FixedUpdate()
    {
        Vector2 aux = joystick.Direction;
        if (aux == Vector2.zero)
        {
            if (press)
            {
                img[0].color = vanila;
                img[1].color = vanila;
                img[2].color = vanila;
                img[3].color = vanila;
                player._stop();
                press = false;
            }
        }
        else
        {
            press = true;

            if (aux.x > 0)
                if (aux.y > 0)
                {
                    player._move(Vector2.up);
                    img[0].color = vanila;
                    img[1].color = pressed;
                    img[2].color = vanila;
                    img[3].color = vanila;
                }
                else
                {
                    player._move(Vector2.right);
                    img[0].color = vanila;
                    img[1].color = vanila;
                    img[2].color = vanila;
                    img[3].color = pressed;
                }
            else
                if (aux.y > 0)
            {
                player._move(Vector2.left);
                img[0].color = pressed;
                img[1].color = vanila;
                img[2].color = vanila;
                img[3].color = vanila;
            }
            else
            {
                player._move(Vector2.down);
                img[0].color = vanila;
                img[1].color = vanila;
                img[2].color = pressed;
                img[3].color = vanila;
            }
        }
    }
}
 
 */