using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTest : MonoBehaviour
{
    // Start is called before the first frame update
    Camera main;
    Vector3 pos = Vector3.zero;
    RectTransform rect;
    public GameObject image;
    void Start()
    {
        rect = GetComponent<RectTransform>();
        main = Camera.main;
        //Debug.Log(main.ScreenToWorldPoint(this.transform.position));
        //main.WorldToScreenPoint(this.transform.position);

        //rect.anchoredPosition = RectTransformUtility.WorldToScreenPoint(main, pos);
        //Debug.Log(pos);
        //rect.transform.position = Vector3.one;
    }
    private void Update()
    {
        //Debug.Log(string.Format("{0}, {1}, {2}", main.ScreenToWorldPoint(this.rect.position), main.ScreenToWorldPoint(this.rect.localPosition), transform.position));
        //rect.anchoredPosition = RectTransformUtility.WorldToScreenPoint(main, pos);
        //Debug.Log(pos);
        //Debug.Log(main.ScreenToWorldPoint(Input.mousePosition));
        //Vector3 vec = main.ScreenToWorldPoint(Input.mousePosition);
        //vec.z = 10;
        //this.transform.position = vec;
        Debug.Log(main.ScreenToWorldPoint(transform.position));
    }
}
