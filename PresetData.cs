using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PresetData
{
    public bool IsPresetDataOpened { get; set; }

    public WeaponDataEntity _currentWeapon;
    public ArmorDataEntity _currentArmor;
    public AccessoryDataEntity[] _currentAccessory = new AccessoryDataEntity[Nums.AccessoryCount];
    public SkillStoneDataEntity[] _currentSkillStone = new SkillStoneDataEntity[Nums.SkillStoneCount];
}
