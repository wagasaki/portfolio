using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapWayPointDataEntity
{
    public int Index;
    public string Keyword;
    public string MapName_Kor;
    public string MapName_Eng;
    public string LinkedWayPoint;
    public int WayPointLevel;
    public int MapPosX;
    public int MapPosY;
    public WayPointType WayPointType;
    public string EnemyType;
    public int EnemyIndex;
    public float EnemyHPModifier;
    public int Reward_Gold;
    public int Reward_Exp;
    public float Reward_Bonus;
    public string DropTable;
    public int Soul;
    public string WayPointTo;
    public int WayPointToIndex;
    public eBattleMap BattleMap;
    public eBackSound BackSound;
}
