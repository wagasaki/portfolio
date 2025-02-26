using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum eTextEffectMoveType
{
    Upward,
    UpBounce,
}
public class DamageTextEffect : MonoBehaviour
{
    private TextMeshProUGUI _dmgText;
    private RectTransform _rect;
    private Camera _mainCam;
    private Color _defaultColor;
    private Coroutine _textMoveRout;
    private GameObject _criticalImage;
    private RectTransform _parent;
    private void Awake()
    {
        _dmgText = GetComponent<TextMeshProUGUI>();
        _dmgText.text = string.Empty;
        _defaultColor = _dmgText.color;
        _rect = GetComponent<RectTransform>();
        _criticalImage = transform.Find("CriticalText").gameObject;
        _mainCam = Camera.main;
        _parent = transform.parent.gameObject.GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        _criticalImage.SetActive(false);
        StopAllCoroutines();
    }
    private void OnDisable()
    {
        _criticalImage.SetActive(false);
        StopAllCoroutines();
    }
    public void SetDmgText(string content, eTextEffectMoveType moveType, Vector2 pos, bool isCrit)
    {
        gameObject.transform.localScale = Vector3.one;
        _dmgText.text = string.Format("-{0}",content);
        if (isCrit)
        {
            _criticalImage.SetActive(true);
            _dmgText.color = _defaultColor;//red
        }
        else
            _dmgText.color = Color.white;
        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(_mainCam, pos) + new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));
        screenPoint.z = 0;
        Vector2 screenPos;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, screenPoint, _mainCam, out screenPos))
        {
            _rect.anchoredPosition = screenPos;
        }


        _textMoveRout = StartCoroutine(MoveText(moveType));
    }
    public void SetAttackStateText(string content, eTextEffectMoveType moveType, Vector2 pos)
    {
        gameObject.transform.localScale = Vector3.one;
        _dmgText.text = content;
        ColorUtility.TryParseHtmlString("#FFB600", out Color color);
        _dmgText.color = color;
        _rect.anchoredPosition = RectTransformUtility.WorldToScreenPoint(_mainCam, pos) + new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));
        _textMoveRout = StartCoroutine(MoveText(moveType));
    }
    public void SetShieldText(string content, eTextEffectMoveType moveType, Vector2 pos)
    {
        gameObject.transform.localScale = Vector3.one;
        _dmgText.text = string.Format("+{0}", content);
        _dmgText.color = Color.cyan;
        _rect.anchoredPosition = RectTransformUtility.WorldToScreenPoint(_mainCam, pos) + new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));


        _textMoveRout = StartCoroutine(MoveText(moveType));
    }
    public void SetHealText(string content, eTextEffectMoveType moveType, Vector2 pos)
    {
        gameObject.transform.localScale = Vector3.one;
        _dmgText.text = string.Format("+{0}", content);
        _dmgText.color = Color.green;
        _rect.anchoredPosition = RectTransformUtility.WorldToScreenPoint(_mainCam, pos) + new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));


        _textMoveRout = StartCoroutine(MoveText(moveType));
    }

    private IEnumerator MoveText(eTextEffectMoveType moveType)
    {
        float proc = 0;
        float end = 0.5f;
        float sizetime = 0.6f;
        Color color = _dmgText.color;
        while (proc < sizetime)
        {
            if (proc < sizetime / 2)
            {
                transform.localScale = (1 + proc) * Vector3.one;
            }
            else
            {
                transform.localScale = (1 + sizetime / 2 - proc) * Vector3.one;
            }
            proc += Time.deltaTime;
            yield return null;
        }
        proc = 0;
        while (proc < end)
        {//850 820 950 970
            _rect.anchoredPosition += new Vector2(0, 1 - proc) * Time.deltaTime * 200;
            color.a = 1.5f - proc;
            _dmgText.color = color;

            transform.localScale = new Vector3(1, 1 - proc, 1);


            proc += Time.deltaTime;

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
