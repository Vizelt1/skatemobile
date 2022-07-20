/**
 *  Spline
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

public class Spline : MonoBehaviour
{
    public Vector3[] points;
    public Transform[] transforms;
    private SkController player;
    private bool isFollowingThisSpline = false;
    private Transform ContactPoint;

    void Start()
    {
        transforms = new Transform[transform.childCount];
        points = new Vector3[transform.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            transforms[i] = transform.GetChild(i);
            points[i] = transforms[i].position;
            
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<SkController>();
    }

    private void Update()
    {
        RaycastHit hit;
        for (int i = 0; i < points.Length-1; i++)
        {
            Ray r = new Ray(points[i], points[i + 1] - points[i]);
            Debug.DrawRay(r.origin, r.direction * Vector3.Distance(points[i], points[i + 1]), Color.blue);

            if (Physics.SphereCast(r.origin, 0.03f, r.direction * Vector3.Distance(points[i], points[i + 1]), out hit, Vector3.Distance(points[i], points[i + 1])))
            {
                if (hit.collider.gameObject.layer == 11)
                {
                    Debug.Log(hit.transform.position);
                    player.SplineFollow.OldRot = player.transform.rotation;
                    player.SplineFollow.ContactPoint = hit.transform.position;
                    player.StartFollow(i, i + 1, this, hit.distance, hit.point);
                    return;
                }
            }

        }
    }
}
