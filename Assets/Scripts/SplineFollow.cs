/**
 *  SplineFollow
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

[RequireComponent(typeof(Rigidbody))]
public class SplineFollow : MonoBehaviour
{
    private float lastGrindTime;
    private float grindDelay = 0.5f;
    public Spline spline;
    public bool Forward = true;
    public float Speed = 1f;
    public float DownForce = 1f;

    public bool follow;
    private float duration;
    public int currentPoint;
    private float timeStarted;
    private int nextPoint = 1; // 1 = forward, -1 = backward
    public Vector3 offset = Vector3.zero;

    public Vector3 ContactPoint;
    public Quaternion OldRot;

    private Rigidbody rb;
    private SkController sc;

    public float LastGrindTime()
    {
        return Time.time - lastGrindTime;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SkController>();
    }

    public void StartFollow(Spline sp, float speed, bool forward, int cp, float percent, Vector3 offset)
    {
        if (Time.time < lastGrindTime + grindDelay)
        {
            sc.IsGrinding = false;
            return;
        }
    
        rb.isKinematic = false;
        spline = sp;
        sc.Skateboard.Trick = false;
        sc.FootPlacement.OnGround();
        Speed = Mathf.Clamp(speed, 1, sc.MaxSpeed);
        this.offset = ContactPoint - offset;
        transform.rotation = OldRot;
        offset.y = 0;
        AlignWithSurface();

        if (sp.gameObject.layer == 10)//9 = concrete , 10 = metal
        {
            sc.PlayRandomClip(sc.grind_rail_land);
            sc.SetRandomClip(sc.grind_rail);
            sc.audioSource.Play();
        }
        if (sp.gameObject.layer == 9)//9 = concrete , 10 = metal
        {
            sc.SetRandomClip(sc.grind_concrete);
            sc.audioSource.Play();
        }

        if (sp.gameObject.layer == 12) //coping{
        {
            Speed /= 6f;
        }

        currentPoint = cp;
        Forward = forward;
        if (Forward)
        {
            nextPoint = 1;
        }
        else
        {
            nextPoint = -1;
        }
        CalculateDuration(currentPoint);
        timeStarted = Time.time - (duration * percent);
        follow = true;
    }




    public void CalculateDuration(int p)
    {
        Vector3 p1 = spline.points[p];
        Vector3 p2 = spline.points[p+nextPoint];
        Debug.LogWarning("NextPoint: " + (p + nextPoint));

        float dist = Vector3.Distance(p1, p2);

        duration = dist / Speed;
    }



    // Update is called once per frame
    void Update()
    {
        if (follow)
        {

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = Lerp(spline.points[currentPoint] + offset, spline.points[currentPoint + nextPoint] + offset, timeStarted, duration);


            //(Quaternion.LookRotation(spline.points[currentPoint + nextPoint] - spline.points[currentPoint]));

            if (Vector3.Distance(transform.position - offset, spline.points[currentPoint+nextPoint]) < 0.1f) {
                if (hasNextPoint())
                {
                    currentPoint += nextPoint;
                    CalculateDuration(currentPoint);
                    timeStarted = Time.time;
                    AlignWithSurface();

                }
                else
                {
                    Debug.LogWarning("Stop Follow");
                    StopFollow();
                    
                }
                
            }
        }
    }


    public void AlignWithSurface()
    {
        Ray ray = new Ray(sc.GroundChecker.position, -Vector3.up);
        RaycastHit hit;
        Quaternion rot;

        if (Physics.Raycast(ray, out hit))

        {

            rot = Quaternion.FromToRotation(sc.transform.up, hit.normal) *
            transform.rotation;


            rb.MoveRotation(rot);
            

        }
    }

    public bool hasNextPoint()
    {
        return currentPoint+nextPoint > 0 && (currentPoint+nextPoint) < spline.points.Length-1;
    }

    public void StopFollow()
    {
        if (!follow) return;
       
        lastGrindTime = Time.time;
        follow = false;
        rb.isKinematic = false;
        rb.detectCollisions = true;

        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        rb.AddForceAtPosition((spline.points[currentPoint + nextPoint] - spline.points[currentPoint]).normalized * Mathf.Clamp(Speed,1,7), rb.worldCenterOfMass, ForceMode.VelocityChange);
        rb.AddForceAtPosition(transform.up * sc.PopOut, rb.worldCenterOfMass, ForceMode.Acceleration);
        sc.IsGrinding = false;
    }


    public Vector3 Lerp(Vector3 start, Vector3 end, float timeStartedLerping, float lerpTime)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / (lerpTime);

        var result = Vector3.Lerp(start, end, percentageComplete);

        return result;
    }
}
