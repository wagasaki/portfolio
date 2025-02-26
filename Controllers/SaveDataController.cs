#if UNITY_WINRT && !UNITY_EDITOR
using UnityEngine.Windows;
#endif
using GleyAllPlatformsSave;
using System;
using System.IO;
using UnityEngine;
/// <summary>
/// Version 1.0.3
/// 
/// Serializes any type of serializable class to a byte array which is then encrypted and saved to PlayerPrefs of to a file depending on your settings
/// 
/// For a detailed usage example see the TestSave.cs
/// 
/// </summary>
using UnityEngine.Events;

public class SaveDataController : SaveDataBase
{

    private string _fullPath;

    public UserData UserData { get; private set; }

#if JSONSerializationGooglePlaySave
    static ISaveClass cloudSaveMethod;
#endif

    public override bool DataInit()
    {
        if (_isReady) return _isReady;

        AddRequiredScript();
        _fullPath =  Application.persistentDataPath + "/UserData/";
        Debug.Log(_fullPath);
        LoadGame();


        _isReady = true;
        return _isReady;
    }

    public Action OnAdsRemove { get; set; }
    public void RemoveAds()
    {
        if (UserData.GetIsAdsHide == false)
        {
            UserData.GetIsAdsHide = true;
            SaveGame();
            OnAdsRemove?.Invoke();
            OnAdsRemove = null;
        }
        else
        {
            Debug.Log("RemoveAdsButton have not to exist. IsAdsHide already true");
        }
    }


    #region DataSave&Load
    public void SaveGame()
    {
        if(Directory.Exists(_fullPath) == false)
        {
            Directory.CreateDirectory(_fullPath);
        }


        UserData.PlayTime = (long)Time.realtimeSinceStartup;
        string savepath = _fullPath + Constants.SaveFileName;
        Save(UserData, savepath, DataWasSaved, true);
    }

    public void SaveGameToGPGS()
    {
        SaveString(UserData, DataWasSavedToGPGS, true);
    }
    public void LoadGame()
    {
        string loadpath = _fullPath + Constants.SaveFileName;
        Load<UserData>(loadpath, DataWasLoaded, true);
    }
    string _loadfileregisteredTime;
    public void LoadGameFromGPGS()
    {
        GPGSSaveLoadUIPopup popup = InitController.Instance.UIs.OpenUIPopup<GPGSSaveLoadUIPopup>(null, InitController.Instance.UIs.transform);
        popup.InitUIPopup();
        GPGSBinder.Instance.LoadCloud(Constants.SaveFileName, (success, result, game) => {
            if (success)
            {
                Debug.Log("Data load from GPGS success");
                _loadfileregisteredTime = game.Description;
                InitController.Instance.UIs.CloseUIPopup(popup);
                LoadString<UserData>(result, DataWasLoadedFromGPGS, true);
            }
            else
            {
                Debug.Log("Data load from GPGS failed");
                InitController.Instance.OverlayText.TextEffect("GPGS Failed");
                InitController.Instance.UIs.CloseUIPopup(popup);
            }

        });

    }
    public void ClearGPGS()
    {
        GPGSSaveLoadUIPopup popup = InitController.Instance.UIs.OpenUIPopup<GPGSSaveLoadUIPopup>(null, InitController.Instance.UIs.transform);
        popup.InitUIPopup();
        GPGSBinder.Instance.DeleteCloud(Constants.SaveFileName, (success) =>
        {
            if(success)
            {
                Debug.Log("Data Cleared");
                InitController.Instance.OverlayText.TextEffect("Data Cleared");
            }
            else
            {
                Debug.Log("Failed to Clear");
                InitController.Instance.OverlayText.TextEffect("Failed to Clear");
            }
            InitController.Instance.UIs.CloseUIPopup(popup);
        });
    }
    public void ClearDatas()
    {
        ClearAllData(_fullPath);
        Debug.Log("clear data in "+_fullPath);
    }
    public void ClearFiles(string filename)
    {
        string clearfilepath = _fullPath + filename;
        ClearFIle(clearfilepath);
        Debug.Log("clear files in " +clearfilepath);
    }

    public void ClearFilesOnlyForTest(string filename)
    {
        string clearfilepath = _fullPath + filename;
        ClearFIle(clearfilepath);
        UserData = null;
        LoadGame();
    }

