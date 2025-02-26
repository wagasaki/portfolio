using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoElem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject _panel;

    public void OnOff()//임시임
    {
        _panel.SetActive(!_panel.activeInHierarchy);
    }
}
