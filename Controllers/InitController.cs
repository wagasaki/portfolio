using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;

public enum eLanguage
{
    Eng,
    Kor,
}

public class InitController : MonoBehaviour
{
    private static InitController instance = null;

    public static InitController Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }


    private GameDataController _gameDataController = new GameDataController();
    private SaveDataController _saveDataController = new SaveDataController();
    private GamePlayController _gamePlayController;
    private UIController _uiController;
    private SceneController _sceneController;
    private SoundController _soundController;
    private LocaleController _localeController;
    private OverlayTextEffectController _overlayTextEffectController;
    private FirebaseBinder _firebaseBinder;

    public GameDataController GameDatas { get { return _gameDataController; } }
    public SaveDataController SaveDatas { get { return _saveDataController; } }
    public GamePlayController GamePlays { get { return _gamePlayController; } }

    public SoundController Sounds { get { return _soundController; } }
    public UIController UIs { get { return _uiController; } }
    public SceneController Scenes { get { return _sceneController; } }
    public LocaleController Locales { get { return _localeController; } }
    public OverlayTextEffectController OverlayText { get{ return _overlayTextEffectController; } }

    public FirebaseBinder FirebaseBinders { get { return _firebaseBinder; } }

    #region Actions
    /// <summary>
    /// call 이후 null 초기화
    /// </summary>
    public Action OnAfterAllControllerLoaded { get; set; }

    #endregion

    private float _prevTimeScale;
    public void QuitGame()
    {
        SaveDatas.SaveGame();
        Application.Quit();
    }
    public void SetTimeScale(int speed)
    {
        _prevTimeScale = Time.timeScale;
        Time.timeScale = speed;
        if(speed == 0)
        {
            Sounds.StopAllSFX();
        }
    }
    public void ReturnTimeScale()
    {
        Time.timeScale = _prevTimeScale;
    }
    private void Awake()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("InitController");
            if (go == null)
                go = new GameObject { name = "InitController" };
            DontDestroyOnLoad(go);
            InitController component = go.GetComponent<InitController>();
            if (component == null)
                component = go.AddComponent<InitController>();

            instance = component;

            Input.multiTouchEnabled = false;

            GameDataControllerInit();//순서1
            SaveDataControllerInit();
            SceneControllerInit();
            SoundControllerInit();
            LocaleControllerInit();
            OverlayTextEffectControllerInit();

            Sounds.PlayBGM(Constants.Lobby, eBackSound.None);
            //UIControllerInit();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        void AfterFirebaseLogin()
        {
            StartCoroutine(InitPlayerControllerRout());
            IEnumerator InitPlayerControllerRout()
            {
                //LoadingPanelCanvas _loadingPanelCanvas = Instantiate(Resources.Load<LoadingPanelCanvas>(Paths.LoadingPanelCanvas));
                //_loadingPanelCanvas.InitUICanvas();
                yield return LocalizationSettings.InitializationOperation;
                InitPlayControllers();
                Sounds.PlaySFX(eSFX.Click_Enterance);
                //_loadingPanelCanvas.FadeOut();
            }
        }
        _firebaseBinder = Instantiate(Resources.Load<FirebaseBinder>(Paths.FireBaseBinder));
        FirebaseBinders.InitFirebaseAuth(() => AfterFirebaseLogin());
        
        void InitPlayControllers()
        {
#if UNITY_EDITOR
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == Constants.Main)
            {
                OnChangeSceneInitContorller(Constants.Main, true);

            }
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == Constants.Lobby)
            {
                OnChangeSceneInitContorller(Constants.Lobby, true);
            }
#elif UNITY_ANDROID

            OnChangeSceneInitContorller(Constants.Lobby,true);

#else  //현재는 ios등에대한 해당사항 없음

            OnChangeSceneInitContorller(Constants.Lobby,true);

#endif

            //GPGSBinder.Instance.Login();

            Application.targetFrameRate = PlayerPrefs.GetInt(Constants.CurrentFPS, 60);
        }
    }
    public void OnChangeSceneInitContorller(string sceneName, bool isnew)
    {
        if(sceneName == Constants.Main)
        {
            GamePlayControllerInit(isnew); //1
            UIControllerInit(); //2 

            Sounds.PlayBGM(sceneName, GamePlays.CurrentMapWayData.BackSound);
            OnAfterAllControllerLoaded?.Invoke();
            OnAfterAllControllerLoaded = null;
        }
        else if(sceneName == Constants.Lobby)
        {
            UIControllerInit();
            //_uiController.ActiveLobbyButtons();
            Sounds.PlayBGM(sceneName, eBackSound.None);
        }
        
    }

    private bool GameDataControllerInit()
    {
        return _gameDataController.DataInit();
    }
    private bool SaveDataControllerInit()
    {
        return _saveDataController.DataInit();
    }
    private bool GamePlayControllerInit(bool isnew)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GameController");
        if (obj == null)
            _gamePlayController = Instantiate(Resources.Load<GamePlayController>(Paths.GamePlayController)); 
        else
            _gamePlayController = obj.GetComponent<GamePlayController>();

        return _gamePlayController.InitGamePlayController(isnew);
    }
    private bool UIControllerInit()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("UIController");
        if (obj == null)
            _uiController = Instantiate(Resources.Load<UIController>(Paths.UIController));
        else
            _uiController = obj.GetComponent<UIController>();
        
        return _uiController.InitUIController();
    }
    private bool SceneControllerInit()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("SceneController");
        if (obj == null)
            _sceneController = Instantiate(Resources.Load<SceneController>(Paths.SceneController));
        else
            _sceneController = obj.GetComponent<SceneController>();
        return _sceneController.InitSceneController();
    }

    private bool SoundControllerInit()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("SoundController");
        if (obj == null)
            _soundController = Instantiate(Resources.Load<SoundController>(Paths.SoundContoller));
        else
            _soundController = obj.GetComponent<SoundController>();
        return _soundController.InitSoundController();
    }

    private bool LocaleControllerInit()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("LocaleController");
        if (obj == null)
            _localeController = Instantiate(Resources.Load<LocaleController>(Paths.LocaleController));
        else
            _localeController = obj.GetComponent<LocaleController>();
        return _localeController.InitLocaleController();
    }

    private bool OverlayTextEffectControllerInit()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("OverlayTextEffectController");
        if (obj == null)
            _overlayTextEffectController = Instantiate(Resources.Load<OverlayTextEffectController>(Paths.OverlayTextEffectController));
        else
            _overlayTextEffectController = obj.GetComponent<OverlayTextEffectController>();
        return _overlayTextEffectController.InitOverlayController();
    }
}
