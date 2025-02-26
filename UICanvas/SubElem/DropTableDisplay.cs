using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropTableDisplay : MonoBehaviour
{
    List<DropTableDisplayElem> _dropTableElemList = new List<DropTableDisplayElem>();
    readonly int droptablecount = 12;

    private void Awake()
    {
        DropTableDisplayElem elemPrefab = Resources.Load<DropTableDisplayElem>(Paths.DropTableDisplayElem);

        for(int i = 0; i < droptablecount; i++)
        {
            DropTableDisplayElem elems = Instantiate(elemPrefab, this.transform);
            elems.gameObject.SetActive(false);
            _dropTableElemList.Add(elems);
        }
    }

    public void InitDisplay()
    {
        Dictionary<string,bool> drops = InitController.Instance.GameDatas.IsDropTableItemsIsAquired_itemIndex(InitController.Instance.GamePlays.CurrentMapWayData);

        int constraintcount = (drops.Count - 1) / 5;
        GetComponent<GridLayoutGroup>().constraintCount = constraintcount+1;

        for (int i = 0; i<_dropTableElemList.Count;i++)
        {
            if(i<drops.Count)
                _dropTableElemList[i].gameObject.SetActive(true);
            else
                _dropTableElemList[i].gameObject.SetActive(false);
        }


        int index = 0;
        foreach (var a in drops)
        {
            Sprite sprite = InitController.Instance.GameDatas.AllItemSpriteDic[a.Key];

            _dropTableElemList[index].InitElem(sprite, a.Value);
            index++;
            if(index >= droptablecount)
            {
                break;
            }
        }

    }
}
