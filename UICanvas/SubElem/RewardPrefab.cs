using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class RewardPrefab : UIBase
{

    private enum Texts
    {
        RewardText,
    }
    private enum Images
    {
        EffectImage,
        RewardImage,
        FrameImage,
        CoverImage,
        ItemTypeImage
    }
    private enum GameObjects
    {
        ItemType,
    }
    private Image _rewardImage;
    private TextMeshProUGUI _rewardText;
    private CanvasGroup _canvasGroup;
    //private Material _coverMat;
    private GameObject _itemTypeObj;
    private float _alpha;
    public float Alpha { get { return _alpha; } set { _alpha = value; _canvasGroup.alpha = _alpha; } }
    private void Awake()
    {
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindObject(typeof(GameObjects));
        _rewardText = GetText((int)Texts.RewardText);
        _rewardImage = GetImage((int)Images.RewardImage);
        _canvasGroup = GetComponent<CanvasGroup>();
        //_coverMat = GetImage((int)Images.CoverImage).material;
        _itemTypeObj = GetObject((int)GameObjects.ItemType);
    }

    public void InitGoldElem(Sprite itemsprite, int value)
    {
        StartCoroutine(InitElem());

        _rewardText.text = value.ToString();
        GetImage((int)Images.FrameImage).gameObject.SetActive(false);
        GetImage((int)Images.CoverImage).gameObject.SetActive(false);
        GetImage((int)Images.EffectImage).gameObject.SetActive(false);
        _itemTypeObj.SetActive(false);

        _rewardImage.sprite = itemsprite;
    }
    public void InitExpElem(Sprite itemsprite, float value)
    {
        StartCoroutine(InitElem());


        _rewardText.text = value.ToString();
        GetImage((int)Images.FrameImage).gameObject.SetActive(false);
        GetImage((int)Images.CoverImage).gameObject.SetActive(false);
        GetImage((int)Images.EffectImage).gameObject.SetActive(false);
        _itemTypeObj.SetActive(false);

        _rewardImage.sprite = itemsprite;
    }
    public void InitStaminaElem(Sprite itemsprite, int value)
    {
        StartCoroutine(InitElem());


        _rewardText.text = value.ToString();
        GetImage((int)Images.FrameImage).gameObject.SetActive(false);
        GetImage((int)Images.CoverImage).gameObject.SetActive(false);
        GetImage((int)Images.EffectImage).gameObject.SetActive(false);
        _itemTypeObj.SetActive(false);

        _rewardImage.sprite = itemsprite;
    }
    public void InitIemElem(eReward rewardType, Sprite itemsprite, string keyword)
    {
        StartCoroutine(InitElem());

        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(keyword, out string[] names);
        _rewardText.GetComponent<LocalizeStringEvent>().StringReference.Arguments = names;
        _rewardText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Default");
        GetImage((int)Images.FrameImage).gameObject.SetActive(true);
        GetImage((int)Images.FrameImage).sprite = InitController.Instance.GameDatas.SpriteDic["FrameGlowSprite"];
        GetImage((int)Images.CoverImage).gameObject.SetActive(true);
        GetImage((int)Images.EffectImage).gameObject.SetActive(true);
        _itemTypeObj.SetActive(true);
        if (rewardType == eReward.Weapon)
        {
            GetImage((int)Images.ItemTypeImage).sprite = InitController.Instance.GameDatas.SpriteDic["WeaponIcon"];
        }
        else if (rewardType == eReward.Armor)
        {
            GetImage((int)Images.ItemTypeImage).sprite = InitController.Instance.GameDatas.SpriteDic["ArmorIcon"];
        }
        else if (rewardType == eReward.Accessory)
        {
            GetImage((int)Images.ItemTypeImage).sprite = InitController.Instance.GameDatas.SpriteDic["AccessoryIcon"]; 
        }
        else
        {
            GetImage((int)Images.ItemTypeImage).sprite = InitController.Instance.GameDatas.SpriteDic["SkillStoneIcon"];
        }
        StartCoroutine(ItemCoverFade());

        _rewardImage.sprite = itemsprite;
    }
    IEnumerator ItemCoverFade()
    {
        Image effectimage = GetImage((int)Images.EffectImage);
        Color color = new Color(1, 1, 0.5f, 0);
        float proc = 0;
        while(proc<1)
        {
            //_coverMat.SetFloat("_FadeAmount", proc);
            color.a = proc;
            effectimage.color = color;
            effectimage.transform.localScale = Vector3.one * proc;
            effectimage.transform.Rotate(0,0,Time.deltaTime*10);
            proc += Time.deltaTime;
            yield return null;
        }
        //_coverMat.SetFloat("_FadeAmount", 1);
        while(true)
        {
            effectimage.transform.Rotate(0, 0, Time.deltaTime * 10);
            yield return null;
        }
    }
    IEnumerator InitElem()
    {
        Alpha = 0;
        while (Alpha < 1)
        {
            Alpha += Time.deltaTime * 2;
            yield return null;
        }
    }
}
