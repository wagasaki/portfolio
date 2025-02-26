
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;


public class CharacterToolTip : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI _toolTipText;
    private GameObject _toolTips;
    private Coroutine _toolTipRout;


    private int _tooltipCount;
    private List<int> _tooltipIndexlist;
    StringTable _table;
    private bool _isReady = false;
    private LobbyUICharacter _lobbyCharacter;
    public void OnPointerClick(PointerEventData eventData)
    {
        //if (_isReady == false) return;
        ShowTooltips();
    }
    private void Awake()
    {
        _toolTips = Utils.FindChild<RectTransform>(gameObject, "Tooltips").gameObject;
        _toolTipText = _toolTips.transform.Find("TooltipText").GetComponent<TextMeshProUGUI>();
        _toolTipText.rectTransform.sizeDelta = new Vector2(_toolTipText.rectTransform.sizeDelta.x, _toolTipText.preferredHeight * 1.1f);
        _toolTipText.text = string.Empty;
        _toolTips.SetActive(false);
        _lobbyCharacter = transform.root.GetComponent<LobbyUICharacter>();
    }
    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
  
        //_table = LocalizationSettings.StringDatabase.GetTable("Tooltip");
        var table = LocalizationSettings.StringDatabase.GetTableAsync("Tooltip");
        yield return table;
        _table = table.Result;
        _tooltipCount = _table.Count;
        _tooltipIndexlist = new List<int>(_tooltipCount);
        for (int i = 0; i < _tooltipCount; i++)
        {
            _tooltipIndexlist.Add(i);
        }
        if(_lobbyCharacter != null)
        _lobbyCharacter.OnIdleAction += () => ShowTooltips();

        _isReady = true;
    }
    public void ShowTooltips()
    {
        if (_isReady == false) return;
        _isReady = false;
        _toolTips.SetActive(true);
        if (_tooltipIndexlist == null)
        {
            _tooltipIndexlist = new List<int>(_tooltipCount);
            for (int i = 0; i < _tooltipCount; i++)
            {
                _tooltipIndexlist.Add(i);
            }
        }

        int index = Random.Range(0, _tooltipIndexlist.Count);

        int rand = _tooltipIndexlist[index];
        _tooltipIndexlist.RemoveAt(index);

        if (_tooltipIndexlist.Count == 0)
        {
            _tooltipIndexlist.Clear();
            _tooltipIndexlist = new List<int>(_tooltipCount);
            for (int i = 0; i < _tooltipCount; i++)
            {
                _tooltipIndexlist.Add(i);
            }
        }

        var table = LocalizationSettings.StringDatabase.GetTableEntryAsync("Tooltip", rand.ToString());
        table.Completed += (result) =>
        {
            if (_toolTipText == null) return;
            _toolTipText.text = result.Result.Entry.Value.ToString();
            _toolTipText.rectTransform.sizeDelta = new Vector2(_toolTipText.rectTransform.sizeDelta.x, _toolTipText.preferredHeight * 1.1f);
            if (_toolTipRout != null)
            {
                StopCoroutine(_toolTipRout);
                _toolTipRout = null;
            }
            _isReady = true;
            _toolTipRout = StartCoroutine(fade());
        };
        IEnumerator fade()
        {
            yield return YieldCache.WaitForSeconds(3);
            _toolTipText.text = string.Empty;
            _toolTips.SetActive(false);
            _toolTipRout = null;
        }
    }

}

