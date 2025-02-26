using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingData
{
    public float BGMVolume;
    public float SFXVolume;


    public SettingData()
    {
        BGMVolume = 1;
        SFXVolume = 1;
    }
}
