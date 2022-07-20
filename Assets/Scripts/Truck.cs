/**
 *  Truck
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

public class Truck : MonoBehaviour
{
    public Transform[] anchors = new Transform[2];
    public LayerMask LayerMask;
    public float TruckMaxAngle;
    [Range(0f,3f)] public float TruckAdjust;
    public float TruckMinDist;
    public float TruckMaxDist;
    public Skateboard Skateboard;

    private SkController controller;
    


    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<SkController>();
    }

    public void Update()
    {
        RaycastHit LeftHit;
        RaycastHit RightHit;

        bool Left = Physics.SphereCast(anchors[0].position, 0.03f, -Vector3.up, out LeftHit, TruckMaxDist, LayerMask);
        bool Right = Physics.SphereCast(anchors[1].position, 0.03f, -Vector3.up, out RightHit, TruckMaxDist, LayerMask);

        if (Left || Right)
        {
            float a = transform.localEulerAngles.x;

            if (LeftHit.distance > TruckMinDist && Left)
            {
                a += TruckAdjust;
            }
            if (RightHit.distance > TruckMinDist && Right)
            {
                a -= TruckAdjust;
            }

            if (RightHit.distance < LeftHit.distance) a = -a;

            a = Mathf.Clamp(a, -TruckMaxAngle, TruckMaxAngle);
            if (!controller.IsGrinding) a = 0;
            transform.localEulerAngles = new Vector3(-90 - a, 0, 0);
            Skateboard.zRot = a;
        }
    }

}
