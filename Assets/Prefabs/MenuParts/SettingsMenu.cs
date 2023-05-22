using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour {
    
    public AudioMixer audioMixer;
    public Slider master, music, sfx;
    public Toggle fullScreenToggle, UItoggle;

    void Start ()
    {
        LoadPrefs();
        List<string> options = new List<string>();
    }

    void Update()
    {
       Cursor.visible = true;
    }


    public void SetEffectsVolume()
    {
        audioMixer.SetFloat("SfxVolume", sfx.value);
        PlayerPrefs.SetFloat("SfxVolume", sfx.value);
        //Debug.Log(volume);
    }
    public void SetMusicVolume()
    {
         audioMixer.SetFloat("BgmVolume", music.value);
         PlayerPrefs.SetFloat("BgmVolume", music.value);
         //Debug.Log(volume);
    }
    public void SetMasterVolume()
    {
        audioMixer.SetFloat("MasterVolume", master.value);
        PlayerPrefs.SetFloat("MasterVolume", master.value);
       // Debug.Log(volume);
    }
    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

    }
    void LoadPrefs()
    {
        master.value = PlayerPrefs.GetFloat("MasterVolume");
        music.value = PlayerPrefs.GetFloat("BgmVolume");
        sfx.value = PlayerPrefs.GetFloat("SfxVolume");
    }

}
