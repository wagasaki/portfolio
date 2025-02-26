using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths 
{
    public const string CharacterData = "RPG/CharacterData/CharacterData";
    public const string EnemyData = "RPG/EnemyInfo/EnemyData/EnemyData";
    public const string EnemySprite = "RPG/EnemyData/EnemySprite/";

    private const string Controllers = Prefab + "Controllers/";
    public const string InitController = Controllers + "InitController";
    public const string GamePlayController = Controllers + "GamePlayController";
    public const string SceneController = Controllers + "SceneController";
    public const string UIController = Controllers + "UIController";
    public const string LobbyUIController = Controllers + "LobbyUIController";
    public const string SoundContoller = Controllers + "SoundController";
    public const string SkillController = Controllers + "SkillController";
    public const string EnemySkillController = Controllers + "EnemySkillController";
    public const string LocaleController = Controllers + "LocaleController";
    public const string OverlayTextEffectController = Controllers + "OverlayTextEffectController";
    public const string FireBaseBinder = Controllers + "FireBaseBinder";

    private const string MapData = "RPG/MapData/";
    public const string MapWayPointData = MapData + "MapWayPointData";
    public const string BattleMap = MapData + "BattleMap/";
    public const string MapSprite = MapData + "MapSprite";
    public const string BattleMapText = MapData + "BattleMapName/BattleMapName";

    private const string EquipmentData = "RPG/EquipmentData/";
    public const string WeaponData = EquipmentData + "WeaponData/WeaponData";
    public const string ArmorData = EquipmentData + "ArmorData/ArmorData";
    public const string AccessoryData = EquipmentData + "AccessoryData/AccessoryData";
    public const string SkillStoneData = EquipmentData + "SkillStoneData/SkillStoneData";
    public const string WeaponSpriteData = EquipmentData + "WeaponSpriteData";
    public const string ArmorSpriteData = EquipmentData + "ArmorSpriteData";
    public const string AccessorySpriteData = EquipmentData + "AccessorySpriteData";
    public const string SkillStoneSpriteData = EquipmentData + "SkillStoneSpriteData";
    public const string EmptySprite = EquipmentData + "EmptySprite";


    public const string NameData = "RPG/LocalizationData/NameData";
    public const string LocalizationData = "RPG/LocalizationData/LocalizationData";

    private const string Prefab = "RPG/Prefab/";
    public const string UserCharacter = Prefab + "Character/UserCharacter";
    public const string LobbyCharacter = Prefab + "Character/LobbyCharacter";
    public const string Enemy = Prefab + "Enemy/Enemy";
    public const string MapIcon = Prefab + "MapIconButton";

    private const string UIPrefab = Prefab + "UI/";
    public const string BattleUICanvas = UIPrefab + "BattleUICanvas";
    public const string MapUICanvas = UIPrefab + "MapUICanvas";
    public const string UpperUICanvas = UIPrefab + "UpperUICanvas";
    public const string LoadingPanelCanvas = UIPrefab + "LoadingPanelCanvas";
    public const string LobbyUICanvas = UIPrefab + "LobbyUICanvas";


    public const string Popup = UIPrefab + "Popup/";
    public const string WeaponUIPopup = Popup + "WeaponUIPopup";

    //-------------------------------------------------------------
    private const string SubElem = UIPrefab + "SubElem/";
    public const string WeaponUIElem = SubElem + "WeaponUIElem";
    public const string ArmorUIElem = SubElem + "ArmorUIElem";
    public const string AccessoryUIElem = SubElem + "AccessoryUIElem";
    public const string SkillStoneUIElem = SubElem + "SkillStoneUIElem";
    public const string RewardPrefab = SubElem + "RewardPrefab";
    public const string RewardTextElem = SubElem + "RewardTextElem";
    public const string StateInfo = SubElem + "StateInfo";
    public const string AbilitySelectElem = SubElem + "AbilitySelectElem";
    public const string DropTableDisplayElem = SubElem + "DropTableDisplayElem";

    private const string EquipChangePanel = SubElem + "EquipChangePanel/";
    public const string EquipChangeElem = EquipChangePanel + "EquipChangeElem";

    private const string ShopEquipPanel = SubElem + "ShopEquipPanel/";
    public const string ShopEquipElem = ShopEquipPanel + "ShopEquipElem";
    //------------------------------------------------------------

    private const string ObjectPool = Prefab + "ObjectPool/";
    public const string OneTimeAnimationPool = ObjectPool + "OneTimeAnimationPool";
    public const string TextEffectPool = ObjectPool + "TextEffectPool";
    public const string DamageTextEffectPool = ObjectPool + "DamageTextEffectPool";

    private const string Effect = Prefab + "Effect/";
    private const string TextEffects = Effect + "TextEffect/";
    public const string DmgTextEffect = TextEffects + "DmgTextEffect";
    public const string TextEffect = TextEffects + "TextEffect";
    public const string SkillNameDisplayEffect = TextEffects + "SkillNameDisplayEffect";


    private const string Skill = "RPG/SkillData/";
    public const string SkillData = Skill + "SkillData";
    public const string SkillContentData = Skill + "SkillContentData";
    public const string SkillIcon = "RPG/SkillIcon";
    public const string SkillObject = Prefab + "Skill/SkillObject/";

    private const string Debuff = "RPG/Debuff/";
    public const string DebuffSO = Debuff + "SO/";
    public const string StateIcon = Debuff + "DebuffSprite";
    public const string Poison = DebuffSO + "PoisonDebuff";
    public const string Burn = DebuffSO + "BurnDebuff";
    public const string FrostBite = DebuffSO + "FrostBiteDebuff";
    public const string ElectricShock = DebuffSO + "ElectricShockDebuff";
    public const string Bleeding = DebuffSO + "BleedingDebuff";

    public const string Sprite = "RPG/Sprite/";

    private const string Sound = "RPG/Sound/";
    public const string MainAudioMixer = Sound + "MainAudioMixer";
    public const string BGM = Sound + "BGM";
    public const string SFX = Sound + "SFX";
    public const string SkillSFX = Sound + "SkillSFX";

    public const string DropTable = "RPG/DropTable/DropTableData";


    public const string TutorialData = "RPG/TutorialData/TutorialData";

    public const string SoulEnchantData = "RPG/SoulEnchantData/SoulEnchantData";

    public const string Ability = "RPG/Ability/AbilityData";
}
