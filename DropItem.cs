using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private Vector2 dest = new Vector2(1.3f, -1.47f);
    private void OnEnable()
    {
        transform.position = new Vector2(1.3f, 0f);
        transform.rotation = Quaternion.identity;
        StopAllCoroutines();
        StartCoroutine(MoveToRoutine());
    }

    IEnumerator MoveToRoutine()
    {
        float proc = 0;

        while(proc < 1)
        {
            this.transform.position = Vector2.Lerp(this.transform.position, dest, proc);

            if (transform.position.y < -1.3f)
            {
                transform.position = dest;
                proc = 1;
            }
            proc += Time.deltaTime;
            yield return null;
        }
        bool right = true;
        float angle = 15;
        float current = 0;
        proc = 5;
        while (proc>0)
        {
            if (angle <= 0) break;
            if(right)
            {
                current -= Time.deltaTime * 100;
                transform.rotation = Quaternion.Euler(0, 0, current);
                if(current <= -angle)
                {
                    right = false;
                    angle -= 2;
                }
            }
            else
            {
                current += Time.deltaTime * 100;
                transform.rotation = Quaternion.Euler(0, 0, current);
                if (current >= +angle)
                {
                    right = true;
                    angle -= 2;
                }
            }

            proc -= Time.deltaTime;
            yield return null;
        }
    }
}
