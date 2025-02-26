using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleController : MonoBehaviour
{
    private bool _isReady = false;
    private bool _active = false;
    public eLanguage CurrentLocaleID { get; private set; }

    public bool InitLocaleController()
    {
        if (_isReady) return _isReady;

        DontDestroyOnLoad(this);

        int id = GetLocaleID();
        ChangeLocale(id);
        CurrentLocaleID = (eLanguage)id;


        _isReady = true;
        return _isReady;
    }
    public void ChangeLocale(int localeID)
    {
        //Debug.Log(localeID);
        if (_active == true)
            return;
        StartCoroutine(SetLocale(localeID));
    }
    IEnumerator SetLocale(int localeID)
    {
        _active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt("LocaleKey", localeID);
        CurrentLocaleID = Utils.ParseEnum<eLanguage>(localeID.ToString(), true);
        _active = false;
    }

    public int GetLocaleID()
    {
        int localeId;
        SystemLanguage locale = Application.systemLanguage;
        if (locale == SystemLanguage.Korean)
        {
            localeId = (int)eLanguage.Kor;
        }
        else
        {
            localeId = (int)eLanguage.Eng;
        }
        int id = PlayerPrefs.GetInt("LocaleKey", localeId);
        return id;
    }
}
