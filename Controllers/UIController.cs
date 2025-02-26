using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using Random = UnityEngine.Random;


public class UIController : UIControllerBase
{
    private bool _isReady = false;
    #region MainScene
    private BattleUICanvas _battleUICanvasPrefab;
    private MapUICanvas _mapUICanvasPrefab;

    #endregion

    #region LobbyScene
    private LobbyUICanvas _lobbyUICanvas;

    #endregion


    private Action _onMainSceneUpdate;

    public BattleUICanvas GetBattlelUICanvas { get; private set; }
    public MapUICanvas GetMapUICanvas { get; private set; }



    private RectTransform _battleMapNameTrans;
    private TextMeshProUGUI _battleMapText;

    public bool InitUIController()
    {
        if (_isReady) return _isReady;

        GetComponent<Canvas>().worldCamera = Camera.main;
        _uiPopupStack = new Stack<UIPopup>();
        IsESC_Usable = true;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == Constants.Main)
        {
            InitOnMainScene();
        }
        else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == Constants.Lobby)
        {
            InitOnLobbyScene();
        }

        _isReady = true;

        return _isReady;
    }
    private void Update()
    {
        _onMainSceneUpdate?.Invoke();

        if (Input.GetKeyUp(KeyCode.Escape)&& IsESC_Usable)
        {
            if(_uiPopupStack.Count>0)
            {
                if (_uiPopupStack.Peek().IsEscapekeyCanClosePopup == false) return; // gameover, ability, reward, tutorial

                CloseUIPopup();
            }
            else
            {
                OpenUIPopup<EscapeUIPopup>(null, transform).InitUIPopup();
            }
        }
#if UNITY_EDITOR

        if(Input.GetKeyDown(KeyCode.Space))
        {
            ScreenCapture.CaptureScreenshot("Cap" + Time.realtimeSinceStartup + ".png");
        }
#endif
    }

    public void InitOnMainScene()
    {
        if (true)
        {
            Debug.Log("MainScene");
            _permenantUICanvasList = null;
            _permenantUICanvasList = new List<UICanvas>();
            _battleUICanvasPrefab = Resources.Load<BattleUICanvas>(Paths.BattleUICanvas);
            _mapUICanvasPrefab = Resources.Load<MapUICanvas>(Paths.MapUICanvas);

            GetBattlelUICanvas = Instantiate(_battleUICanvasPrefab);
            GetMapUICanvas = Instantiate(_mapUICanvasPrefab);

            _permenantUICanvasList.Add(GetBattlelUICanvas); //TODO  Enum순으로 넣는 방법 생각좀
            _permenantUICanvasList.Add(GetMapUICanvas);

            foreach (var a in _permenantUICanvasList) a.InitUICanvas();

            (_permenantUICanvasList[(int)PermanantUI.MapUI] as MapUICanvas).InitMapData(InitController.Instance.GamePlays.IngameStat.CurrentMapName, InitController.Instance.GamePlays.IngameStat.CurrentMapWayIndex);
            _permenantUICanvasList[(int)PermanantUI.BattleUI].gameObject.SetActive(false);
            _loadingPanelCanvasPrefab = Resources.Load<LoadingPanelCanvas>(Paths.LoadingPanelCanvas);
            if (_loadingPanelCanvas == null)
                _loadingPanelCanvas = Instantiate(_loadingPanelCanvasPrefab);
            _loadingPanelCanvas.InitUICanvas();


            _battleMapNameTrans = Instantiate(Resources.Load<GameObject>(Paths.BattleMapText), this.transform).GetComponent<RectTransform>();
            _battleMapNameTrans.anchoredPosition = Vector2.right * 2500;
            _battleMapText = _battleMapNameTrans.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            _battleMapText.gameObject.GetComponent<LocalizeStringEvent>().StringReference.SetReference("Equip", "Blank");
        }
    }
    private Coroutine _battleMapMoveRout;
    public void MapNameTextEffect(string keyword)
    {
        InitController.Instance.GameDatas.LocalizationDataDic.TryGetValue(keyword, out string[] value);
        LocalizeStringEvent ev = _battleMapText.gameObject.GetComponent<LocalizeStringEvent>();
        ev.StringReference.Arguments = value;
        ev.StringReference.SetReference("Equip", "Default");
        //Debug.Log($"==========================================={value[0]}");

        if(_battleMapMoveRout != null)
        {
            StopCoroutine(_battleMapMoveRout);
            _battleMapMoveRout = null;

        } 
        _battleMapMoveRout = StartCoroutine(MapNameMoveRout());
    }
    IEnumerator MapNameMoveRout()
    {
        _battleMapNameTrans.gameObject.SetActive(true);
        _battleMapNameTrans.anchoredPosition = Vector2.right * 2500;
        float rate = 0;
        while(_battleMapNameTrans.anchoredPosition.x > 0)
        {
            rate += Time.deltaTime;
            _battleMapNameTrans.anchoredPosition = Vector2.Lerp(_battleMapNameTrans.anchoredPosition, Vector2.zero, rate);
            if (_battleMapNameTrans.anchoredPosition.x < 3f)
                _battleMapNameTrans.anchoredPosition = Vector2.zero;
            yield return null;
        }
        _battleMapNameTrans.anchoredPosition = Vector2.zero;

        yield return YieldCache.WaitForSeconds(1f);

        rate = 0;
        while (_battleMapNameTrans.anchoredPosition.x > -2500)
        {
            rate += Time.deltaTime;
            _battleMapNameTrans.anchoredPosition = Vector2.Lerp(_battleMapNameTrans.anchoredPosition, Vector2.left * 2500, rate);
            if (_battleMapNameTrans.anchoredPosition.x < -2497)
                _battleMapNameTrans.anchoredPosition = Vector2.left * 2500;
            yield return null;
        }
        _battleMapNameTrans.gameObject.SetActive(false);
    }


    public void InitOnLobbyScene()
    {
        _permenantUICanvasList = null;
        _permenantUICanvasList = new List<UICanvas>();

        _lobbyUICanvas = FindObjectOfType(typeof(LobbyUICanvas), true) as LobbyUICanvas;
        if(_lobbyUICanvas == null)
        {
            _lobbyUICanvas = Instantiate(Resources.Load<LobbyUICanvas>(Paths.LobbyUICanvas)); //TODO 이거 uicontroller랑 좀 방식 다르게 해놨는데 나중에 마무리하면서 다 일괄정리
        }
        _permenantUICanvasList.Add(_lobbyUICanvas);


        foreach (var a in _permenantUICanvasList) a.InitUICanvas();

        Instantiate(Resources.Load<GameObject>(Paths.LobbyCharacter));
    }
    //public void ActiveLobbyButtons()
    //{
    //    if(_lobbyUICanvas!=null)
    //    {
    //        _lobbyUICanvas.ActiveButtons();
    //    }
    //}
}
