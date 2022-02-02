using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moviment : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    [SerializeField]
    Vector2 x, y;
    [SerializeField]
    float temp = 0.1f, vel;

    [SerializeField]
    Vector2 initial, input;

    [SerializeField]
    Transform obj, obj2;

    bool able;

    void Start()
    {
        input = new Vector2();
        obj.position = initial;
        obj2.position = initial;
        able = true;
    }

    void Update()
    {
        if (able)
        {
            if ((Input.GetAxisRaw("Horizontal") != 0) | (Input.GetAxisRaw("Vertical") != 0))
            {
                able = false;

                if (Input.GetAxisRaw("Horizontal") != 0)
                    if ((Input.GetAxisRaw("Horizontal") > 0))
                    {
                        input.x += 1;
                        anim.SetInteger("dir", 4);
                    }
                    else
                    { 
                        input.x -= 1;
                        anim.SetInteger("dir", 2);
                    }

                if (Input.GetAxisRaw("Vertical") != 0)
                    if ((Input.GetAxisRaw("Vertical") > 0))
                    {
                        input.y += 1;
                        anim.SetInteger("dir", 1);
                    }
                    else
                    {
                        input.y -= 1;
                        anim.SetInteger("dir", 3);

                    }

                StartCoroutine(Move());
            }
        }
    }

    IEnumerator Move()
    {
        Vector2 aux = initial + input.x * x + input.y * y;
        obj2.position = aux;
        while (Vector2.Distance( aux, obj.position) > 0.01f )
        {
            obj.position = Vector2.MoveTowards(obj.position, aux, vel*Time.deltaTime);
            yield return null;
            //yield return new WaitForSeconds(temp);
        }
        anim.SetInteger("dir", -anim.GetInteger("dir"));
        able = true;
    }

    void Be_Able()
    {
        able = true;
    }

}
