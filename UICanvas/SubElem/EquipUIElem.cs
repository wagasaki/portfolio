using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class EquipUIElem<T> : UIBase
{
    protected T _dataEntity;
    protected ShopUIPopup _callShopUIPopup;

    protected TextMeshProUGUI _nameText, _costText, _collectionCountText, _holdingStatValueText;
    protected GameObject _equippedTag, _panelObject, _costImage;
    protected Button _buyButton;

    protected int _cost;

    protected int _maxHoldingCount;
    protected float _holdingValue;
    protected bool _isBuyable;

    public abstract void InitElem(T dataEntity, bool isAcquired, ShopUIPopup shop);


    protected virtual void ShowEquipPopup()
    {
        Debug.Log("유아이 클릭");
        InitController.Instance.Sounds.PlaySFX(eSFX.Click_Light);
    }
    public void Equip(bool isEquip)
    {
        _equippedTag.SetActive(isEquip);
    }
    protected void InitPanelObject(T dataEntity, bool isAcquired)
    {
        _panelObject.SetActive(!isAcquired);

        if (_isBuyable)
        {
            _panelObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);
            _costImage.SetActive(true);
            _costText.text = _cost.ToString("n0");
            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(() => { AcquireThisItem(dataEntity); });
        }
        else
        {
            _panelObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            _costImage.SetActive(false);
        }
    }
    protected void ColorCostText(int gold)
    {
        if (gold < _cost)
        {
            _costText.color = Color.red;
        }
        else
        {
            _costText.color = Color.white;
        }
    }
    protected void AcquireThisItem(T dataEntity)
    {
        if (InitController.Instance.GamePlays.UseGold(_cost))
        {
            Debug.Log("구입성공");
            InitController.Instance.Sounds.PlaySFX(eSFX.Click_Success);
            int count = SetItemDataOnEquipReturnCount();

            InitController.Instance.GamePlays.IngameStat.SetItemOwnedStat();

            _collectionCountText.text = string.Format("{0}/{1}", count, _maxHoldingCount);
            _holdingStatValueText.text = Mathf.Ceil(_holdingValue * count).ToString();
            _panelObject.SetActive(false);
        }
        else //로직 통과 못할 시
        {
            InitController.Instance.Sounds.PlaySFX(eSFX.Click_Failed);
            Debug.Log("구입실패 - 이유출력");
        }
    }
    protected abstract int SetItemDataOnEquipReturnCount();
}
