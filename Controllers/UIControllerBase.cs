using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UICanvasIndex
{
    UIPopup,
    WeaponUIPopup,
}
public enum PermanantUI
{
    BattleUI,
    MapUI,
}
public class UIControllerBase : MonoBehaviour
{
    protected Stack<UIPopup> _uiPopupStack;
    protected UIPopup[] _uiPopupPrefab;
    protected LoadingPanelCanvas _loadingPanelCanvas;
    protected LoadingPanelCanvas _loadingPanelCanvasPrefab;
    protected List<UICanvas> _permenantUICanvasList;
    public Stack<UIPopup> CurrentPopupStack { get { return _uiPopupStack; } }
    public bool IsESC_Usable { get; set; }

    public T OpenUIPopup<T>(string name = null, Transform parent = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

                                            
        GameObject go = Instantiate(Resources.Load<GameObject>(string.Concat(Paths.Popup, name)));
        T popup = Utils.GetOrAddComponent<T>(go);

        _uiPopupStack.Push(popup);

        if (parent != null)
            go.transform.SetParent(parent);
        else
        {
            go.transform.SetParent(transform);
        }
        go.transform.localScale = Vector3.one;
        popup.InitSortOrder(_uiPopupStack.Count);
        return popup;
    }
    public void CloseUIPopup(UIPopup popup)
    {
        if (_uiPopupStack.Count == 0) return;
        UIPopup peek = _uiPopupStack.Peek();

        if (popup != peek)
        {
            Debug.Log($"close popup failed / popup = {popup.GetInstanceID()}+{popup.name}, peek = {peek.GetInstanceID()}+{peek.name}");

            return;
        }
        Debug.Log("close" + popup.name);
        CloseUIPopup();
    }
    public void CloseUIPopup()
    {
        if (_uiPopupStack.Count == 0)
            return;

        UIPopup popup = _uiPopupStack.Pop();

        if (popup.gameObject == null) return;
        popup.OnCloseUIPopup();
        Destroy(popup.gameObject);

        popup = null;

    }
    public void CloseAllUIPopup()
    {
        while (_uiPopupStack.Count > 0)
            CloseUIPopup();
    }

    public UIPopup GetUIPopup(string popupname)
    {
        foreach (var a in _uiPopupStack)
        {
            if (a.GetType().ToString() == popupname)
                return a;
        }
        return null;
    }    

    public void LoadingPanel(PermanantUI current, PermanantUI target, Action duringFadeAction, Action afterFadeAction = null)
    {
        if(_loadingPanelCanvas== null||_loadingPanelCanvas.gameObject == null)
        {
            Debug.Log("Panel disappear");
            return;
        }
        _loadingPanelCanvas.gameObject.SetActive(true);
        IsESC_Usable = false;
        Action actionDuringFadeOut = () =>
        {
            _permenantUICanvasList[(int)current].gameObject.SetActive(false);
            RefreshPermanantUI(current, false);
            RefreshPermanantUI(target, true);
            if (target == PermanantUI.MapUI)
            {
                _permenantUICanvasList[(int)PermanantUI.MapUI].gameObject.SetActive(true);
                _permenantUICanvasList[(int)PermanantUI.MapUI].GetComponent<MapUICanvas>().AfterBattleAdjustMapEffect();
                RefreshPermanantUI(PermanantUI.MapUI, false);
            }
            else if (target == PermanantUI.BattleUI)
            {
                RefreshPermanantUI(PermanantUI.MapUI, true);
                _permenantUICanvasList[(int)PermanantUI.MapUI].gameObject.SetActive(false);
            }
            duringFadeAction?.Invoke();

            _permenantUICanvasList[(int)target].gameObject.SetActive(true);
        };

        _loadingPanelCanvas.FadeInOout(actionDuringFadeOut, afterFadeAction);
    }

    public void RefreshPermanantUI(PermanantUI ui, bool isTarget)
    {
        _permenantUICanvasList[(int)ui].RefreshUI(isTarget);
    }
}
