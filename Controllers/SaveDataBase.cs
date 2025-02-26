#if UNITY_WINRT && !UNITY_EDITOR
using UnityEngine.Windows;
#endif
using GleyAllPlatformsSave;
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

public class SaveDataBase
{
    static ISaveClass saveMethod;
    protected bool _isReady = false;


#if JSONSerializationGooglePlaySave
    static ISaveClass cloudSaveMethod;
#endif
    public virtual bool DataInit()
    {
        return _isReady;
    }

    protected static void AddRequiredScript()
    {
#if JSONSerializationFileSave
        saveMethod = new JSONSerializationFileSave();
#endif

#if JSONSerializationPlayerPrefs
    saveMethod = new JSONSerializationPlayerPrefs();
#endif

#if BinarySerializationFileSave
        saveMethod = new BinarySerializationFileSave();
#endif

#if BinarySerializationPlayerPrefs
    saveMethod = new BinarySerializationPlayerPrefs();
#endif
    }


    /// <summary>
    /// Save the specified dataToSave, to fileName and encrypt it.
    /// </summary>
    /// <param name="dataToSave">Any type of serializable class</param>
    /// <param name="path">File name path.</param>
    /// <param name="encrypt">If set to <c>true</c> encrypt.</param>
    /// <param name="CompleteMethod">Called after all is done</param>
    protected void Save<T>(T dataToSave, string path, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new()
    {
        if (saveMethod == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Current platform (" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + ") is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#else
             Debug.LogError("Current platform is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#endif
            return;
        }
        saveMethod.Save<T>(dataToSave, path, CompleteMethod, encrypted);
    }

    protected void SaveString<T>(T dataToSave, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new()
    {
        saveMethod.SaveString<T>(dataToSave, CompleteMethod, encrypted);
    }

    /// <summary>
    /// Load the specified fileName and decript it.
    /// Returns any type of serializable data
    /// If specified file does not exists, a new one is generated and the defauld values from serializeable class are saved 
    /// </summary>
    /// <param name="path">File name.</param>
    /// <param name="CompleteMethod">Called after all is done</param>
    /// <param name="encrypted">If set to <c>true</c> encrypted.</param>
    protected void Load<T>(string path, UnityAction<T, SaveResult, string> CompleteMethod, bool encrypted) where T : class, new()
    {
        if (saveMethod == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Current platform (" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + ") is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#else
             Debug.LogError("Current platform is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#endif
            return;
        }
        saveMethod.Load<T>(path, CompleteMethod, encrypted);
    }
    protected void LoadString<T>(string stringToLoad, UnityAction<T, SaveResult, string> LoadCompleteMethod, bool encrypted) where T : class, new()
    {
        saveMethod.LoadString<T>(stringToLoad, LoadCompleteMethod, encrypted);
    }

    protected void ClearAllData(string path)
    {
        if (saveMethod == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Current platform (" + UnityEditor.EditorUserBuildSettings.activeBuildTarget + ") is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#else
             Debug.LogError("Current platform is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
#endif
            return;
        }
        saveMethod.ClearAllData(path);
    }


    protected void ClearFIle(string path)
    {
        if (saveMethod == null)
        {
            Debug.LogError("Current platform is not added to plugin settings. Go to Window->Gley->Save and add your current platform");
            return;
        }
        saveMethod.ClearFile(path);
    }
}
