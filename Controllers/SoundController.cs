using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    private bool _isReady = false;

    private AudioSource _bgmAudioSource, _mainBGMAudioSource, _backSource, _sfxSource;
    private AudioMixer _audioMixer;
    private AudioMixerGroup _sfxMixerGroup;
    private float _bgmVolume;
    private float _sfxVolume;
    private int _mainBGMIndex;
    private Dictionary<int, AudioClip> _mainBGMDic;
    private Dictionary<int, WaitForSeconds> _sfxTick;
    private Coroutine _bgmRout, _mainBGMRout;
    private Dictionary<string, AudioSource> _sfxDic = new Dictionary<string, AudioSource>();

    public bool InitSoundController()
    {
        if (_isReady) return _isReady;

        DontDestroyOnLoad(this);
        _sfxTick = new Dictionary<int, WaitForSeconds>();
        foreach(var a in InitController.Instance.GameDatas.SFXClipDic)
        {
            int leng = Mathf.CeilToInt(a.Value.length);
            if (_sfxTick.ContainsKey(leng)==false)
            {
                _sfxTick.Add(leng, new WaitForSeconds(leng));
            }
        }

        _bgmAudioSource = Utils.FindChild<AudioSource>(gameObject, "BGMAudioSource", true);
        _mainBGMAudioSource = Utils.FindChild<AudioSource>(gameObject, "MainBGMAudioSource", true);
        _backSource = Utils.FindChild<AudioSource>(gameObject, "BackAudioSource", true);
        _sfxSource = Utils.FindChild<AudioSource>(gameObject, "SFXAudioSource", true);
        _audioMixer = Resources.Load<AudioMixer>(Paths.MainAudioMixer);
        _bgmAudioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
        _mainBGMAudioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
        _backSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
        _sfxSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("SFX")[0];
        _sfxMixerGroup = _sfxSource.outputAudioMixerGroup;

        InitController.Instance.Scenes.OnSceneLoadCompleted += PlayBGM;
        //InitController.Instance.Scenes.OnSceneExitStarted += StopBGM;

        _bgmVolume = PlayerPrefs.GetFloat(Constants.BGMVolume, 0.5f);
        _sfxVolume = PlayerPrefs.GetFloat(Constants.SFXVolume, 0.5f);
        current = _bgmVolume;
        //_mainBGMAudioSource.clip = Resources.Load<AudioClip>("RPG/Sound/BGM/357");
        _mainBGMIndex = 0;
        _mainBGMDic = new Dictionary<int, AudioClip>();
        AudioClip[] mainbgms = Resources.LoadAll<AudioClip>("RPG/Sound/BGM/Main");
        int index = 0;
        foreach(var a in mainbgms)
        {
            _mainBGMDic.Add(index, a);
            index++;
        }

        _isReady = true;
        return _isReady;
    }
    private void Start()
    {
        BGMVolume(_bgmVolume);
        SFXVolume(_sfxVolume);
    }
    public void SetBGMVolume(float value)
    {
        _bgmVolume = value;
        BGMVolume(_bgmVolume);
    }
    public void SetSFXVolume(float value)
    {
        _sfxVolume = value;
        SFXVolume(_bgmVolume);
    }
    private void BGMVolume(float volume)
    {
        _audioMixer.SetFloat(Constants.BGMVolume, 20 * Mathf.Log10(volume));
    }
    private void SFXVolume(float volume)
    {
        _audioMixer.SetFloat(Constants.SFXVolume, 20 * Mathf.Log10(volume));
    }
    public void PlayBGM(string sceneName, eBackSound backSound, bool isDecrescendo = true)
    {
        if (_bgmRout != null) StopCoroutine(_bgmRout);
        _bgmRout = StartCoroutine(PlayBGMRout(sceneName, backSound, isDecrescendo));
    }
    float current;
    IEnumerator PlayBGMRout(string sceneName, eBackSound backSound, bool isDecrescendo = true)
    {
        //float current = _bgmVolume;
        float timeproc = current;
        while (current > 0.0001f)
        {
            current = Mathf.Max(0.0001f, current - Time.deltaTime* timeproc);
            BGMVolume(current);
            yield return null;
        }
        _bgmAudioSource.Pause();
        _mainBGMAudioSource.Pause();
        if (_mainBGMRout != null)
        {
            StopCoroutine(_mainBGMRout);
            _mainBGMRout = null;
        }
        switch (sceneName)
        {
            case Constants.Lobby:
                Debug.Log("Lobby BGM");
                _bgmAudioSource.clip = Resources.Load<AudioClip>("RPG/Sound/BGM/Locations_Forest_Loop_CompleteTrack");
                _bgmAudioSource.Play();
                _bgmAudioSource.loop = true;
                break;
            case Constants.Main:
                Debug.Log("Main BGM");
                _mainBGMAudioSource.Play();
                _mainBGMRout = StartCoroutine(MainBGMIsPlayingCheck());

                break;
            case Constants.Battle:
                Debug.Log("Battle BGM");
                _bgmAudioSource.clip = Resources.Load<AudioClip>("RPG/Sound/BGM/Battle");
                _bgmAudioSource.Play();
                _bgmAudioSource.loop = true;
                break;
            case Constants.Victory:
                Debug.Log("Victory BGM");
                _bgmAudioSource.clip = Resources.Load<AudioClip>("RPG/Sound/BGM/Victory");
                _bgmAudioSource.Play();
                _bgmAudioSource.loop = false;
                break;
            case Constants.Lose:
                Debug.Log("Lose BGM");
                _bgmAudioSource.clip = Resources.Load<AudioClip>("RPG/Sound/BGM/Lose");
                _bgmAudioSource.Play();
                _bgmAudioSource.loop = false;
                break;
            case Constants.GameOver:
                Debug.Log("GameOver BGM");
                _bgmAudioSource.clip = Resources.Load<AudioClip>("RPG/Sound/BGM/Lose");//
                _bgmAudioSource.Play();
                _bgmAudioSource.loop = true;
                break;
            default:
                Debug.Log($"{sceneName} scene is null");
                break;
        }

        switch (backSound)
        {
            case eBackSound.None:
                _backSource.Stop();
                break;
            case eBackSound.CaveSound:
                _backSource.clip = Resources.Load<AudioClip>("RPG/Sound/Climate/" + Constants.CaveSound);
                _backSource.Play();
                _backSource.loop = true;
                break;
            case eBackSound.ForestSound:
                _backSource.clip = Resources.Load<AudioClip>("RPG/Sound/Climate/" + Constants.ForestSound);
                _backSource.Play();
                _backSource.loop = true;
                break;
            case eBackSound.PlaneSound:
                _backSource.clip = Resources.Load<AudioClip>("RPG/Sound/Climate/" + Constants.PlaneSound);
                _backSource.Play();
                _backSource.loop = true;
                break;
            case eBackSound.SnowStormSound:

                _backSource.clip = Resources.Load<AudioClip>("RPG/Sound/Climate/" + Constants.SnowStormSound);
                _backSource.Play();
                _backSource.loop = true;
                Debug.Log(_backSource.isPlaying);
                break;
            default:
                break;
        }
        timeproc = _bgmVolume;
        while (current < _bgmVolume)
        {
            current = Mathf.Max(0.0001f, current + Time.deltaTime * timeproc * 0.5f);
            BGMVolume(current);
            yield return null;
        }
    }
    IEnumerator MainBGMIsPlayingCheck()
    {
        WaitForSeconds tick = new WaitForSeconds(2f);
        while(true)
        {
            if(_mainBGMAudioSource.isPlaying == false)
            {
                int index = (_mainBGMIndex) % _mainBGMDic.Count;
                _mainBGMAudioSource.clip = _mainBGMDic[index];
                _mainBGMIndex++;
                _mainBGMAudioSource.Play();
            }
            yield return tick;
        }
    }
    void AdjustPitch(AudioSource source)
    {
        if (Time.timeScale > 1)
        {
            float scale = 1 + (Time.timeScale - 1) / 4; //TODO 원래 2로 나눴었는데.. 너무 피치가 빨라져서 소리가 듣기가 싫어짐. 그래서 그냥 소리가 좀 늦더라도 덜 어색하도록
            source.pitch = scale;
            float pitchshifter = 1f / (1 + (Time.timeScale - 1) / 4); 
            source.outputAudioMixerGroup.audioMixer.SetFloat("PitchBlend", pitchshifter);
        }
        else
        {
            source.outputAudioMixerGroup.audioMixer.SetFloat("PitchBlend", 1f);
            source.pitch = 1;
        }
    }
    public void PlaySFX(eSFX sfxenum)
    {
        string name = sfxenum.ToString();

        if(_sfxDic.TryGetValue(name, out AudioSource source))
        {
            source.Stop();
            source.PlayOneShot(InitController.Instance.GameDatas.SFXClipDic[sfxenum]);
            AdjustPitch(source);
            return;
        }

        GameObject obj = new GameObject(sfxenum.ToString());
        obj.transform.parent = gameObject.transform;
        AudioSource newSource = obj.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = _sfxMixerGroup;
        newSource.PlayOneShot(InitController.Instance.GameDatas.SFXClipDic[sfxenum]);
        AdjustPitch(newSource);
        _sfxDic.Add(sfxenum.ToString(), newSource);
    }
    public void PlaySFX(string skillsound, int index)
    {
        string name = index.ToString();
        if (_sfxDic.TryGetValue(name, out AudioSource source))
        {
            source.Stop();
            source.PlayOneShot(InitController.Instance.GameDatas.SkillSFXClipDic[skillsound]);
            AdjustPitch(source);
            return;
        }

        GameObject obj = new GameObject(name);
        obj.transform.parent = gameObject.transform;
        AudioSource newSource = obj.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = _sfxMixerGroup;
        newSource.PlayOneShot(InitController.Instance.GameDatas.SkillSFXClipDic[skillsound]);
        AdjustPitch(newSource);
        _sfxDic.Add(name, newSource);

    }
    public void PlaySFXLoop(eSFX sfxenum)
    {
        string name = sfxenum.ToString();
        if (_sfxDic.TryGetValue(name, out AudioSource source))
        {
            if (source.isPlaying) return;
            source.Stop();
            if(source.clip == null) source.clip =InitController.Instance.GameDatas.SFXClipDic[sfxenum];
            source.loop = true;
            source.Play();
            AdjustPitch(source);
            return;
        }

        GameObject obj = new GameObject(sfxenum.ToString());
        obj.transform.parent = gameObject.transform;
        AudioSource newSource = obj.AddComponent<AudioSource>();
        newSource.loop = true;
        newSource.outputAudioMixerGroup = _sfxMixerGroup;
        newSource.clip = InitController.Instance.GameDatas.SFXClipDic[sfxenum];
        newSource.Play();
        AdjustPitch(newSource);
        _sfxDic.Add(name, newSource);
    }
    public void StopSFX(eSFX sfxenum)
    {
        string name = sfxenum.ToString();

        if(_sfxDic.TryGetValue(name, out AudioSource source))
        {
            source.Stop();
            return;
        }
    }
    public void StopAllSFX()
    {
        foreach(var a in _sfxDic)
        {
            a.Value.Stop();
        }
    }

    //IEnumerator SFXDisable(GameObject obj, WaitForSeconds tick)
    //{
    //    yield return tick;
    //    obj.gameObject.SetActive(false);
    //}
}
