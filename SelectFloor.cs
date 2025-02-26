using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectFloor : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _openedFloorText, _currentFloorText;
    [SerializeField]
    private Button _moveBtn, _upOneBtn, _upTenBtn, _downOneBtn, _downTenBtn, _closeBtn;

    private int _currentFloor;
    private int _openedFloor;
    public int CurrentFloor
    {
        get { return _currentFloor; }
        set { 
            _currentFloor = value;
            _currentFloorText.text = $"오크 군락지 - 지하 {_currentFloor}층";
        }
    }
    public int OpenedFloor
    {
        get { return _openedFloor; }
        set
        {
            _openedFloor = value;
            _openedFloorText.text = _openedFloor.ToString() + "층 까지 개방됨";
        }
    }
    //public void InitSelectFloor(int currentfloor)
    //{
    //    _currentFloor = currentfloor;
    //    _currentFloorText.text = $"오크 군락지 - 지하 {_currentFloor}층";
    //    OpenedFloor = 10;
    //    _moveBtn.onClick.AddListener(() =>
    //    {
    //        this.gameObject.SetActive(false);
    //        GamePlayController.instance.CurrentLevel = _currentFloor;
    //        GamePlayController.instance.SetBattle(GamePlayController.instance.CurrentLevel);
    //        });

    //    _upOneBtn.onClick.AddListener(() => FloorSearch(true, 1));
    //    _upTenBtn.onClick.AddListener(() => FloorSearch(true, 10));
    //    _downOneBtn.onClick.AddListener(() => FloorSearch(false, 1));
    //    _downTenBtn.onClick.AddListener(() => FloorSearch(false, 10));
    //    _closeBtn.onClick.AddListener(() => this.gameObject.SetActive(false));
    //}

    //public void FloorSearch(bool dir, int amount)
    //{
    //    if(dir)
    //    {
    //        if (_currentFloor + amount <= GamePlayController.instance.MapDataEntity.Count)
    //        {
    //            _currentFloor += amount;
    //        }
    //        else
    //        {
    //            _currentFloor = GamePlayController.instance.MapDataEntity.Count;
    //        }
    //    }
    //    else
    //    {
    //        if (_currentFloor - amount >= 1)
    //        {
    //            _currentFloor -= amount;
    //        }
    //        else
    //        {
    //            _currentFloor = 1;
    //        }
    //    }
    //    _currentFloorText.text = $"오크 군락지 - 지하 {_currentFloor}층";
    //}
}
