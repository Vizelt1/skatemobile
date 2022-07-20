/**
 *  Skater Controller
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
using DitzeGames.MobileJoystick;

[RequireComponent(typeof(Rigidbody))]
public class SkController : MonoBehaviour
{
    [Header("Game")]
    public Arcade arcade;

    [Header("Skate")]
    public Skateboard Skateboard;
    public Transform Nose, Tail;
    public Truck FrontTruck, BackTruck;

    [Header("Stats")]
    public float PushForce;
    public float SmoothAlign = 0.3f;
    public float PopOut;
    public float TurnSpeed;
    public float RotateSpeed;
    public float PopForce;
    public float MaxSpeed;
    public float AngleDirChangeLimit;

    private GameObject Camera;
    [Header("Camera")] public Transform CameraHolder;
    [Range(0f, 1f)] public float CameraSmoothFollow;
    public Vector3 CameraOffset;

    [Header("Grind")]
    public bool IsGrinding;

    [Header("Animator")]
    public Animator Animator;
    public IKFootPlacement FootPlacement;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] land;
    public AudioClip[] pop;
    public AudioClip[] rolling;
    public AudioClip[] grind_rail;
    public AudioClip[] grind_rail_land;
    public AudioClip[] grind_concrete;

    [Header("Replay")]
    public Replay replay;

    [Header("Boutons")]
    [SerializeField] private Joystick LeftJoystick;
    [SerializeField] private Joystick RightJoystick;
    [SerializeField] private Button LeftTurn;
    [SerializeField] private Button RightTurn;
    [SerializeField] private Button PushButton;

    [Header("Physique")]
    public bool IsGrounded;
    public bool Fakie;
    public LayerMask LayerMask;
    public Transform GroundChecker;
    private Rigidbody rb;
    private float mult = 0;

    //Axis
    private float L_minYAxis, L_minXAxis, L_maxYAxis, L_maxXAxis;
    private float R_minYAxis, R_minXAxis, R_maxYAxis, R_maxXAxis;
    private float L_curYAxis, L_curXAxis;
    private float R_curYAxis, R_curXAxis;

    [HideInInspector] public SplineFollow SplineFollow;
    private bool NollieFirst;
    private bool RegularFirst;
    private float lastPopTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        SplineFollow = GetComponent<SplineFollow>();
        MoveToSpawnPoint();

        PopForce = Settings.PopForce;
        MaxSpeed = Settings.MaxSpeed;
    }

    public void PlayRandomClip(AudioClip[] source)
    {
        int i = (int) (Random.Range(0, source.Length) + 0.5f);
        audioSource.PlayOneShot(source[i]);
    }

    public void SetRandomClip(AudioClip[] source)
    {
        int i = (int)(Random.Range(0, source.Length) + 0.5f);
        audioSource.clip = source[i];
    }

    public void Update()
    {
        Trick();


        Animator.SetBool("IsGrinding", IsGrinding);
        if (Time.time - replay.lastReplayKey > (0.5f - (replay.Frequency * 0.01f)))
        {
            replay.lastReplayKey = Time.time;
            SaveReplayValue("none");
        }

        CameraHolder.position = transform.position + CameraOffset;

        if (IsGrounded && !SplineFollow.follow)
        {
            if (Fakie)
            {
                CameraHolder.rotation = Quaternion.Slerp(CameraHolder.rotation, Quaternion.Euler(CameraHolder.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 180, CameraHolder.rotation.eulerAngles.z), CameraSmoothFollow);
            }
            else
            {
                CameraHolder.rotation = Quaternion.Slerp(CameraHolder.rotation, Quaternion.Euler(CameraHolder.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, CameraHolder.rotation.eulerAngles.z), CameraSmoothFollow);
            }
        }
    }

    void FixedUpdate()
    {

        bool wasGrounded = IsGrounded;
        RaycastHit hit;
        if (Physics.Raycast(GroundChecker.position, -transform.up, out hit, 100f, LayerMask))
        {
            if (IsGrinding || (Time.time - lastPopTime < 0.1f))
            {
                IsGrounded = false;
            }
            else
            {
                Skateboard.GroundDist = hit.distance;
                if (hit.distance > 0.2f) IsGrounded = false;
                else IsGrounded = true;
            }

        }


        /**
        RaycastHit F_hit;
        RaycastHit B_hit;
        Ray F_ray = new Ray(Nose.position, Vector3.down);
        Ray B_ray = new Ray(Tail.position, Vector3.down);

        Debug.DrawRay(F_ray.origin, F_ray.direction, Color.cyan);
        Debug.DrawRay(B_ray.origin, B_ray.direction, Color.magenta);

        if (!IsGrinding && !IsGrounded && Physics.Raycast(B_ray, out B_hit, 2f, LayerMask) && Physics.Raycast(F_ray, out F_hit, 2f, LayerMask) && (L_curXAxis + R_curXAxis + L_curYAxis + R_curYAxis == 0))
        {
            if (Mathf.Abs(B_hit.distance - F_hit.distance) > 0.1f)
            {
                Vector3 rot = transform.localEulerAngles;
                
                if (B_hit.distance > F_hit.distance)
                {
                    rot.x -= autoLean;
                }
                else
                {
                    rot.x += autoLean;
                }

                transform.localEulerAngles = rot;
            }



        }*/

        AlignWithSurface();

        if (rb.velocity.magnitude < 0.1 && !IsGrinding)
        {
            Fakie = false;
            audioSource.Stop();
        }
        else if (!IsGrinding)
        {
            SetRandomClip(rolling);
            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        if (Fakie && IsGrounded)
        {
            Animator.SetBool("Switch", true);
        }
        else if (IsGrounded)
        {
            Animator.SetBool("Switch", false);
        }



        if (!wasGrounded && IsGrounded)
        {//land
            rb.constraints = RigidbodyConstraints.None;
            NollieFirst = false;
            RegularFirst = false;
            PlayRandomClip(land);
            ClearAxis();
            SetRandomClip(rolling);
            audioSource.Play();
        }

        Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 vel = new Vector2(rb.velocity.x, rb.velocity.z);

        float angle = Mathf.Abs(Vector3.Angle(forward, vel));
        if (Fakie)
        {
            forward *= -1;
        }
        angle = Mathf.Abs(Vector3.Angle(forward, vel));

        if ((angle > 180 - AngleDirChangeLimit && angle < 180 + AngleDirChangeLimit) && vel.magnitude > 0.1f)
        {
            Fakie = !Fakie;
        }

        Animator.SetBool("IsGrounded", IsGrounded);


        if (Input.GetButton("Marker"))
        {
            SetSpawnPoint();
        }

        if (Input.GetButton("Respawn"))
        {
            MoveToSpawnPoint();
        }

        if (Input.GetButton("Push") || PushButton.Pressed)
        {
            Push();
        }
        if (Input.GetAxis("RTrigger") > 0 || RightTurn.Pressed)
            Turn(1);
        else if (Input.GetAxis("LTrigger") > 0 || LeftTurn.Pressed)
            Turn(-1);


        CheckVelocity();



    }

    public void Push()
    {
        if (!IsGrounded || IsGrinding) return;
        if (Fakie) rb.AddForceAtPosition(-transform.forward * PushForce, transform.position);
        else rb.AddForceAtPosition(transform.forward * PushForce, transform.position);
    }

    public void Turn(int dir)
    {
        //if (IsGrinding) return;
        //rb.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + (force * TurnSpeed), transform.rotation.eulerAngles.z));
        if (IsGrounded)
            transform.Rotate(0, TurnSpeed * dir, 0, Space.Self);
        else
            transform.Rotate(0, RotateSpeed * dir, 0, Space.Self);
    }

    public void CheckVelocity()
    {

        if (!IsGrounded) return;

        if (rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxSpeed;
        }

        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = 0;
        rb.velocity = transform.TransformDirection(localVelocity);
    }

    public void AlignWithSurface()
    {
        Ray ray = new Ray(GroundChecker.position, -Vector3.up);
        RaycastHit hit;
        Quaternion rot;

        if (Physics.Raycast(ray, out hit) && !IsGrinding)

        {

            rot = Quaternion.FromToRotation(GroundChecker.up, hit.normal) *
            transform.rotation;

            if (Vector3.Cross(rot.eulerAngles, Vector3.right) == Vector3.zero) return;


            transform.rotation = Quaternion.Lerp(transform.rotation, rot,
            SmoothAlign);

        }
    }
    public void StartFollow(int p1, int p2, Spline sp, float dist, Vector3 hitPoint)
    {
        if (IsGrinding) return;
        ClearAxis();
        IsGrinding = true;
        
        Vector2 dir = new Vector2(sp.points[p2].x - sp.points[p1].x, sp.points[p2].z - sp.points[p1].z);
        Vector2 vel = new Vector2(rb.velocity.x, rb.velocity.z);

        bool fw = true;
        int cp = p1;
        float percent = dist / Vector3.Distance(sp.points[p2], sp.points[p1]);
        Vector3 offset = hitPoint;

        if (Vector3.Angle(dir, vel) > 90)
        {
            fw = false;
            cp = p2;
            percent = 1 - percent;
        }

        Debug.Log(cp + ";" + Vector3.Angle(dir, vel) + ";" + percent + ";" + fw);
        SplineFollow.StartFollow(sp, rb.velocity.magnitude, fw, cp, percent, offset);
        NollieFirst = false;
        RegularFirst = false;
        SaveReplayValue("none");
        ClearAxis();
    }



    public void Pop()
    {
        bool grind = IsGrinding;
        if (Time.time < lastPopTime + 0.2f) return;
        lastPopTime = Time.time;
        if (IsGrinding) SplineFollow.StopFollow();
        audioSource.Stop();
        PlayRandomClip(pop);
        IsGrounded = false;
        if (grind) rb.AddForceAtPosition(Vector3.up * PopForce / 2f, rb.worldCenterOfMass, ForceMode.Acceleration);
        else rb.AddForceAtPosition(Vector3.up * PopForce, rb.worldCenterOfMass, ForceMode.Acceleration);
    }

    private float Sign(float f)
    {
        if (f == 0) return 1;
        else return Mathf.Abs(f) / f;
    }

    public void Trick()
    {
        L_curXAxis = LeftJoystick.Horizontal + Input.GetAxis("L_Hor");
        L_curYAxis = LeftJoystick.Vertical + Input.GetAxis("L_Ver");

        R_curXAxis = RightJoystick.Horizontal + Input.GetAxis("R_Hor");
        R_curYAxis = RightJoystick.Vertical + Input.GetAxis("R_Ver");

        Skateboard.SetGround(IsGrounded);
        Skateboard.SetGrinding(IsGrinding);
        Skateboard.SetBoneFactor(L_curYAxis + R_curYAxis);
        Skateboard.SetRotateFactor((L_curXAxis / 2f) - (R_curXAxis / 2f));
        FootPlacement.FeetOffset(R_curXAxis, R_curYAxis, L_curXAxis, L_curYAxis, NollieFirst);

        


        if (IsGrounded && (R_curYAxis < -0.1f || L_curYAxis > 0.1f))
        {
            Animator.SetBool("Crouch", true);
        }
        else
        {
            Animator.SetBool("Crouch", false);
        }

        if (!IsGrounded && !IsGrinding) return;

        if (IsGrinding && (R_curYAxis <= -0.5 || L_curYAxis >= 0.5))
            ClearAxis();

        if ((R_minYAxis < -0.9f || (IsGrinding && (R_minYAxis < -0.5f))) && !NollieFirst)
        {
            RegularFirst = true;
            NollieFirst = false;
            Animator.SetBool("Switch", false);
            if (R_curYAxis > -0.1f && R_curYAxis < 0.1f && !NollieFirst)
            {
                Pop();


                if (R_minXAxis > -0.4f && R_maxXAxis < 0.4f)
                { // Pour les ollies, kickflip, heel et dolphin

                    if (L_curXAxis < -0.3f)
                    {
                        PerformTrick("Kickflip");
                        arcade.points += 10;

                    }
                    else if (L_curXAxis > 0.3f)
                    {
                        PerformTrick("Heelflip");
                        arcade.points += 10;
                    }
                    else
                    {
                        PerformTrick("Ollie");
                        arcade.points += 1;
                    }

                }
                else
                {// Pour les shovit et les shovit + flip

                    if (R_maxXAxis < 0.9f && R_minXAxis > -0.9f)
                    {//Shov

                        if (R_minXAxis < -0.4f)
                        {
                            if (L_curXAxis < -0.3f)
                            {
                                PerformTrick("VarialFlip");
                                arcade.points += 15;
                            }
                            else if (L_curXAxis > 0.3f)
                            {
                                PerformTrick("InwardHeel");
                                arcade.points += 20;
                            }
                            else
                            {
                                PerformTrick("BSShov");
                                arcade.points += 10;
                            }

                        }
                        else if (R_maxXAxis > 0.3f)
                        {
                            if (L_curXAxis < -0.3f)
                            {
                                PerformTrick("Hardflip");
                                arcade.points += 20;
                            }
                            else if (L_curXAxis > 0.3f)
                            {
                                PerformTrick("VarialHeel");
                                arcade.points += 20;
                            }
                            else
                            {
                                PerformTrick("FSShov");
                                arcade.points += 15;
                            }
                        }

                    }
                    else
                    {//360 shov


                        if (R_minXAxis < -0.9f)
                        {
                            if (L_curXAxis < -0.3f)
                            {
                                PerformTrick("TreFlip");
                                arcade.points += 25;
                            }
                            else if (L_curXAxis > 0.3f)
                            {
                                PerformTrick("InwardHeel");
                                arcade.points += 20;
                            }
                            else
                            {
                                PerformTrick("360BSShove");
                                arcade.points += 20;
                            }

                        }
                        else if (R_maxXAxis > 0.9f)
                        {
                            if (L_curXAxis < -0.3f)
                            {
                                PerformTrick("Hardflip");
                                arcade.points += 20;
                            }
                            else if (L_curXAxis > 0.3f)
                            {
                                PerformTrick("LaserFlip");
                                arcade.points += 30;
                            }
                            else
                            {
                                PerformTrick("360FSShove");
                                arcade.points += 25;
                            }
                        }

                    }

                }

                ClearAxis();
                return;
            }
        }



        //Nollie et Switch
        if ((L_maxYAxis > 0.9f || (IsGrinding && (L_maxYAxis > 0.5f) && !RegularFirst)))
        {
            NollieFirst = true;
            RegularFirst = false;
            Animator.SetBool("Switch", true);
            if (L_curYAxis < 0.1f && L_curYAxis > -0.1f && !RegularFirst)
            {
                Pop();


                if (L_minXAxis > -0.4f && L_maxXAxis < 0.4f)
                { // Pour les ollies, kickflip, heel et dolphin


                    if (R_curXAxis < -0.3f)
                    {
                        PerformTrick("NollieKickflip");
                        arcade.points += 20;
                    }
                    else if (R_curXAxis > 0.3f)
                    {
                        PerformTrick("NollieHeel");
                        arcade.points += 20;
                    }
                    else
                    {
                        PerformTrick("Nollie");
                        arcade.points += 2;
                    }

                }
                else
                {// Pour les shovit et les shovit + flip

                    if (L_maxXAxis < 0.9f && L_minXAxis > -0.9f)
                    {//Shov

                        if (L_minXAxis < -0.4f)
                        {
                            if (R_curXAxis < -0.3f)
                            {
                                PerformTrick("NollieVarialFlip");
                                arcade.points += 20;
                            }
                            else if (R_curXAxis > 0.3f)
                            {
                                PerformTrick("NollieInwardHeel");
                                arcade.points += 30;
                            }
                            else
                            {
                                PerformTrick("NollieBSShov");
                                arcade.points += 8;
                            }

                        }
                        else if (L_maxXAxis > 0.3f)
                        {
                            if (R_curXAxis < -0.3f)
                            {
                                PerformTrick("NollieHardflip");
                                arcade.points += 25;
                            }
                            else if (R_curXAxis > 0.3f)
                            {
                                PerformTrick("NollieVarialHeel");
                                arcade.points += 18;
                            }
                            else
                            {
                                PerformTrick("NollieFSShov");
                                arcade.points += 10;
                            }
                        }

                    }
                    else
                    {//360 shov


                        if (L_minXAxis < -0.9f)
                        {
                            if (R_curXAxis < -0.3f)
                            {
                                PerformTrick("NollieTreFlip");
                                arcade.points += 35;
                            }
                            else if (R_curXAxis > 0.3f)
                            {
                                PerformTrick("NollieInwardHeel");
                                arcade.points += 30;
                            }
                            else
                            {
                                PerformTrick("Nollie360BSShove");
                                arcade.points += 24;
                            }

                        }
                        else if (L_maxXAxis > 0.9f)
                        {
                            if (R_curXAxis < -0.3f)
                            {
                                PerformTrick("NollieHardflip");
                                arcade.points += 25;
                            }
                            else if (R_curXAxis > 0.3f)
                            {
                                PerformTrick("NollieLaserFlip");
                                arcade.points += 28;
                            }
                            else
                            {
                                PerformTrick("Nollie360FSShove");
                                arcade.points += 20;
                            }
                        }

                    }

                }

                ClearAxis();
                return;
            }
        }

        if (R_curYAxis < R_minYAxis) R_minYAxis = R_curYAxis;
        if (R_curYAxis > R_maxYAxis) R_maxYAxis = R_curYAxis;
        if (R_curXAxis < R_minXAxis) R_minXAxis = R_curXAxis;
        if (R_curXAxis > R_maxXAxis) R_maxXAxis = R_curXAxis;

        if (L_curYAxis < L_minYAxis) L_minYAxis = L_curYAxis;
        if (L_curYAxis > L_maxYAxis) L_maxYAxis = L_curYAxis;
        if (L_curXAxis < L_minXAxis) L_minXAxis = L_curXAxis;
        if (L_curXAxis > L_maxXAxis) L_maxXAxis = L_curXAxis;
    }

    public void ClearAxis()
    {
        L_minXAxis = 0;
        L_maxXAxis = 0;
        L_minYAxis = 0;
        L_maxYAxis = 0;
        R_minXAxis = 0;
        R_maxXAxis = 0;
        R_minYAxis = 0;
        R_maxYAxis = 0;

    }

    public void PerformTrick(string name)
    {
        //Pour pouvoir enregistrer facilement dans le replay
        Skateboard.PerformTrick(name);
        SaveReplayValue(name);
        if (name.ToLower().Contains("heel") || name.ToLower().Contains("laser"))          
            Animator.SetTrigger("PopHeel");
        else if (name.ToLower().Contains("flip") && !name.ToLower().Contains("laser"))
            Animator.SetTrigger("PopFlip");
        else
            Animator.SetTrigger("PopOllie");
    }

    public void SaveReplayValue(string trigger)
    {
        if (!Settings.Replay) return;
        if (!gameObject.activeSelf) return;
        replay.SaveValue(Time.time, transform.position, transform.rotation, IsGrounded, trigger, Skateboard.GetLocalRot(), FootPlacement.FootWeight, Animator.GetBool("Switch"), Animator.GetBool("Crouch"), FrontTruck.transform.localRotation, BackTruck.transform.localRotation, IsGrinding, FootPlacement.GetFrontFootPos(), FootPlacement.GetBackFootPos());
    }

    public void SetSpawnPoint()
    {
        GameObject.FindGameObjectWithTag("SpawnPoint").transform.position = transform.position;
        GameObject.FindGameObjectWithTag("SpawnPoint").transform.rotation = transform.rotation;
    }

    public void MoveToSpawnPoint()
    {
        SplineFollow.StopFollow();
        rb.Sleep();
        transform.position = (GameObject.FindGameObjectWithTag("SpawnPoint").transform.position);
        transform.rotation = (GameObject.FindGameObjectWithTag("SpawnPoint").transform.rotation);
        rb.velocity = Vector3.zero;
        replay.Reset();
        Fakie = false;
    }
}