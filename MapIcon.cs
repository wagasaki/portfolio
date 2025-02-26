using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;



public enum WayPointType
{
    None = -1,
    Normal,
    Boss,
    WayPoint,
}
[Serializable]
public class MapEffect
{
    public eMapEffect EffectType;
    public float EffectValue;
    public int Duration;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isRandom">true면 랜덤. false면 초기화. type == none</param>
    public MapEffect(bool isRandom)
    {
        if(isRandom == true) //랜덤
        {
            int lv;
            int duration;

            float maxValue = 10f;
            float randValue = UnityEngine.Random.Range(0, maxValue);
            if(randValue<3f) //30%
            {
                lv = 2;
            }
            else //70%
            {
                lv = 1;
            }


            float randDuration = UnityEngine.Random.Range(0, 10f);
            if (randDuration < 2) //20%
            {
                duration = 3;
            }
            else if (randDuration < 5) //30%
            {
                duration = 2;
            }
            else//50%
            {
                duration = 1;
            }
            //Debug.Log(randDuration + "----------------------------------" + duration);
            Duration = duration;

            float rand = UnityEngine.Random.Range(0, 4f);
            if(rand<1)
            {
                EffectType = eMapEffect.MoneyIncrease; // x2, 3
                EffectValue = 2 + 1 * (lv-1);
            }
            else if(rand < 2)
            {
                EffectType = eMapEffect.EXPIncrease; // x2, 3
                EffectValue = 2 + 1 * (lv - 1);
            }
            else if(rand < 3)
            {
                EffectType = eMapEffect.DropIncrease; // x2, 3
                EffectValue = 2 + 1 * (lv - 1);
            }
            else
            {
                EffectType = eMapEffect.HPDecrease;// 0.5
                EffectValue = 0.5f;
            }

        }
        else //초기화
        {
            EffectType = eMapEffect.None;
            EffectValue = 0;
            Duration = 0;
        }
    }
}
public class MapIcon : UIBase, IPointerClickHandler
{
    enum Buttons
    {
        MoveButton,
        DoActionButton,
    }
    enum Texts
    {
        GradeInfoText,
        LvInfoText,
        DoActionText,
        DoActionStaminaText,
        MapEffectText,
    }
    enum GameObjects
    {
        DoActionStamina,
        MapEffectElem
    }
    enum Images
    {
        WayImage,
        SpotInfo
    }
    [SerializeField]
    private GameObject _infos, _buttons;
    private GameObject _doActionStamina;
    [SerializeField]
    private Sprite _normalSprite, _bossSprite, _waypointSprite;
    [SerializeField]
    private Image _glowImage;
    private Image _wayImage;
    private Button _doActionButton;
    private Button _moveBtn;
    private TextMeshProUGUI _doActionText;
    private Color _bossColor = new Color(0.5f, 0, 0, 0.8f);
    private Color _normalColor = new Color(0, 0.5f, 0, 0.8f);
    private Color _wayColor = new Color(0, 0.35f, 0.5f, 0.8f);
    private MapUICanvas _mapUICanvae;
    private MapEffectElem _mapEffectElem;
    private MapEffect _mapEffect;
    public MapEffect CurrentMapEffect 
    { 
        get 
        {
            return _mapEffect; 
        } 
        set 
        {
            _mapEffect = value;

            if(value !=null)
            {
                _mapEffectElem.gameObject.SetActive(true);
                _mapEffectElem.InitElem(_mapEffect);
            }
            else
            {
                _mapEffectElem.gameObject.SetActive(false);
            }
        } 
    }

