using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LerpTest : MonoBehaviour
{
    private TextMeshProUGUI _text;
    //int max = 500;
    //float start = 0;
    //float mainnum;

    public Transform center;
    public Transform target;

    int A = 5;
    int[] B = new int[] { 0, 1, 2 };
    RefTestClass test = new RefTestClass(0, new int[] { 1, 2, 3 });
    private void Start()
    {
        //_text = GetComponent<TextMeshProUGUI>();

        //center.position = new Vector2(3, 4);
        //target.position = new Vector3(1, 4);
        //Debug.Log(center.TransformDirection(target.position));



        RefTest(A, B, test);

    }

    private void Update()
    {
        #region 러프테스트
        //start += Time.deltaTime;
        //float dest = Mathf.Log10(1 + start * 0.1f);
        //if (dest >= 0.1f) dest = 1;
        //mainnum = Mathf.Lerp(mainnum, max, dest);

        //Debug.Log(mainnum +"/"+Mathf.Log10(1+ start*0.1f));
        //_text.text = mainnum.ToString();
        #endregion
        #region TransformDirection테스트
        //Debug.Log(center.TransformDirection(target.position));
        #endregion
        #region

        #endregion
    }


    public void RefTest(int a, int[] b, RefTestClass c)
    {
        Debug.Log($"A : {A}, B : {B[0]}/ a : {a}, b : {b[0]}/ C : {test.A},{test.B[0]}, c: {c.A},{c.B[0]}");

        a = 10;
        b[0] = 20;
        c.A = 5;
        c.B[0] = 3;
        Debug.Log($"A : {A}, B : {B[0]}/ a : {a}, b : {b[0]}/ C : {test.A},{test.B[0]}, c: {c.A},{c.B[0]}");

        b = new int[3];
        c = new RefTestClass(100, new int[] {200,300,400 });
        Debug.Log($"A : {A}, B : {B[0]}/ a : {a}, b : {b[0]}/ C : {test.A},{test.B[0]}, c: {c.A},{c.B[0]}");

        b[0] = 21;
        c.A = 1002;
        c.B[0] = 450;
        Debug.Log($"A : {A}, B : {B[0]}/ a : {a}, b : {b[0]}/ C : {test.A},{test.B[0]}, c: {c.A},{c.B[0]}");
    }
}
public class RefTestClass
{
    public int A;
    public int[] B;
   
    public RefTestClass(int a, int[] b)
    {
        A = a;
        B = b;
    }
}
