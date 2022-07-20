/**
 *  Haircuts
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

public class Haircut : MonoBehaviour
{
    public GameObject[] Haircuts;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject g in Haircuts)
        {
            g.SetActive(false);
        }

        Haircuts[Settings.Haircut].SetActive(true);
    }

    public void UpdateHair()
    {
        foreach (GameObject g in Haircuts)
        {
            g.SetActive(false);
        }

        Haircuts[Settings.Haircut].SetActive(true);
    }
}
