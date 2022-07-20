/**
 *  Feet Placement using Inverse Kinematics
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

public class IKFootPlacement : MonoBehaviour
{
    private Animator Animator;

    [Header("Feet")]
    public Transform FrontFoot;
    public Transform BackFoot;
    public Transform FrontFootAir;
    public Transform BackFootAir;

    [Header("Offset")]
    public Vector2 FrontFootMaxOffset;
    public Vector2 BackFootMaxOffset;
    public float maxFootHeight = 0.2f;
    public float FootWeight = 1f;


    private Vector3 DefaultFFPos;
    private Vector3 DefaultBFPos;

    private Vector3 DefaultFFAPos;
    private Vector3 DefaultBFAPos;


    void Start()
    {
        Animator = GetComponent<Animator>();
        DefaultFFPos = FrontFoot.localPosition;
        DefaultBFPos = BackFoot.localPosition;
        DefaultFFAPos = FrontFootAir.localPosition;
        DefaultBFAPos = BackFootAir.localPosition;
    }

    public void FeetOffset(float rx, float ry, float lx, float ly, bool NollieFirst)
    {
        if (FootWeight == 0)
        {
            float f = 0.3f;
            byte a = 1;
            byte b = 1;
            if (rx+ry+lx+ly == 0)
            {
                f = 0.2f;
            }

            if (NollieFirst) b = 2;
            else a = 2;

            f = 0.5f;

            ry = Mathf.Clamp(ry, -0.5f, 0.2f);
            ly = Mathf.Clamp(ly, -0.2f, 0.5f);
            Vector3 LeftOffset = new Vector3(DefaultFFAPos.x + 1.5f * FrontFootMaxOffset.x * lx, DefaultFFAPos.y + 0.1f*a + (ly * 0.1f), DefaultFFAPos.z + 1.2f * FrontFootMaxOffset.y * ly);
            Vector3 RightOffset = new Vector3(DefaultBFAPos.x + 1.5f*BackFootMaxOffset.x * rx, DefaultBFAPos.y + 0.1f*b - (ry * 0.1f), DefaultBFAPos.z + 1.2f * BackFootMaxOffset.y * ry);

            FrontFootAir.localPosition = Vector3.Lerp(FrontFootAir.localPosition, LeftOffset, f);
            BackFootAir.localPosition = Vector3.Lerp(BackFootAir.localPosition, RightOffset, f);

            return;
        } else
        {
            ry = Mathf.Clamp(ry, -0.5f, 0.2f);
            ly = Mathf.Clamp(ly, -0.2f, 0.5f);
            Vector3 LeftOffset = new Vector3(DefaultFFPos.x + FrontFootMaxOffset.x * lx, DefaultFFPos.y + (ly * 0.2f), DefaultFFPos.z + FrontFootMaxOffset.y * ly);
            Vector3 RightOffset = new Vector3(DefaultBFPos.x + BackFootMaxOffset.x * rx, DefaultBFPos.y - (ry * 0.2f), DefaultBFPos.z + BackFootMaxOffset.y * ry);

            FrontFoot.localPosition = Vector3.Lerp(FrontFoot.localPosition, LeftOffset, 0.3f);
            BackFoot.localPosition = Vector3.Lerp(BackFoot.localPosition, RightOffset, 0.3f);
            FrontFootAir.localPosition = DefaultFFAPos;
            BackFootAir.localPosition = DefaultBFAPos;
        }
    }

    // Update is called once per frame
    void OnAnimatorIK()
    {

        Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, FootWeight);
        Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, FootWeight);

        Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, FootWeight);
        Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, FootWeight);

        Animator.SetIKPosition(AvatarIKGoal.LeftFoot, FrontFoot.position);
        Animator.SetIKPosition(AvatarIKGoal.RightFoot, BackFoot.position);

        Animator.SetIKRotation(AvatarIKGoal.LeftFoot, FrontFoot.rotation);
        Animator.SetIKRotation(AvatarIKGoal.RightFoot, BackFoot.rotation);

    }

    public Vector3 GetFrontFootPos()
    {
        if (FootWeight == 0) return FrontFootAir.localPosition;
        else return FrontFoot.localPosition;
    }

    public Vector3 GetBackFootPos()
    {
        if (FootWeight == 0) return BackFootAir.localPosition;
        else return BackFoot.localPosition;
    }

    public void SetFrontFootPos(Vector3 v)
    {
        if (FootWeight == 0) FrontFootAir.localPosition = v;
        else FrontFoot.localPosition = v;
    }

    public void SetBackFootPos(Vector3 v)
    {
        if (FootWeight == 0) BackFootAir.localPosition = v;
        else BackFoot.localPosition = v;
    }

    public void InAir()
    {
        if (FootWeight == 0) return;
        FootWeight = 0;
    }

    public void OnGround()
    {
        if (FootWeight == 1) return;
        FootWeight = 1;
    }

    public void ReplayInAir()
    {
        FootWeight = 0;
    }

    public void ReplayOnGround()
    {
        FootWeight = 1;
    }

}
