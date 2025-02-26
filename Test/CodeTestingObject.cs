using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System.Reflection;
using System.Text;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using System.Linq;

public class CodeTestingObject : MonoBehaviour
{
    private InventoryIterator _inventoryIterator;
    private InvenInfo[] _invenInfos = { new InvenInfo("No1", 0, false, 0), new InvenInfo("No2", 1, false, 0), new InvenInfo("No3", 2, false, 0) };
    private MonsterInfo[] _monInfos = { new MonsterInfo("Goblin", 0, 10), new MonsterInfo("Orc", 1, 20), new MonsterInfo("Harpy", 2, 15) };
    private void Start()
    {
        Iterator inven = new InventoryIterator(_invenInfos);
        Iterator mon = new MonsterIterator(_monInfos);
        ShowIterator(inven);
        ShowIterator(mon);
    }
    void ShowIterator(Iterator iterator)
    {
        for(int i = 0; i < iterator.GetLength(); i++)
        {
            InfoBase info = iterator.Next();
            Debug.Log($"{info.Name}, {info.Index}, {info.IsAquired}, {info.Count}, {info.HP}");
        }
    }
}
public class InventoryIterator : Iterator
{
    private List<InfoBase> _infos = new List<InfoBase>();
    private int _index = -1;
    public InventoryIterator(InfoBase[] array)
    {
        for(int i = 0; i< array.Length;i++)
        {
            _infos.Add(array[i]);
        }
        _index = 0;
    }
    public bool HasNext()
    {
        bool hasNext = _index < _infos.Count;
        return hasNext;
    }

    public InfoBase Next()
    {
        return _infos[_index++];
    }
    public int GetLength()
    {
        return _infos.Count;
    }
    public void Remove(InfoBase _target)
    {
        _infos.Remove(_target);
    }
}
public class MonsterIterator : Iterator
{
    private List<InfoBase> _infos = new List<InfoBase>();
    private int _index = -1;
    public MonsterIterator(MonsterInfo[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            _infos.Add(array[i]);
        }
        _index = 0;
    }
    public bool HasNext()
    {
        bool hasNext = _index < _infos.Count;
        return hasNext;
    }

    public InfoBase Next()
    {
        return _infos[_index++];
    }
    public int GetLength()
    {
        return _infos.Count;
    }
    public void Remove(InfoBase _target)
    {
        _infos.Remove(_target);
    }
}
public interface Iterator
{
    bool HasNext();
    InfoBase Next();
    void Remove(InfoBase _target);
    int GetLength();
}

public class InvenInfo : InfoBase
{
    public InvenInfo(string name, int index, bool isAquired, int count)
    {
        Name = name;
        Index = index;
        IsAquired = isAquired;
        Count = count;
    }
}
public class MonsterInfo : InfoBase
{
    public MonsterInfo(string name, int index, int hp)
    {
        Name = name;
        Index = index;
        HP = hp;
    }
}
public class InfoBase
{
    public string Name = string.Empty;
    public int Index = 0;
    public bool IsAquired = false;
    public int Count = 0;
    public int HP;
}