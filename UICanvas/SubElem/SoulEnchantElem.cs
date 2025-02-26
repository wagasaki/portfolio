using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoulEnchantElem : UIBase
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI _levelText, _nameText;
    [SerializeField] Image _glowImage;
    [SerializeField] Image _enchantIcon;

    private int _maxLv;
    public Sprite GetSprite { get { return _enchantIcon.sprite; } }
    private int _index;
    public string GetKey { get; private set; }
    public void SetElemData(int index, string key, int lv, int maxlv, Action<int> OnClicked)
    {
        _index = index;
        GetKey = key;
        _maxLv = maxlv;
        _levelText.text = string.Format("Lv {0}/{1}", lv, maxlv);
        button.onClick.AddListener(() =>
        {
            OnClicked?.Invoke(_index);
        });
    }
    public void RefreshData(int currentlv)
    {
        _levelText.text = string.Format("Lv {0}/{1}", currentlv, _maxLv);
    }
    public void OnSelected(bool isSelected)
    {
        _glowImage.gameObject.SetActive(isSelected);
    }
}
