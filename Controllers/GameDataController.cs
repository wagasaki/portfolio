using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameDataController
{
    private bool _isReady = false;

    private MapWayPointData MapWayPointDatas;

    public Dictionary<string, List<MapWayPointDataEntity>> MapWayPointDataEntityDic { get; private set; }
    public Dictionary<string, Sprite> MapSpriteDic { get; private set; }

    //public CharacterData CharacterDatas { get; private set; }
    public EnemyData EnemyDatas { get; private set; }
    public Dictionary<string, List<EnemyDataEntity>> EnemyDataDic { get; private set; }
    public Dictionary<string, Dictionary<string, Sprite>> EnemySpriteDic { get; private set; }

    public LocalizationData LocalizationDatas { get; private set; }
    public Dictionary<string, string[]> LocalizationDataDic { get; private set; }

    public WeaponData WeaponDatas { get; private set; }
    public ArmorData ArmorDatas { get; private set; }
    public AccessoryData AccessoryDatas { get; private set; }
    public SkillStoneData SkillStoneDatas { get; private set; }

    public Dictionary<string, Sprite> AllItemSpriteDic { get; private set; }
    public Dictionary<string, Sprite> SpriteDic { get; private set; }
    public SkillData SkillDatas { get; private set; }
    public SkillContentData SkillContentDatas { get; private set; }
    public Dictionary<string, SkillDataEntity> SkillDataDic { get; private set; }
    public Dictionary<string, string[]> SkillContentDataDic { get; private set; }
    public Dictionary<string, Sprite> SkillIconDic { get; private set; }
    public Dictionary<string, Sprite> StateIconDic { get; private set; }
    public Dictionary<eSFX, AudioClip> SFXClipDic { get; private set; }
    public Dictionary<string, AudioClip> SkillSFXClipDic { get; private set; }
    public Dictionary<string, SkillObject> SkillObjectDic { get; private set; }
    public Dictionary<string, DropTableDataEntity> DropTableDic { get; private set; }
    public Dictionary<int, TutorialDataEntity> TutorialDataDic { get; private set; }
    public Dictionary<string, AbilityDataEntity> EnemyAbilityDataDic { get; private set; }
    public int StatInfoEnumCount { get; private set; }
    public int LanguageEnumCount { get; private set; }
    public int StatinfoForInfoElemEnumCount { get; private set; }

    public Dictionary<string, SoulEnchantDataEntity> SoulEnchantDataDic { get; private set; }
    public bool DataInit()
    {
        if(_isReady) return _isReady;
        Debug.Log("GameDataController Init");
        //CharacterDatas = Resources.Load<CharacterData>(Paths.CharacterData);
        EnemyDatas = Resources.Load<EnemyData>(Paths.EnemyData);

        //-----------------------------------------------------

        StatInfoEnumCount = Utils.EnumCount<eStatInfo>();
        LanguageEnumCount = Utils.EnumCount<eLanguage>();
        StatinfoForInfoElemEnumCount = Utils.EnumCount<eStatInfoForInfoElem>();

        LocalizationDatas = Resources.Load<LocalizationData>(Paths.LocalizationData);
        LocalizationDataDic = new Dictionary<string, string[]>();

        for(int i = 0; i< LocalizationDatas.All.Count;i++)
        {
            string keyword = LocalizationDatas.All[i].Keyword;
            string[] str = new string[LanguageEnumCount];
            str[(int)eLanguage.Eng] = LocalizationDatas.All[i].Eng;
            str[(int)eLanguage.Kor] = LocalizationDatas.All[i].Kor;
            LocalizationDataDic.Add(keyword, str);
        }

        #region mapData
        MapWayPointDatas = Resources.Load<MapWayPointData>(Paths.MapWayPointData);
        MapWayPointDataEntityDic = new Dictionary<string, List<MapWayPointDataEntity>>();

        var fieldinfo = MapWayPointDatas.GetType().GetFields();
        for(int i = 0; i< fieldinfo.Length;i++)
        {
            MapWayPointDataEntityDic.Add(fieldinfo[i].Name, fieldinfo[i].GetValue(MapWayPointDatas) as List<MapWayPointDataEntity>);
        }

        //Sprite[] mapsprite = Resources.LoadAll<Sprite>(Paths.MapSprite);
        MapSpriteDic = Resources.LoadAll<Sprite>(Paths.MapSprite).ToDictionary(keySelector: x => x.name, elementSelector: x => x);

        #endregion
        #region EnemyData
        EnemyDataDic = new Dictionary<string, List<EnemyDataEntity>>();
        //맵데이터 처럼 리플렉션으로 해도 되는데 분류방식 변경할지도 몰라서 일단 하드코딩


        EnemyDataDic.Add(Constants.Cave, EnemyDatas.Cave);
        EnemyDataDic.Add(Constants.Human, EnemyDatas.Human);
        EnemyDataDic.Add(Constants.Inanimate, EnemyDatas.Inanimate);
        EnemyDataDic.Add(Constants.Wild, EnemyDatas.Wild);
        EnemyDataDic.Add(Constants.Undead, EnemyDatas.Undead);//TODO 이거 datas로 옮겨서 하는것도?
        EnemyDataDic.Add(Constants.Boss, EnemyDatas.boss);

        EnemySpriteDic = new Dictionary<string, Dictionary<string, Sprite>>();

        EnemySpriteDic.Add(Constants.Cave, Resources.LoadAll<Sprite>(string.Concat(Paths.EnemySprite, Constants.Cave)).ToDictionary(keySelector: x => x.name.Split('_')[0], elementSelector: x => x));
        EnemySpriteDic.Add(Constants.Human, Resources.LoadAll<Sprite>(string.Concat(Paths.EnemySprite, Constants.Human)).ToDictionary(keySelector: x => x.name.Split('_')[0], elementSelector: x => x));
        EnemySpriteDic.Add(Constants.Inanimate, Resources.LoadAll<Sprite>(string.Concat(Paths.EnemySprite, Constants.Inanimate)).ToDictionary(keySelector: x => x.name.Split('_')[0], elementSelector: x => x));
        EnemySpriteDic.Add(Constants.Wild, Resources.LoadAll<Sprite>(string.Concat(Paths.EnemySprite, Constants.Wild)).ToDictionary(keySelector: x => x.name.Split('_')[0], elementSelector: x => x));
        EnemySpriteDic.Add(Constants.Undead, Resources.LoadAll<Sprite>(string.Concat(Paths.EnemySprite, Constants.Undead)).ToDictionary(keySelector: x => x.name.Split('_')[0], elementSelector: x => x));
        EnemySpriteDic.Add(Constants.Boss, Resources.LoadAll<Sprite>(string.Concat(Paths.EnemySprite, Constants.Boss)).ToDictionary(keySelector: x => x.name.Split('_')[0], elementSelector: x => x));

        #endregion
        //-----------------------------------------------------
        #region equipmentData
        WeaponDatas = Resources.Load<WeaponData>(Paths.WeaponData);
        ArmorDatas = Resources.Load<ArmorData>(Paths.ArmorData);
        AccessoryDatas = Resources.Load<AccessoryData>(Paths.AccessoryData);
        SkillStoneDatas = Resources.Load<SkillStoneData>(Paths.SkillStoneData);

        AllItemSpriteDic = new Dictionary<string, Sprite>();

        Sprite[] weaponsprite = Resources.LoadAll<Sprite>(Paths.WeaponSpriteData);
        foreach (var a in weaponsprite)
        {
            AllItemSpriteDic.Add(a.name, a);
        }

        Sprite[] armorsprite = Resources.LoadAll<Sprite>(Paths.ArmorSpriteData);
        foreach (var a in armorsprite)
        {
            AllItemSpriteDic.Add(a.name, a);
        }

        Sprite[] accessorysprite = Resources.LoadAll<Sprite>(Paths.AccessorySpriteData); //.Resources.LoadAll<Sprite>(Paths.AccessorySpriteData + "/Accessory");//
        foreach (var a in accessorysprite)
        {
            AllItemSpriteDic.Add(a.name, a);

        }

        Sprite[] skillstonesprite = Resources.LoadAll<Sprite>(Paths.SkillStoneSpriteData);
        foreach (var a in skillstonesprite)
        {
            AllItemSpriteDic.Add(a.name, a);
        }


        #endregion
        //-----------------------------------------------------
        #region skilldata
        SkillDatas = Resources.Load<SkillData>(Paths.SkillData);
        SkillDataDic = new Dictionary<string, SkillDataEntity>();
 
        for(int i = 0; i< SkillDatas.SkillDatas.Count;i++)
        {
            SkillDataDic.Add(SkillDatas.SkillDatas[i].Keyword, SkillDatas.SkillDatas[i]);
        }

        for (int i = 0; i< SkillStoneDatas.SkillStone.Count;i++)
        {
            if (SkillDataDic.TryGetValue(SkillStoneDatas.SkillStone[i].SkillKeyword, out SkillDataEntity value))
            {
                SkillStoneDatas.SkillStone[i].SkillDataEntity = value;
            }
            else
            {
                Debug.Log($"SkillDataDic에 {SkillStoneDatas.SkillStone[i].SkillKeyword}에 해당하는 값이 없음. {i}//");
            }
        }

        SkillContentDatas = Resources.Load<SkillContentData>(Paths.SkillContentData);
        SkillContentDataDic = new Dictionary<string, string[]>();
        for(int i = 0; i< SkillContentDatas.SkillContentDataEntity.Count;i++)
        {
            string[] content = new string[LanguageEnumCount];
            content[(int)eLanguage.Eng] = SkillContentDatas.SkillContentDataEntity[i].Eng;
            content[(int)eLanguage.Kor] = SkillContentDatas.SkillContentDataEntity[i].Kor;

            SkillContentDataDic.Add(SkillContentDatas.SkillContentDataEntity[i].Keyword, content);
        }

        SkillIconDic = Resources.LoadAll<Sprite>(Paths.SkillIcon).ToDictionary(keySelector: x => x.name, elementSelector: x => x);

        SkillObjectDic = Resources.LoadAll<SkillObject>(Paths.SkillObject).ToDictionary(keySelector: m => m.name, elementSelector: m => m);

        StateIconDic = Resources.LoadAll<Sprite>(Paths.StateIcon).ToDictionary(keySelector: x => x.name, elementSelector: x => x);

        #endregion
        //-----------------------------------------------------
        #region soundclip
        SFXClipDic = new Dictionary<eSFX, AudioClip>();
        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>(Paths.SFX);
        foreach(var a in sfxClips)
        {
            SFXClipDic.Add(Utils.ParseEnum<eSFX>(a.name), a);
        }
        SkillSFXClipDic = Resources.LoadAll<AudioClip>(Paths.SkillSFX).ToDictionary(x => x.name, x => x);
        #endregion
        //-----------------------------------------------------
        #region DropTable
        DropTableDic = Resources.Load<DropTableData>(Paths.DropTable).Table1.ToDictionary(keySelector: x => x.Keyword, elementSelector: x => x);

        #endregion
        #region Sprites

        SpriteDic = Resources.LoadAll<Sprite>(Paths.Sprite).ToDictionary(keySelector: x => x.name, elementSelector: x => x);
        #endregion
        //---------------------------------------------------------
        TutorialData tutodata = Resources.Load<TutorialData>(Paths.TutorialData);
        TutorialDataDic = new Dictionary<int, TutorialDataEntity>();
        foreach(var a in tutodata.Tutorial_0)
        {
            string key = string.Format("Tutorial_{0}_{1}", a.Type, a.SubIndex);
            a.Key = key;
            TutorialDataDic.Add(a.Index, a);
        }

        //================================================

        EnemyAbilityDataDic = Resources.Load<AbilityData>(Paths.Ability).EnemyAbility.ToDictionary(keySelector: x => x.Key, elementSelector: x => x);

        //================================================
        SoulEnchantDataDic = Resources.Load<SoulEnchantData>(Paths.SoulEnchantData).SoulEnchant.ToDictionary(keySelector: x => x.Key, elementSelector: x => x);

        //============end====================
        Resources.UnloadUnusedAssets();
        _isReady = true;
        return _isReady;
    }




    /// <summary>
    /// 해당 드롭테이블에 속한 드롭아이템 딕셔너리 반환. 아이템 이름은 10000 식의 index를 string값으로 지님
    /// </summary>
    /// <param name="dropTable"></param>
    /// <returns></returns>
    public Dictionary<string, float> GetDropItemDic(MapWayPointDataEntity waydata, float dropRate = 1)
    {
        DropTableDataEntity drops;
        if (InitController.Instance.GamePlays.IsCurrentWayPointCleared && InitController.Instance.GamePlays.CurrentMapWayData.WayPointType == WayPointType.Boss)
        {
            if (DropTableDic.TryGetValue((DropTableDic[waydata.DropTable].BaseTable), out drops) == false)
            {
                Debug.LogWarning("empty DropTable");
                return null;
            }
        }
        else
        {
            DropTableDic.TryGetValue(waydata.DropTable, out drops);
        }


        //if (DropTableDic.TryGetValue(waydata.DropTable, out DropTableDataEntity drops) == false)
        //{
        //    Debug.LogWarning("empty DropTable");
        //    return null;
        //}
        dropRate = Mathf.Max(1, dropRate); // 최소값이 1
        float dropbonus = InitController.Instance.GamePlays.IngameStat.GetTotalStat(eStatInfo.ItemFind) * dropRate;
        Debug.Log(dropbonus);
        Dictionary<string, float> dropitems = new Dictionary<string, float>();

        //float nodrop = drops.NoDrop;
        
        if (string.IsNullOrEmpty(drops.DropItem_Weapon) == false)
        {
            string[] weapon = drops.DropItem_Weapon.Split('/');
            int[] proc = Array.ConvertAll(drops.DropWeight_Weapon.Split('/'), int.Parse);
            if (weapon.Length != proc.Length) Debug.LogWarning("droptable data nor proc doesn't exist");
            for (int i = 0; i < weapon.Length; i++)
            {
                dropitems.Add(weapon[i], proc[i] * dropbonus);
                //nodrop += proc[i];
            }
        }

        if (string.IsNullOrEmpty(drops.DropItem_Armor) == false)
        {
            string[] armor = drops.DropItem_Armor.Split('/');
            int[] proc = Array.ConvertAll(drops.DropWeight_Armor.Split('/'), int.Parse);
            if (armor.Length != proc.Length) Debug.LogWarning("droptable data nor proc doesn't exist");
            for (int i = 0; i < armor.Length; i++)
            {
                dropitems.Add(armor[i], proc[i] * dropbonus);
                //nodrop += proc[i];
            }
        }


        if (string.IsNullOrEmpty(drops.DropItem_Accessory) == false)
        {
            string[] accessory = drops.DropItem_Accessory.Split('/');
            int[] proc = Array.ConvertAll(drops.DropWeight_Accessory.Split('/'), int.Parse);
            if (accessory.Length != proc.Length) Debug.LogWarning("droptable data nor proc doesn't exist");
            for (int i = 0; i < accessory.Length; i++)
            {
                dropitems.Add(accessory[i], proc[i] * dropbonus);
                //nodrop += proc[i];
            }
        }

        if (string.IsNullOrEmpty(drops.DropItem_SkillStone) == false)
        {
            string[] skillstone = drops.DropItem_SkillStone.Split('/');

            int[] proc = Array.ConvertAll(drops.DropWeight_SkillStone.Split('/'), int.Parse);
            if (skillstone.Length != proc.Length) Debug.LogWarning("droptable data nor proc doesn't exist");
            for (int i = 0; i < skillstone.Length; i++)
            {
                dropitems.Add(skillstone[i], proc[i] * dropbonus);
                //nodrop += proc[i];
            }
        }


        return dropitems;
    }
    /// <summary>
    /// string : itemIndex / bool : isItemAquired
    /// </summary>
    /// <param name="droptable"></param>
    /// <returns></returns>
    public Dictionary<string, bool> IsDropTableItemsIsAquired_itemIndex(MapWayPointDataEntity waydata)
    {
        Dictionary<string, float> drops = GetDropItemDic(waydata);//key : itemindex, value : droprate


        Dictionary<string, bool> indexKeywords = new Dictionary<string, bool>(); //key : itemIndex, value : isItemAquired

        foreach (var a in drops)
        {
            int itemindex = int.Parse(a.Key) / 10000;
            if (itemindex == 1) //weapon
            {
                foreach (var b in WeaponDatas.Weapon)
                {
                    if (b.Index.ToString() == a.Key)
                    {
                        bool isAquired = InitController.Instance.SaveDatas.UserData.IsWeaponAcuiredDic[b.Keyword].IsAcquired;
                        indexKeywords.Add(a.Key, isAquired);
                        break;
                    }
                }
            }
            else if(itemindex == 2) //armor
            {
                foreach (var b in ArmorDatas.Armor)
                {
                    if (b.Index.ToString() == a.Key)
                    {
                        bool isAquired = InitController.Instance.SaveDatas.UserData.IsArmorAcquiredDic[b.Keyword].IsAcquired;
                        indexKeywords.Add(a.Key, isAquired);
                        break;
                    }
                }
            }
            else if(itemindex == 3) // acce
            {
                foreach (var b in AccessoryDatas.Accessory)
                {
                    if (b.Index.ToString() == a.Key)
                    {
                        bool isAquired = InitController.Instance.SaveDatas.UserData.IsAccessoryAcquiredDic[b.Keyword].IsAcquired;
                        indexKeywords.Add(a.Key, isAquired);
                        break;
                    }
                }
            }
            else if(itemindex == 4) // skill
            {
                foreach (var b in SkillStoneDatas.SkillStone)
                {
                    if (b.Index.ToString() == a.Key)
                    {
                        bool isAquired = InitController.Instance.SaveDatas.UserData.IsSkillStoneAcquiredDic[b.Keyword].IsAcquired;
                        indexKeywords.Add(a.Key, isAquired);
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("wrong keyword");
            }



        }

        return indexKeywords;
    }
}
