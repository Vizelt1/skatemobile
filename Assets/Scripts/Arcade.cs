/**
 *  Arcade Mode
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
using UnityEngine.UI;
using UnityEngine.Advertisements;



public class Arcade : MonoBehaviour//, IUnityAdsListener
{
    public Ads ad;
    public int points;

    public float timeStarted;
    public float duration = 120;

    public Text text;

    public ArcadePanel ArcadePanel;
    private bool hasFinished = false;

    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        //Advertisement.AddListener(this);
        points = 0;
        timeStarted = Time.time;
        hasFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeStarted > duration && !hasFinished)
        {
            if (active) {
                hasFinished = true;
                Finish();
            }
            else
            {
                timeStarted = Time.time;
                //ad.ShowVideo();
            }


        } else
        {
            if (active) text.text = "Time left: " + Mathf.RoundToInt(duration - (Time.time - timeStarted)) + "s , Points : " + points;
        }
    }

    public void Finish()
    {
        //ad.ShowVideo();
        int money = Mathf.RoundToInt(points * 0.1f);
        int xp = Mathf.RoundToInt(points * 0.2f);
        ArcadePanel.Show(points, money, xp, false);
    }
    /**

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError(message);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if ((placementId == "Rewarded_Android" || placementId == "Rewarded_iOS") && showResult == ShowResult.Finished)
        {
            int money = Mathf.RoundToInt(points * 0.2f);
            int xp = Mathf.RoundToInt(points * 0.2f);
            ArcadePanel.Show(points, money, xp, true);
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
       // throw new System.NotImplementedException();
    }

    public void OnUnityAdsReady(string placementId)
    {
        //throw new System.NotImplementedException();
    }*/
}
