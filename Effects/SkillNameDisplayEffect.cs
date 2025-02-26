using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNameDisplayEffect : MonoBehaviour
{
    private TextMeshProUGUI _skillNameText;
    private Image _backImage;
    private RectTransform _rect;
    
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _backImage = GetComponent<Image>();
        _skillNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
    }
    private void OnDisable()
    {
        _rect.anchoredPosition = new Vector2(-700, 0);
        _skillNameText.color = Color.clear;
    }

    public void InitEffect(Color color, string name)
    {
        _skillNameText.color = color;
        _skillNameText.text = name;
        _backImage.color = Color.white;
        StartCoroutine(MoveText());
    }
    IEnumerator MoveText()
    {
        Vector3 anchorPos = new Vector3(-350, 0, 0);
        Vector3 destPos = new Vector3(-350, 200, 0);
        Vector3 velocity = Vector3.zero;
        _rect.anchoredPosition = new Vector3(-700, 0,0);
        WaitForSeconds tick = new WaitForSeconds(0.5f);


        while(_rect.anchoredPosition.x < -351)
        {
            Vector3 pos = Vector3.SmoothDamp(_rect.anchoredPosition, anchorPos, ref velocity, 0.5f);
            _rect.anchoredPosition = pos;
            yield return null;
        }
        yield return tick;
        velocity = Vector3.zero;
        Color textColor = _skillNameText.color;
        Color backColor = Color.white;
        while(_rect.anchoredPosition.y < 199)
        {
            Vector3 pos = Vector3.SmoothDamp(_rect.anchoredPosition, destPos, ref velocity, 0.5f);
            _rect.anchoredPosition = pos;
            textColor.a = (200 - pos.y) / 200;
            backColor.a = textColor.a;
            _skillNameText.color = textColor;
            _backImage.color = backColor;
            yield return null;
        }
        gameObject.SetActive(false);
        yield return null;
    }
}