    private void DataWasLoaded(UserData data, SaveResult result, string message)
    {
        if (result == SaveResult.Success)
        {
            UserData = data;
            if(data.IngameStatData !=null && data.IngameStatData.GetTotalStat(eStatInfo.Stamina) <= 0)
            {
                UserData.IngameStatData = null;
                Debug.Log("Data exist. But Stamina is zero, clear the IngameStatData and loaded successfully");
            }
            else
            {
                Debug.Log("Data exist. Data loaded successfully");
            }
        }
        else if (result == SaveResult.EmptyData)
        {
            Debug.Log("Data is empty. Created new data");

            UserData = new UserData();
            UserData.NewUserData();
        }
        else
        {
            Debug.Log("Data load failed : " + message);
        }
    }
    private void DataWasSaved(SaveResult result, string message)
    {
        if (result == SaveResult.Success)
        {
            Debug.Log("data saved successfully");
        }
        else if (result == SaveResult.EmptyData)
        {
            Debug.Log("data is empty");
        }
        else
        {
            Debug.Log("data save failed : " + message);
        }
    }
    private void DataWasSavedToGPGS(SaveResult result, string message)
    {
        GPGSSaveLoadUIPopup popup = InitController.Instance.UIs.OpenUIPopup<GPGSSaveLoadUIPopup>(null, InitController.Instance.UIs.transform);
        popup.InitUIPopup();
        if (result == SaveResult.Success)
        {
            Debug.Log("data converted to string successfully");
            GPGSBinder.Instance.SaveCloud(Constants.SaveFileName, message, (success) =>
            {
                if (success)
                {
                    Debug.Log("Save to Cloud Success");
                    InitController.Instance.OverlayText.TextEffect("Success");
                }
                else
                {
                    Debug.Log("Save to Cloud Failed");

                    InitController.Instance.OverlayText.TextEffect("Failed");
                }
                InitController.Instance.UIs.CloseUIPopup(popup);
                return;
            });
        }
        else
        {
            InitController.Instance.OverlayText.TextEffect(result.ToString());
            InitController.Instance.UIs.CloseUIPopup(popup);
        }
    }
    private void DataWasLoadedFromGPGS(UserData data, SaveResult result, string message)
    {
        if (result == SaveResult.Success)
        {
            YesOrNoUIPopup popup = InitController.Instance.UIs.OpenUIPopup<YesOrNoUIPopup>(null, InitController.Instance.UIs.transform);
            popup.InitUIPopup();
            popup.OnGPGSLoadCalledFromLobbySetting(()=>OnYes(), _loadfileregisteredTime, data);

            void OnYes()
            {
                void DataCheck()//TODO HACK 보존해야할 데이터를 여기서 관리... 서버 데이터베이스를 안쓰기 때문에 당장은 하나하나 이런식으로 체크해주는 수밖에없을거같음
                {
                    if (UserData.GetIsAdsHide == true) data.GetIsAdsHide = true;
                }

                DataCheck();

                UserData = data;
                if (data.IngameStatData != null && data.IngameStatData.GetTotalStat(eStatInfo.Stamina) <= 0)
                {
                    UserData.IngameStatData = null;
                    Debug.Log("Data exist. But Stamina is zero, clear the IngameStatData and loaded successfully");
                    InitController.Instance.OverlayText.TextEffect("Success/NewGame");
                }
                else if (data.IngameStatData != null && data.IngameStatData.GetTotalStat(eStatInfo.Stamina) > 0)
                {
                    Debug.Log("Data exist. Data loaded successfully");
                    InitController.Instance.OverlayText.TextEffect("Success/Continue");
                }
                else
                {
                    Debug.Log("Data exist. Data loaded successfully but as IngameStatData is null, start new game");
                    InitController.Instance.OverlayText.TextEffect("Success/NewGame");
                }
            }

        }
        else 
        {
            InitController.Instance.OverlayText.TextEffect(string.Concat(result.ToString(), message));
        }
    }

    #endregion
    public Action<int> OnSoulChanged { get; set; }
    public bool UseSoul(int amount, Action<int> OnUse = null)
    {
        if (UserData.CurrentSoul < amount)
        {
            return false;
        }
        UserData.CurrentSoul -= amount;
        OnUse?.Invoke(UserData.CurrentSoul);
        OnSoulChanged?.Invoke(UserData.CurrentSoul);
        return true;
    }
    public bool GetSoul(int amount, Action<int> OnGet = null)
    {
        UserData.CurrentSoul += amount;
        int current = UserData.CurrentSoul;
        OnGet?.Invoke(current);
        OnSoulChanged?.Invoke(current);
        return true;
    }
}
