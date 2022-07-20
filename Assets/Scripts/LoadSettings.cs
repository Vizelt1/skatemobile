/**
 *  Settings loader
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSettings : MonoBehaviour
{

    public Camera Cam;
    public Camera FreeCam;
    public GameObject PostProcessing;
    public GameObject Terrain;
    public Replay Replay;
    public Arcade Arcade;

    private float defaultCamFov = 68;
    private float fisheyeCamFov = 120;

    void Start()
    {
        Load();
    }

    public void LoadCamSettings()
    {
        if (Settings.FisheyeCam)
        {
            Cam.fieldOfView = fisheyeCamFov;
            FreeCam.fieldOfView = fisheyeCamFov;
        } else
        {
            Cam.fieldOfView = defaultCamFov;
            FreeCam.fieldOfView = defaultCamFov;
        }
    }


    public void Load()
    {

        if (Settings.FisheyeCam)
        {
            Cam.fieldOfView = fisheyeCamFov;
            FreeCam.fieldOfView = fisheyeCamFov;
        } else
        {
            Cam.fieldOfView = defaultCamFov;
            FreeCam.fieldOfView = defaultCamFov;
        }
        PostProcessing.SetActive(Settings.PostProcessing);
        Terrain.SetActive(Settings.Terrain);
        Replay.Frequency = Settings.ReplayFreq;
        Arcade.active = Settings.Arcade;
    }


}
