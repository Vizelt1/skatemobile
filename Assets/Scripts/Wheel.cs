/**
 *  Wheel
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

public class Wheel : MonoBehaviour
{
    private Vector3 oldPos;

    // Start is called before the first frame update
    void Start()
    {
        oldPos = transform.position;
        transform.localScale *= Settings.WheelSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = transform.position;
        Vector3 delta = newPos - oldPos;
        delta.y = 0;
        Vector3 angles = transform.localEulerAngles;
        angles.y += delta.magnitude * 300;
        transform.localEulerAngles = angles;

        oldPos = newPos;
    }
}
