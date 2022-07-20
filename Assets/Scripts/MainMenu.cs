/**
 *  Main Menu
    Copyright (C) 2021 Vizelt

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 * 
 * 
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int CurrentMap = 1;

    public Text ModFolder;

    public RawImage SkateparkImg;
    public Text SkateparkText;

    public Texture[] SkateparkScreenshots;
    public string[] SkateparkNames;

    public GameObject[] Menus;

    public VideoClip[] clips;
    private int currentClip = 0;
    public VideoPlayer player;

    [Header("Settings Menu")]
    public Slider ReplayFreqSlider;
    public Text ReplayFreqText;
    public Toggle PostProcessing;
    public Toggle Replay;
    public Toggle Shadows;
    public Slider QualitySlider;
    public Text QualityText;
    public Toggle Terrain;
    public Slider DeckWidthSlider;
    public Text DeckWidthText;
    public Slider DeckLengthSlider;
    public Text DeckLengthText;
    public Slider WheelSizeSlider;
    public Text WheelSizeText;
    public Slider HairSlider;
    public Haircut Haircut;
    public Slider PopForceSlider;
    public Text PopForceText;
    public Slider MaxSpeedSlider;
    public Text MaxSpeedText;

    private void Start()
    {
        ModFolder.text = "Mod folder: " + Application.persistentDataPath + "/Customization/";
        HairSlider.value = Settings.Haircut;
    }

    public void NextMap()
    {
        CurrentMap++;
        if (CurrentMap > SkateparkScreenshots.Length)
        {
            CurrentMap = 1;
        }
        SkateparkImg.texture = SkateparkScreenshots[CurrentMap-1];
        SkateparkText.text = SkateparkNames[CurrentMap-1];
    }

    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/GTMGS7xNMB");
    }

    public void NextClip()
    {
        currentClip++;
        if (currentClip >= clips.Length)
        {
            currentClip = 0;
        }
        player.clip = clips[currentClip];
    }

    public void SwitchMenu(int m)
    {
        CloseAllMenus();
        Menus[m].SetActive(true);

        if (Menus[m].name.ToLower() == "settings")
        {
            OnSettingsMenuOpen();
        }
    }

    public void OnSettingsMenuOpen()
    {
        ReplayFreqSlider.value = Settings.ReplayFreq;
        ReplayFreqText.text = "Replay Frequency: " + Settings.ReplayFreq + "Hz";
        QualitySlider.value = QualitySettings.GetQualityLevel();
        QualityText.text = "Quality Level : " + QualitySettings.GetQualityLevel();
        PostProcessing.isOn = Settings.PostProcessing;
        Replay.isOn = Settings.Replay;
        Shadows.isOn = Settings.Shadows;
        Terrain.isOn = Settings.Terrain;
        DeckLengthSlider.value = Settings.DeckLength;
        DeckLengthText.text = "Deck Length: " + Settings.DeckLength;
        DeckWidthSlider.value = Settings.DeckWidth;
        DeckWidthText.text = "Deck Width: " + Settings.DeckWidth;
        WheelSizeSlider.value = Settings.WheelSize;
        WheelSizeText.text = "Wheels Size: " + Settings.WheelSize;
        PopForceSlider.value = Settings.PopForce;
        PopForceText.text = "Pop Force: " + Settings.PopForce;
        MaxSpeedSlider.value = Settings.MaxSpeed;
        MaxSpeedText.text = "Max Speed: " + Settings.MaxSpeed;
    }

   

    public void FreqSliderChange()
    {
        Settings.ReplayFreq = (int) ReplayFreqSlider.value;
        ReplayFreqText.text = "Replay Frequency: " + Settings.ReplayFreq + "Hz";
    }

    public void QualitySliderChange()
    {
        Settings.QualityLevel = (int)QualitySlider.value;
        QualityText.text = "Quality Level : " + Settings.QualityLevel;
        QualitySettings.SetQualityLevel(Settings.QualityLevel);
    }

    public void HairSliderChange()
    {
        Settings.Haircut = (int)HairSlider.value;
        Haircut.UpdateHair();
    }

    public void DeckWidthChange()
    {
        Settings.DeckWidth = Round100(DeckWidthSlider.value);
        DeckWidthText.text = "Deck Width: " + Settings.DeckWidth;
    }

    public void MaxSpeedChange()
    {
        Settings.MaxSpeed = Round100(MaxSpeedSlider.value);
        MaxSpeedText.text = "Max Speed: " + Settings.MaxSpeed;
    }

    public void PopForceChange()
    {
        Settings.PopForce = Round100(PopForceSlider.value);
        PopForceText.text = "Pop Force: " + Settings.PopForce;
    }

    public void WheelSizeChange()
    {
        Settings.WheelSize = Round100(WheelSizeSlider.value);
        WheelSizeText.text = "Wheels Size: " + Settings.WheelSize;
    }

    public void DeckLengthChange()
    {
        Settings.DeckLength = Round100(DeckLengthSlider.value);
        DeckLengthText.text = "Deck Length: " + Settings.DeckLength;
    }

    public float Round100(float f)
    {
        return Mathf.RoundToInt(f * 100) / 100F;
    }

    public void BackToMainMenu()
    {
        CloseAllMenus();
        Menus[0].SetActive(true);
    }

    private void CloseAllMenus()
    {
        foreach (GameObject go in Menus)
        {
            go.SetActive(false);
        }
    }

    public void LoadMap(bool arcade)
    {
        if (arcade)
        {
            Settings.Arcade = true;
        } else
        {
            Settings.Arcade = false;
        }
        SceneManager.LoadScene(CurrentMap);
    }

    public void SetPostProcessing(bool state)
    {
        Settings.PostProcessing = state;
    }

    public void SetTerrain(bool state)
    {
        Settings.Terrain = state;
    }


    public void SetShadows(bool state)
    {
        Settings.Shadows = state;
    }

    public void SetFisheye(bool state)
    {
        Settings.FisheyeCam = state;
    }

    public void SetReplays(bool state)
    {
        Settings.Replay = state;
    }

}
