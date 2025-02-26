using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropTableDisplayElem : MonoBehaviour
{
    private Image _itemImage;
    private Image _blockImage;
    private void Awake()
    {
        _itemImage = transform.Find("ItemImage").GetComponent<Image>();
        _blockImage = transform.Find("BlockImage").GetComponent<Image>();
    }

    public void InitElem(Sprite sprite, bool isAquired)
    {
        _itemImage.sprite = sprite;
        if (isAquired) _itemImage.color = Color.white;
        else _itemImage.color = Color.black;
        _blockImage.gameObject.SetActive(!isAquired); // isAquired = true면 끄고, false면 켠다

    }
}
