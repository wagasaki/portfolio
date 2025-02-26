using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Utils
{
    public static T ParseEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static int EnumCount<T>()
    {
        return System.Enum.GetValues(typeof(T)).Length;
    }
    public static T GetOrAddComponent<T>(GameObject go)where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>타입
    /// <param name="go"></param>부모오브젝트
    /// <param name="name"></param>찾을 이름
    /// <param name="recursive"></param>반복여부
    /// <returns></returns>
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
                return transform.GetComponent<T>();
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>부모 오브젝트
    /// <param name="name"></param>찾을 이름
    /// <param name="recursive"></param>반복여부
    /// <returns></returns>
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }

    /// <summary>
    /// 플레이어 스탯같은경우는 여기서 계산 안하고 따로 할거. 근데 일단 기록용으로 여기 isPlayer 안에 넣어둠. 나중에 빼서 다른 함수로 ㄱㄱ 
    /// 기본스탯에 +10씩 해주는건 플레이어랑 같음. 근데 플레이어는 ingamestat자체에서 +10을 해주는거라... 나중에서 거기서는 0으로 시작하고 따로 함수만든거에서 +10해주면될듯? 이 함수처럼
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="level"></param>
    /// <param name="strengthRatio"></param>
    /// <param name="agilityRatio"></param>
    /// <param name="vitalityRatio"></param>
    /// <param name="luckRatio"></param>
    /// <param name="result"></param>
    public static void CalcStat(bool isPlayer, int level, int strengthRatio, int agilityRatio, int vitalityRatio, int luckRatio, out float[] result)
    {
        int statpoint = (level) * Nums.StatPointPerLevelUp;
        float sumRatio = strengthRatio + agilityRatio + vitalityRatio + luckRatio;


        result = new float[5];

        float Strength = Mathf.Round(statpoint * (strengthRatio / sumRatio)) + 10; 
        float Agility = Mathf.Round(statpoint * (agilityRatio / sumRatio)) + 10;
        float Vitality = Mathf.Round(statpoint * (vitalityRatio / sumRatio)) + 10;

        float HP;
        float Atk;
        float Def;
        float MinDmg;
        float MaxDmg;

        if (isPlayer)
        {
            HP = Vitality * 10;
            Atk = Strength;
            Def = Agility;
            MinDmg = level * 1;
            MaxDmg = level * 1;
        }
        else
        {
            HP = Vitality * 10;
            Atk = Strength;
            Def = Agility;
            MinDmg = ((level-1) * 2 + 20) * 0.9f;
            MaxDmg = ((level-1) * 2 + 20) * 1.1f;
        }

        result[0] = HP;
        result[1] = Atk;
        result[2] = Def;
        result[3] = MinDmg;
        result[4] = MaxDmg;
    }
}
public class PriorityQueue<T> where T : IComparable<T>
{
    List<T> _heap = new List<T>();

    public void Push(T data)
    {
        _heap.Add(data);

        int now = _heap.Count - 1;
        while(now>0)
        {
            int next = (now - 1) / 2; //하위 노드의 부모 노드 인덱스.
            if (_heap[now].CompareTo(_heap[next]) < 0)
                break;

            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;

            now = next;
        }
    }

    public T Pop()
    {
        if (_heap.Count <= 0) return default;

        T root = _heap[0];

        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        lastIndex--;

        int now = 0;
        while(true)
        {
            int left = now * 2 + 1;
            int right = now * 2 + 2;

            int next = now;

            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                next = left;
            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                next = right;
            if (next == now)
                break;

            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;
        }
        return root;
    }
    public int Count()
    {
        return _heap.Count;
    }
}
public struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Index;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return F < other.F ? 1 : -1;
    }
}
public struct WayNod
{
    public int Index;
    public List<int> NearNodesIndex;

    public WayNod(int index, List<int> near)
    {
        NearNodesIndex = new List<int>();
        Index = index;
        NearNodesIndex = near;
    }

}
public class Astar
{
    public static int CalcCost(int currentIndex,int destIndex, WayNod[] nodes)
    {
        int result = int.MaxValue;


        bool[] closed = new bool[nodes.Length];
        int[] open = new int[nodes.Length];

        for (int i = 0; i < nodes.Length; i++)
            open[i] = int.MaxValue;

        WayNod[] parent = new WayNod[nodes.Length];

        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();


        //시작점 발견 후 시작
        open[currentIndex] = 0;
        pq.Push(new PQNode() { F = 0, G = 0, Index = currentIndex });
        parent[currentIndex].Index = currentIndex;

        while(pq.Count()>0)
        {
            PQNode node = pq.Pop();
            int now = node.Index;
            if (closed[node.Index])
                continue;

            closed[node.Index] = true;
            if (node.Index == destIndex)
                break;
            for(int i = 0; i < nodes[now].NearNodesIndex.Count;i++)
            {
                int nextIndex = nodes[now].NearNodesIndex[i];
                if (closed[nodes[nextIndex].Index])
                    continue;

                int g = node.G + 1;

                if (open[nodes[nextIndex].Index] < g)
                    continue;

                open[nodes[nextIndex].Index] = g;
                pq.Push(new PQNode { F = g, G = g, Index = nodes[now].NearNodesIndex[i] });
                parent[nodes[nextIndex].Index] = nodes[now]; //nodes[nodes[currentIndex].NearNodesIndex[i]]; 
                //currentIndex = nodes[nextIndex].Index;
            }
        }

        Debug.Log(parent.Length);


        int dest = destIndex;
        List<WayNod> points = new List<WayNod>();
        while(parent[dest].Index != dest)
        {
            points.Add(nodes[dest]);
            dest = parent[dest].Index;
        }
        points.Add(parent[dest]);

        foreach(var a in points)
        {
            Debug.Log(a.Index);
        }


        return result;
    }

    public static int[] CalcWays(int currentIndex, int destIndex, WayNod[] nodes)
    {
        bool[] closed = new bool[nodes.Length];
        int[] open = new int[nodes.Length];

        for (int i = 0; i < nodes.Length; i++)
            open[i] = int.MaxValue;

        WayNod[] parent = new WayNod[nodes.Length];

        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();


        //시작점 발견 후 시작
        open[currentIndex] = 0;
        pq.Push(new PQNode() { F = 0, G = 0, Index = currentIndex });
        parent[currentIndex].Index = currentIndex;

        while (pq.Count() > 0)
        {
            PQNode node = pq.Pop();
            int now = node.Index;
            if (closed[node.Index])
                continue;

            closed[node.Index] = true;
            if (node.Index == destIndex)
                break;
            for (int i = 0; i < nodes[now].NearNodesIndex.Count; i++)
            {
                int nextIndex = nodes[now].NearNodesIndex[i];
                if (closed[nodes[nextIndex].Index])
                    continue;

                int g = node.G + 1;

                if (open[nodes[nextIndex].Index] < g)
                    continue;

                open[nodes[nextIndex].Index] = g;
                pq.Push(new PQNode { F = g, G = g, Index = nodes[now].NearNodesIndex[i] });
                parent[nodes[nextIndex].Index] = nodes[now];
            }
        }

        int dest = destIndex;
        List<WayNod> points = new List<WayNod>();
        while (parent[dest].Index != dest)
        {
            points.Add(nodes[dest]);
            dest = parent[dest].Index;
        }
        points.Add(parent[dest]);

        //역순이라 마지막이 자기 자신
        int[] ways = new int[points.Count];
        for(int i = 0; i < ways.Length;i++)
        {
            ways[i] = points[points.Count - i-1].Index;
        }
        return ways;
    }
}
