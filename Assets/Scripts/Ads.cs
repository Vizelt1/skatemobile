/**
 *  Ads
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
//using UnityEngine.Advertisements;

public class Ads : MonoBehaviour
{
    /**
#if UNITY_IOS
    public string gameId = "4229352";
#else
    public string gameId = "4229353";
#endif

    public void Start()
    {
        Advertisement.Initialize(gameId);
        Advertisement.debugMode = true;
    }

    // Implement a function for showing a rewarded video ad:
    public void ShowRewardedVideo()
    {
        if (gameId == "4229352") //IOS
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show("Rewarded_iOS"); 
            }
               
        }
        else //ANDROID
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show("Rewarded_Android");
            }
               
        }
    }

    public void ShowVideo()
    {
        if (gameId == "4229352") //IOS
        {
            if (Advertisement.IsReady())
                Advertisement.Show("Interstitial_iOS");
        } else //ANDROID
        {
            if (Advertisement.IsReady())
                Advertisement.Show("Interstitial_Android");
        }

    }


    */
}