    private void Awake()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));

        _wayImage = GetImage((int)Images.WayImage);
        _doActionButton = GetButton((int)Buttons.DoActionButton);
        _moveBtn = GetButton((int)Buttons.MoveButton);
        _doActionText = GetText((int)Texts.DoActionText);
        _doActionStamina = GetObject((int)GameObjects.DoActionStamina);
        _mapEffectElem = GetObject((int)GameObjects.MapEffectElem).GetComponent<MapEffectElem>();
        _mapEffectElem.gameObject.SetActive(false);
        _mapEffect = null;
    }
    public void InitMapIcon(MapWayPointDataEntity dataEntity, Action<int> onMoveButton, MapUICanvas canvas)
    {
        _buttons.SetActive(false);
        _infos.SetActive(true);
        _mapUICanvae = canvas;

        GetText((int)Texts.MapEffectText).gameObject.SetActive(false);
        TextMeshProUGUI gradeText = GetText((int)Texts.GradeInfoText);
        if (dataEntity.WayPointType == WayPointType.WayPoint)
        {
            _wayImage.sprite = _waypointSprite;
            _doActionStamina.SetActive(false);
            InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(dataEntity.WayPointTo, out string[] names);
            GetText((int)Texts.LvInfoText).GetComponent<LocalizeStringEvent>().StringReference.Arguments = names;
            GetText((int)Texts.LvInfoText).GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
            _doActionText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("UpperUI", "MoveMap");
            _doActionButton.onClick.AddListener(() =>
            {
                InitController.Instance.GamePlays.MoveToOtherMap(dataEntity);
            });
            gradeText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("UpperUI", "WayPoint");
            GetImage((int)Images.SpotInfo).color = _wayColor;
        }
        else
        {
            string key = InitController.Instance.GameDatas.EnemyDataDic[dataEntity.EnemyType][dataEntity.EnemyIndex].Keyword;
            _wayImage.sprite = InitController.Instance.GameDatas.EnemySpriteDic[dataEntity.EnemyType][key];
            _wayImage.SetNativeSize();

            if (dataEntity.WayPointType == WayPointType.Boss)
            {
                gradeText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("UpperUI", "Boss");
                GetImage((int)Images.SpotInfo).color = _bossColor;
            }
            else if(dataEntity.WayPointType == WayPointType.Normal)
            {
                //gradeText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("UpperUI", "Normal");
                //gradeText.color = _normalColor;
                gradeText.gameObject.SetActive(false);
                GetImage((int)Images.SpotInfo).color = _normalColor;
            }
            _doActionStamina.SetActive(true);
            GetText((int)Texts.LvInfoText).text = "Lv " + dataEntity.WayPointLevel.ToString();
            _doActionText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("UpperUI", "Battle");
            GetText((int)Texts.DoActionStaminaText).text = "-" + Nums.BaseBattleStaminaCost;
            _doActionButton.onClick.AddListener(() =>
            {
                InitController.Instance.GamePlays.MoveToBattleMap(dataEntity, canvas.CurrentMapEffect);
            });
        }
        _moveBtn.onClick.AddListener(() =>
        {
            onMoveButton.Invoke(dataEntity.Index);
            _buttons.SetActive(false);
            _infos.SetActive(true);
        });

        if(_mapUICanvae.CurrentWayIndex == dataEntity.Index)
        {
            ActiveWayPoint(true);
        }
        else
        {
            ActiveWayPoint(false);
        }

        //if(dataEntity.Index == 3 || dataEntity.Index == 8)
        //{
        //    GetText((int)Texts.MapEffectText).gameObject.SetActive(true);
        //    GetText((int)Texts.MapEffectText).text = "Enemy HP*0.5(4)";
        //}
        //if(UnityEngine.Random.Range(0,10)<2)
        //{
        //    _mapEffectElem.gameObject.SetActive(true);
        //    _mapEffectElem.InitElem("Enemy HP * 0.5 (3)");
        //}
    }

    public void ActiveWayPoint(bool selected)
    {
        if (selected == true)
        {
            _moveBtn.gameObject.SetActive(false);
            _doActionButton.gameObject.SetActive(true);
        }
        else
        {
            _moveBtn.gameObject.SetActive(true);
            _doActionButton.gameObject.SetActive(false);
        }
    }
    public void ActiveButton()
    {
        _buttons.SetActive(!_buttons.activeSelf);
        _infos.SetActive(!_buttons.activeSelf);//HACK 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool isOn = _buttons.activeSelf;
        InitController.Instance.GamePlays.OnMapIconClick?.Invoke(this);
        InitController.Instance.GamePlays.OnMapIconClick = CheckMapIcon;
        _buttons.SetActive(!isOn);
        _infos.SetActive(isOn);
    }
    public void CheckMapIcon(MapIcon icon)
    {
        if(icon != null && icon == GetComponent<MapIcon>())
        {
            _buttons.gameObject.SetActive(!_buttons.activeSelf);
            _infos.SetActive(!_buttons.activeSelf);
        }
        else
        {
            _buttons.gameObject.SetActive(false);
            _infos.SetActive(true);
        }
    }
}
