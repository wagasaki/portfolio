using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTextEffectController : MonoBehaviour
{
    [SerializeField]
    private RectTransform _clickImage;
    private bool _isReady = false;
    private TextEffectPool _textEffectPool;

    private Coroutine _gapRout;
    public bool InitOverlayController()
    {
        if (_isReady) return _isReady;

        DontDestroyOnLoad(this);

        _textEffectPool = Instantiate(Resources.Load<TextEffectPool>(Paths.TextEffectPool));
        _textEffectPool.SetPrefab(Resources.Load<TextEffect>(Paths.TextEffect));
        _textEffectPool.InitPool();
        _isReady = true;

        return _isReady;
    }

    public void TextEffect(string content)
    {
        if (_gapRout != null) return;
        _gapRout = StartCoroutine(gap());
        IEnumerator gap()
        {
            yield return YieldCache.WaitForSeconds(1);
            _gapRout = null;
        }
        TextEffect text = _textEffectPool.GetFromPool(0, this.transform);
        text.gameObject.SetActive(true);
        text.InitEffect(content, Vector2.zero);
    }
    Coroutine _clickEffectRout;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(_clickEffectRout!=null) StopCoroutine(_clickEffectRout);
            _clickEffectRout = null;
            _clickImage.gameObject.SetActive(false);
            _clickImage.position = Input.mousePosition;
            _clickImage.localScale = Vector2.zero;

            StartCoroutine(Enlarge());

            IEnumerator Enlarge()
            {
                _clickImage.gameObject.SetActive(true);
                float proc = 0;
                while(proc<1)
                {
                    proc += Time.unscaledDeltaTime * 10;
                    proc = Mathf.Clamp(proc, 0, 1);
                    _clickImage.localScale = Vector2.one * proc;
                    yield return null;
                }
                _clickImage.gameObject.SetActive(false);
            }
        }
    }

}
