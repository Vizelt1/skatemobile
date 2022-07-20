/**
 *  The menu at the end of an arcade game
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

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArcadePanel : MonoBehaviour
{
    public Text Text;
    public Button AdButton;

    public Arcade ac;

    private int money, exp;

 

    public void Show(int points, int m, int xp, bool ad)
    {
        exp = xp;
        money = m;
        if (!ad)
            Text.text = "Score: " + points + "\nYou earnt: " + money + " $\nand " + exp + " EXP"
            + "\nYou can watch an ad to multiply your gains by 2.";
        else
            Text.text = "Score: " + points + "\nYou earnt: " + money + " $\nand " + exp + " EXP"
            + "\nYou already watched an ad.";

        if (ad)
        {
            AdButton.enabled = false;
        }
        gameObject.SetActive(true);
    }

    public void WatchAd()
    {
        //ac.ad.ShowRewardedVideo();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);

        GameProfile.GetInstance().Balance += money;
        GameProfile.GetInstance().AddXP(exp);
        
    }
}
