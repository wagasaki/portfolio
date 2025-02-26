using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : Component
{
    protected Queue<T>[] _pool;

    [SerializeField]
    protected T[] _prefab;

    public void SetPrefab(T prefab)
    {
        T[] temp = _prefab;
        _prefab = new T[temp.Length + 1];
        for(int i = 0; i < temp.Length;i++)
        {
            _prefab[i] = temp[i];
        }
        _prefab[_prefab.Length - 1] = prefab;

    }
    public virtual void InitPool()
    {
        _pool = new Queue<T>[_prefab.Length];
        for (int i = 0; i < _prefab.Length; i++)
            _pool[i] = new Queue<T>();
    }

    public virtual T GetFromPool(int index, Transform parent = null)
    {

        //Debug.Log($"{index},{_pool.Length},{_pool[index]}");
        foreach(T a in _pool[index])
        {
            if (!a.gameObject.activeSelf)
                return a;
        }
        T b = Instantiate(_prefab[index], parent);
        _pool[index].Enqueue(b);

        return b;
    }


}
