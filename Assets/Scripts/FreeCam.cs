using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzeGames.MobileJoystick;

/**
 *  Freecam in replay editor
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
public class FreeCam : MonoBehaviour
{
    public DitzeGames.MobileJoystick.Joystick left;
    public TouchField right;
    public Button up;
    public Button down;

    public float turnSpeed = 4.0f;
    public float moveSpeed = 2.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        Aiming();
        Movement();
       
    }

    void Aiming()
    {
        // get the mouse inputs
        float y = right.TouchDist.normalized.x * (turnSpeed * (cam.fieldOfView/68));
        rotX += right.TouchDist.normalized.y * (turnSpeed * (cam.fieldOfView / 68));

        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // rotate the camera
        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
    }

    void Movement()
    {
        Vector3 dir = new Vector3(0, 0, 0);

        dir.x = left.AxisNormalized.x;
        dir.z = left.AxisNormalized.y;

        if (up.Pressed)
        {
            dir.y++;
        } else if (down.Pressed)
        {
            dir.y--;
        }

        Quaternion q = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        transform.position += (((q * Vector3.right * dir.x) + (Vector3.up * dir.y) + (q * Vector3.forward * dir.z)) * moveSpeed * Time.deltaTime);
    }
}
