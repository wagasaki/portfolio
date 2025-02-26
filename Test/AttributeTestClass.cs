using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTestClass : MonoBehaviour
{
    [CustomAttribute("Very Important")]
    public int a;
}
public class CustomAttribute : Attribute  //SerializeField 같은것도 다 attribute임
{
    string _message;
    public CustomAttribute(string message)
    {
        _message = message;
        Debug.Log(_message);
    }
}