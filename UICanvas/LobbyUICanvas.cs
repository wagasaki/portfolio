using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using BuildInfoUtility;
public class LobbyUICanvas : UICanvas
{
    private Material _titleMat;
    private TextMeshProUGUI _soulText;
    private Action<int> OnSoulUse { get; set; }
    enum Buttons
    {
        NewGameButton,
        ContinueButton,
        SoulEnchantButton,
        SettingButton
    }
    enum GameObjects
    {
        TitleImage,
        Buttons,
    }
    enum Texts
    {
        ContinueConditionText,
        SoulAmountText,
        VersionText,
    }
    private void OnDisable()
    {
        if (InitController.Instance != null)
            InitController.Instance.SaveDatas.OnSoulChanged -= OnSoulUse;
    }
    private void Awake()
    {
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        _titleMat = GetObject((int)GameObjects.TitleImage).GetComponent<Image>().material;
        _soulText = GetText((int)Texts.SoulAmountText);
        GetObject((int)GameObjects.Buttons).gameObject.SetActive(false);
        GetText((int)Texts.VersionText).text = string.Format("Version {0}.{1}", Application.version, BuidRuntimeInfo.Instance.BuildId.ToString());
    }
    public override void InitUICanvas()
    {
        base.InitUICanvas();

        UIController uis = InitController.Instance.UIs;
        TextMeshProUGUI continueconditiontext = GetText((int)Texts.ContinueConditionText);
        ActiveButtons();

        GetButton((int)Buttons.NewGameButton).onClick.AddListener(() =>
        {
            if(InitController.Instance.SaveDatas.UserData.IngameStatData != null)
            {
                //Debug.Log("진행중인 게임이 존재합니다.");
                Action startgame =()=> InitController.Instance.Scenes.ChangeScene(Constants.Main, true);
                StartNewGameUIPopup popup = uis.OpenUIPopup<StartNewGameUIPopup>(null, uis.transform);
                popup.InitUIPopup();
                popup.SetButtonAction(startgame);
                InitController.Instance.Sounds.PlaySFX(eSFX.Click_Failed);
            }
            else
            {
                //Debug.Log("새 게임 시작");
                InitController.Instance.Sounds.PlaySFX(eSFX.Click_Enterance);
                InitController.Instance.Scenes.ChangeScene(Constants.Main, true);
            }
        });

        GetButton((int)Buttons.ContinueButton).onClick.AddListener(() =>
        {
            if(InitController.Instance.SaveDatas.UserData.IngameStatData == null)
            {
                NoSave();
                InitController.Instance.Sounds.PlaySFX(eSFX.Click_Failed);
                return;
            }
            else
            {
                if(InitController.Instance.SaveDatas.UserData.IngameStatData.GetTotalStat(eStatInfo.Stamina) <= 0)
                {
                    NoSave();
                    InitController.Instance.Sounds.PlaySFX(eSFX.Click_Failed);
                    return;
                }
                InitController.Instance.Sounds.PlaySFX(eSFX.Click_Enterance);
                InitController.Instance.Scenes.ChangeScene(Constants.Main, false);
            }
            void NoSave()
            {
                if (InitController.Instance.Locales.CurrentLocaleID == eLanguage.Kor)
                {
                    InitController.Instance.OverlayText.TextEffect("저장된 게임이 없습니다");
                }
                else
                {
                    InitController.Instance.OverlayText.TextEffect("There is no saved game");
                }
            }
        });
        GetButton((int)Buttons.SoulEnchantButton).onClick.AddListener(() =>
        {
            InitController.Instance.Sounds.PlaySFX(eSFX.Click);
            SoulEnchantUIPopup popup = uis.OpenUIPopup<SoulEnchantUIPopup>(null, uis.transform);
            popup.InitUIPopup();
        });
        GetButton((int)Buttons.SettingButton).onClick.AddListener(() =>
        {
            InitController.Instance.Sounds.PlaySFX(eSFX.Click);
            SettingUIPopup popup = uis.OpenUIPopup<SettingUIPopup>("SettingUIPopup", uis.transform);
            popup.InitUIPopup();
            popup.OnDisableByLobbyCanvas = () =>
            {
                 ShowContinueCondition();
            };

        });

        ShowSoulAmount(InitController.Instance.SaveDatas.UserData.CurrentSoul);
        ShowContinueCondition();
        OnSoulUse = (amount)=> ShowSoulAmount(amount);
        InitController.Instance.SaveDatas.OnSoulChanged -= OnSoulUse;
        InitController.Instance.SaveDatas.OnSoulChanged += OnSoulUse;

        void ShowContinueCondition()
        {
            IngameStatData currentdata = InitController.Instance.SaveDatas.UserData.IngameStatData;
            if (currentdata == null || currentdata.GetTotalStat(eStatInfo.Stamina) <= 0)
            {
                LocalizeStringEvent ev = continueconditiontext.GetComponent<LocalizeStringEvent>();
                ev.StringReference.SetReference("Equip", "Menu_Empty");
            }
            else
            {
                string[] maporigin = InitController.Instance.GameDatas.LocalizationDataDic[currentdata.CurrentMapName];
                string[] mapname = new string[maporigin.Length];

                for (int i = 0; i < mapname.Length; i++)
                {
                    mapname[i] = string.Format(
                        "[<color=#00FFFF>Lv{0}</color>/<color=#FFD700>{1}Gold</color>/<color=white>{2} {3}</color>]", 
                        currentdata.CurrentLevel.ToString("N0"), 
                        currentdata.CurrentGold.ToString("N0"), 
                        maporigin[i], 
                        currentdata.CurrentMapWayIndex.ToString("N0"));
                }
                LocalizeStringEvent ev = continueconditiontext.GetComponent<LocalizeStringEvent>();
                ev.StringReference.Arguments = mapname;
                ev.StringReference.SetReference(Constants.Equip, Constants.Default);
            }
        }
    }
    public void ActiveButtons()
    {
        GetObject((int)GameObjects.Buttons).gameObject.SetActive(true);
    }
    void ShowSoulAmount(int amount)
    {
        Debug.Log("소울" + amount);
        _soulText.text = amount.ToString("N0");
        _soulText.rectTransform.sizeDelta = new Vector2(_soulText.preferredWidth, _soulText.rectTransform.rect.height);
    }
    float proc = 0;
    string _titleShineLocation = "_ShineLocation";
    private void Update()
    {
        _titleMat.SetFloat(_titleShineLocation, proc);
        proc += Time.deltaTime*0.3f;
        if (proc >= 1.6f) proc = 0;
    }
}
