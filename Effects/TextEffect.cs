using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
    private RectTransform _rect;
    public Image _back;
    public Color _backColor;
    public TextMeshProUGUI EffectText { get; set; }
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        EffectText = GetComponentInChildren<TextMeshProUGUI>();
        _back = GetComponent<Image>();
        _backColor = _back.color;
    }
    private void OnEnable()
    {
        EffectText.text = string.Empty;
        StopAllCoroutines();
    }
    private void OnDisable()
    {
        EffectText.text = string.Empty;
        StopAllCoroutines();
    }
    public void InitEffect(string content, Vector2 pos)
    {
        EffectText.text = content;
        _backColor.a = 0.4f;
        _back.color = _backColor;

        _rect.anchoredPosition = pos;
        StartCoroutine(MoveText());
    }
    public void InitWithLocalizeText(Vector2 pos)
    {
        _backColor.a = 1f;
        _back.color = _backColor;
        _rect.anchoredPosition = pos;
        StartCoroutine(fade());
        IEnumerator fade()
        {
            yield return YieldCache.WaitForSeconds(2f);
            gameObject.SetActive(false);
        }
    }
    IEnumerator MoveText()
    {
        float proc = 0;
        while (proc < 1)
        {
            _rect.anchoredPosition += new Vector2(0, proc) * Time.deltaTime * 200;
            proc += Time.deltaTime;

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
