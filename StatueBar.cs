using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatueBar : MonoBehaviour
{
    [SerializeField]
    private Image _hpGauge, _turnGauge, _manaGauge;
    private float _currentHP;

    private Coroutine _hpRout;
    public void HP(float hp)
    {
        //Debug.Log(hp);
        //if (_hpRout != null) StopCoroutine(_hpRout);
        //_hpRout = null;
        //_hpRout = StartCoroutine(HPRoutine(hp));
        _hpGauge.fillAmount = hp;
    }
    private IEnumerator HPRoutine(float hp)
    {
        float current = _hpGauge.fillAmount;
        float gap = current-hp;
        while(current>=hp)
        {

            current = current - gap * Time.deltaTime*5;
            _hpGauge.fillAmount = current;
            yield return null;
        }
    }
    public void Turn(float turn)
    {
        _turnGauge.fillAmount = turn;
    }
    public void Mana(float mana)
    {
        _manaGauge.fillAmount = mana;
    }
}
