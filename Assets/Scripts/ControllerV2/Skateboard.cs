/**
 *  Skateboard controls
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

public class Skateboard : MonoBehaviour
{

    public float TrickDuration;
    public float TrickStartTime;
    public float BoneAngle;
    [Range(0,90)] public int MaxFeetRotation;
    public Animator Animator;
    [HideInInspector] public Animator SkateAnimator;
    public IKFootPlacement FootPlacement;
    public Transform Character;
    public float GroundDist;

    private float BoneFactor;
    private float RotateFactor;
    public bool Trick = false;
    private bool IsGrounded = true;
    private bool IsGrinding = false;

    private SkController Controller;
    public float zRot = 0;

    public Collider[] colliders;

    public void SetGround(bool b)
    {
        IsGrounded = b;
    }

    public void SetGrinding(bool b)
    {
        IsGrinding = b;
    }

    private void Start()
    {
        SkateAnimator = GetComponent<Animator>();
        Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<SkController>();

    }

    void FixedUpdate()
    {
        
        if (!Trick && IsGrounded && !IsGrinding && !(SkateAnimator.GetBool("Nosemanual") || SkateAnimator.GetBool("Manual")))
        {
            zRot = 0;
            //transform.parent.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            Character.localRotation = Quaternion.Euler(0, 90, 0);
        }


        if (Time.time < TrickStartTime + TrickDuration) {

            return;
        }

        if (Time.time > TrickStartTime + TrickDuration && !IsGrounded && !Controller.IsGrinding)
        {
            if (Trick)
            {
                FootPlacement.OnGround();
                transform.localRotation = Quaternion.identity;
                Trick = false;
                foreach (Collider cl in colliders)
                {
                    cl.enabled = true;
                }
            }


            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(BoneAngle * BoneFactor, RotateFactor * MaxFeetRotation, zRot), 0.5f);
            Character.localRotation = Quaternion.Slerp(Character.localRotation, Quaternion.Euler(0, 90 + (RotateFactor / 2f) * MaxFeetRotation, BoneAngle * BoneFactor / 4f), 0.2f);
        }

        if (IsGrounded && Trick)
        {
            if (Trick)
            {
                FootPlacement.OnGround();
                Trick = false;
            }
        }

        if (Controller.IsGrinding)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(BoneAngle * BoneFactor, RotateFactor * MaxFeetRotation, zRot), 0.5f);
            Character.localRotation = Quaternion.Slerp(Character.localRotation, Quaternion.Euler(0, 90 + (RotateFactor / 2f) * MaxFeetRotation, BoneAngle * BoneFactor*0.4f), 0.2f);
        }
    }


    public Quaternion GetLocalRot()
    {
        if (Trick) return Quaternion.identity;
        else return transform.localRotation;
    }

    public void SetRotateFactor(float f)
    {
        RotateFactor = Mathf.Clamp(f, -1f, 1f);
    }

    public void SetBoneFactor(float f)
    {
        BoneFactor = Mathf.Clamp(f, -1f, 1f);
    }

    public void PerformTrick(string trickName)
    {
        if (Trick) return;
        Trick = true;
        foreach (Collider cl in colliders)
        {
            cl.enabled = false;
        }
        TrickStartTime = Time.time;
        FootPlacement.InAir();
        SkateAnimator.SetTrigger(trickName);
    }

}
