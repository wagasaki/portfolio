using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectElem : UIBase
{
    enum GameObjects
    {
        Moveback,
    }
    enum Texts
    {
        NameText,
        GradeText,
        StatContentText,
    }
    enum Images
    {
        ItemImage,
    }
    private RectTransform _moveBack;
    private void Awake()
    {
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        _moveBack = GetObject((int)GameObjects.Moveback).GetComponent<RectTransform>();
        Vector2 startpos = Vector2.down * 500;
        _moveBack.anchoredPosition = startpos;
    }
    public void InitElem()
    {
        StartCoroutine(Move());
    }
    IEnumerator Move()
    {
        Vector2 startpos = Vector2.down * 500;
        _moveBack.anchoredPosition = startpos;
        while (startpos.y<0)
        {
            startpos.y += Time.deltaTime * 800;
            _moveBack.anchoredPosition = startpos;
            yield return null;
        }
        _moveBack.anchoredPosition = Vector2.zero;
    }
}
