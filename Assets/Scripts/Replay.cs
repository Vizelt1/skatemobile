/**
 *  Replay system
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
using UnityEngine.UI;

public class Replay : MonoBehaviour
{
    [Header("Character")]
    public Animator anim;
    public Transform Character;
    public GameObject player;
    public IKFootPlacement FootPlacement;

    [Header("Skateboard")]
    public GameObject Skateboard;
    public Animator SkaterAnim;
    public Transform FrontTruck;
    public Transform BackTruck;

    [Header("Skater")]
    public GameObject Skater;
    public GameObject Camera;

    [Header("Sounds")]
    public AudioClip[] land;
    public AudioClip[] pop;
    public AudioClip[] rolling;
    public AudioClip[] grind;

    [Header("Replay Mode")]
    public GameObject FreeCam;
    public GameObject gui;
    public GameObject guiControls;
    public Slider slider;
    public Slider speedSlider;
    public Text speedText;
    public Text KeyFrameButtonText;
    public Text FovSliderText;
    public Slider FovSlider;

    [Header("Settings")]
    public LoadSettings SettingsLoader;


    [Header("Replays value")]
    public List<float> timeStamps;
    public List<Vector3> positions;
    public List<Quaternion> rotations;
    public List<bool> isgrounded;
    public List<bool> switchStance;
    public List<bool> crouch;
    public List<string> triggers;
    public List<Quaternion> boardrotation;
    public List<float> footik;
    public List<Quaternion> ftruckrotation;
    public List<Quaternion> btruckrotation;
    public List<bool> isgrinding;
    public List<Vector3> frontfootpos;
    public List<Vector3> backfootpos;

    [Header("Replay statues")]
    [Range(1,240)] public int Frequency;
    public float playSpeed = 1;
    public bool isPlaying = false;
    public int currentFrame = 0;
    private AudioSource audioSource;

    private float StartedPlaying;

    public CameraKeyFrame key;
    public Image keyFrameIcon; 
    public List<CameraKeyFrame> CameraKeyFrames = new List<CameraKeyFrame>();

    private List<Image> kficons = new List<Image>();
    public int currentCameraKeyFrame;
    public bool PreviewMode;
    public float lastReplayKey;

    private bool inInReplayEditor = false;

    public void PlayRandomClip(AudioClip[] source)
    {
        int i = (int)(Random.Range(0, source.Length) + 0.5f);
        audioSource.PlayOneShot(source[i]);
    }

    public void SetRandomClip(AudioClip[] source)
    {
        int i = (int)(Random.Range(0, source.Length) + 0.5f);
        audioSource.clip = source[i];
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Frequency = Settings.ReplayFreq;
    }

    public void SaveValue(float time, Vector3 pos, Quaternion rot, bool ground, string trigger, Quaternion board_rot, float foot_ik, bool sw, bool cr, Quaternion ftruckRot, Quaternion btruckRot, bool grind, Vector3 frontfoot, Vector3 backfoot)
    {
        timeStamps.Add(time);
        positions.Add(pos);
        rotations.Add(rot);
        isgrounded.Add(ground);
        triggers.Add(trigger);
        boardrotation.Add(board_rot);
        footik.Add(foot_ik);
        switchStance.Add(sw);
        crouch.Add(cr);
        ftruckrotation.Add(ftruckRot);
        btruckrotation.Add(btruckRot);
        isgrinding.Add(grind);
        frontfootpos.Add(frontfoot);
        backfootpos.Add(backfoot);

        if (timeStamps.Count > 10000f)
        {
            timeStamps.Remove(timeStamps[0]);
            positions.Remove(positions[0]);
            rotations.Remove(rotations[0]);
            isgrounded.Remove(isgrounded[0]);
            triggers.Remove(triggers[0]);
            footik.Remove(footik[0]);
            boardrotation.Remove(boardrotation[0]);
            switchStance.Remove(switchStance[0]);
            crouch.Remove(crouch[0]);
            ftruckrotation.Remove(ftruckrotation[0]);
            btruckrotation.Remove(btruckrotation[0]);
            isgrinding.Remove(isgrinding[0]);
            frontfootpos.Remove(frontfootpos[0]);
            backfootpos.Remove(backfootpos[0]);
        }
    }
     
    public void OnSliderChange()
    {
        if (!isPlaying)
            GoToFrame((int) slider.value);
    }

    public void AddCameraKeyFrame()
    {
        for (int i = 0; i < CameraKeyFrames.Count; i++)
        {
            if (timeStamps[currentFrame] == CameraKeyFrames[i].time)
            {
                Destroy(CameraKeyFrames[i]);
                CameraKeyFrames.Remove(CameraKeyFrames[i]);
            }
        }
        CameraKeyFrame kf = Instantiate(key, FreeCam.transform.position, FreeCam.transform.rotation);
        kf.name = "cam_keyframe_" + CameraKeyFrames.Count;
        kf.Set(timeStamps[currentFrame], FreeCam.GetComponent<Camera>().fieldOfView);

        KeyFrameButtonText.text = "Edit Keyframe";

        kficons.Add(Instantiate(keyFrameIcon, slider.handleRect.transform.position, Quaternion.identity, slider.transform));

        CameraKeyFrames.Add(kf);
        ReorganizeCamFrames();
    }

    public void OnSpeedValueChange()
    {
        playSpeed = speedSlider.value;
        speedText.text = "Speed: " + (Mathf.Round(100 * playSpeed) / 100f) + "x";
        anim.speed = playSpeed;
        SkaterAnim.speed = playSpeed;
    }

    public void SwitchPreviewMode()
    {
        PreviewMode = !PreviewMode;
    }

    public void ToggleFisheye()
    {
        Settings.FisheyeCam = !Settings.FisheyeCam;
        SettingsLoader.LoadCamSettings();
    }

    public void HideUI()
    {
        guiControls.SetActive(false);
    }

    public void EnterReplay()
    {
        isPlaying = false;
        inInReplayEditor = true;
        GoToFrame(0);
        slider.maxValue = timeStamps.Count;
        gui.SetActive(true);
        guiControls.SetActive(true);
        player.SetActive(true);
        FreeCam.SetActive(true);
        FreeCam.transform.position = transform.position + new Vector3(0, 1, -2);
        FreeCam.transform.LookAt(transform.position);
        Skater.SetActive(false);
        Camera.SetActive(false);
    }

    public void ExitReplay()
    {
        isPlaying = false;
        inInReplayEditor = false;
        audioSource.Stop();
        gui.SetActive(false);
        guiControls.SetActive(false);
        player.SetActive(false);
        FreeCam.SetActive(false);
        Skater.SetActive(true);
        Camera.SetActive(true);
    }

    public void GoToFrame(int f)
    {
        if (f < timeStamps.Count)
            currentFrame = f;

        SkaterAnim.speed = 100f;
        anim.speed = 100f;

        anim.SetBool("IsGrounded", isgrounded[currentFrame]);
        anim.SetBool("Crouch", crouch[currentFrame]);

        if (triggers[currentFrame] != "none")
        {
            SkaterAnim.SetTrigger(triggers[currentFrame]);
        }

        if (footik[currentFrame] >= 1) FootPlacement.ReplayOnGround();
        else FootPlacement.ReplayInAir();

        FootPlacement.SetFrontFootPos(frontfootpos[currentFrame]);
        FootPlacement.SetBackFootPos(backfootpos[currentFrame]);

        Vector3 pos = positions[currentFrame];
        Quaternion rot = rotations[currentFrame];

        FrontTruck.localRotation = ftruckrotation[currentFrame];
        BackTruck.localRotation = btruckrotation[currentFrame];

        transform.position = pos;
        transform.rotation = rot;


        if (SkaterAnim.GetCurrentAnimatorStateInfo(0).IsName("zero"))
        {
            Skateboard.transform.localRotation = boardrotation[currentFrame];
            Character.localRotation = Quaternion.Euler(0, 90, boardrotation[currentFrame].eulerAngles.x);
        }
        else
        {
            Character.localRotation = Quaternion.Euler(0, 90, 0);
        }
        KeyFrameButtonText.text = "Add Keyframe";

        for (int i = 0; i < CameraKeyFrames.Count; i++)
        {
            if (timeStamps[f] == CameraKeyFrames[i].time)
            {
                currentCameraKeyFrame = i;
                KeyFrameButtonText.text = "Edit Keyframe";
                break;
            }
        }

        if (PreviewMode && !isPlaying)
        {
            currentCameraKeyFrame = 0;
            int nearestKeyFrame = 0;
            float lastNearestFrameTime = float.MaxValue;
            for (int i = 0; i < CameraKeyFrames.Count; i++)
            {
                if (timeStamps[f] >= CameraKeyFrames[i].time && CameraKeyFrames[i].time <= lastNearestFrameTime)
                {
                    nearestKeyFrame = i;
                    lastNearestFrameTime = timeStamps[f];
                }
            }

            currentCameraKeyFrame = nearestKeyFrame;
            Vector3 nextCamPos = CameraKeyFrames[currentCameraKeyFrame].pos;
            Quaternion nextCamRot = CameraKeyFrames[currentCameraKeyFrame].rot;
            float nextfov = CameraKeyFrames[currentCameraKeyFrame].fov;
            float nexttime = CameraKeyFrames[currentCameraKeyFrame].time;
           
            if (nearestKeyFrame+1 < CameraKeyFrames.Count)
            {
                nextCamPos = CameraKeyFrames[nearestKeyFrame + 1].pos;
                nextCamRot = CameraKeyFrames[nearestKeyFrame + 1].rot;
                nexttime = CameraKeyFrames[nearestKeyFrame + 1].time;
                nextfov = CameraKeyFrames[nearestKeyFrame + 1].fov;
            }
            float p = 1 - ((nexttime - timeStamps[f]) / (nexttime - CameraKeyFrames[currentCameraKeyFrame].time));
            if (float.IsNaN(p) || float.IsInfinity(p)) p = 1;

            Debug.Log("p: " + p);
            FreeCam.transform.position = Percent(CameraKeyFrames[currentCameraKeyFrame].pos, nextCamPos, p);
            FreeCam.transform.rotation = Percent(CameraKeyFrames[currentCameraKeyFrame].rot, nextCamRot, p);
            FreeCam.GetComponent<Camera>().fieldOfView = Percent(CameraKeyFrames[currentCameraKeyFrame].fov, nextfov, p);

            SkaterAnim.speed = playSpeed;
            anim.speed = playSpeed;
        }


    }

    public Vector3 Percent(Vector3 start, Vector3 end, float p)
    {
        return start + ((end - start) * p);
    }

    public Quaternion Percent(Quaternion start, Quaternion end, float p)
    {


        return Quaternion.Lerp(start, end, p);
    }

    public float Percent(float start, float end, float p)
    {


        return start + (end - start) * p;
    }




    public Vector3 Lerp(Vector3 start, Vector3 end, float timeStartedLerping, float lerpTime)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / (lerpTime);

        var result = Vector3.Lerp(start, end, percentageComplete);

        return result;
    }

    public float LerpValue(float start, float end, float timeStartedLerping, float lerpTime)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / (lerpTime);

        var result = Mathf.Lerp(start, end, percentageComplete);

        return result;
    }

    public Quaternion Slerp(Quaternion start, Quaternion end, float timeStartedLerping, float lerpTime)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / (lerpTime);

        var result = Quaternion.Lerp(start, end, percentageComplete);

        return result;
    }

    public Quaternion SlerpCam(Quaternion start, Quaternion end, float timeStartedLerping, float lerpTime)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / (lerpTime);

        var result = Quaternion.Lerp(start, end, percentageComplete * (Mathf.Clamp((Time.time - timeStartedLerping) / (lerpTime / 6f), 0f, 1f)));

        return result;
    }

    public float lastFrameChangeTime;

    public void Update()
    {

        if (currentFrame + 1 >= timeStamps.Count)
        {
            isPlaying = false;
            audioSource.Stop();
            if (inInReplayEditor && !isPlaying) gui.SetActive(true);
            return;
        }


        if (isPlaying)
        {
            float nextFrameTime = timeStamps[currentFrame + 1] / playSpeed;
            //Debug.Log(Mathf.Abs(Time.time - (nextFrameTime + StartedPlaying)));
            slider.value = currentFrame;

            transform.position = Lerp(positions[currentFrame], positions[currentFrame + 1], lastFrameChangeTime, (timeStamps[currentFrame + 1] / playSpeed) - (timeStamps[currentFrame] / playSpeed));
            transform.rotation = Slerp(rotations[currentFrame], rotations[currentFrame + 1], lastFrameChangeTime, (timeStamps[currentFrame + 1] / playSpeed) - (timeStamps[currentFrame] / playSpeed));


            FrontTruck.localRotation = ftruckrotation[currentFrame];
            BackTruck.localRotation = btruckrotation[currentFrame];

            if (footik[currentFrame] >= 1) FootPlacement.OnGround();
            else FootPlacement.ReplayInAir();

            FootPlacement.SetFrontFootPos(frontfootpos[currentFrame]);
            FootPlacement.SetBackFootPos(backfootpos[currentFrame]);



            if (SkaterAnim.GetCurrentAnimatorStateInfo(0).IsName("zero"))
            {
                Skateboard.transform.localRotation = Slerp(boardrotation[currentFrame], boardrotation[currentFrame + 1], lastFrameChangeTime, (timeStamps[currentFrame + 1] / playSpeed) - (timeStamps[currentFrame] / playSpeed)); 
                Character.localRotation = Slerp(Quaternion.Euler(0, 90, boardrotation[currentFrame].eulerAngles.x / 3f), Quaternion.Euler(0, 90, boardrotation[currentFrame+1].eulerAngles.x/3f), lastFrameChangeTime, (timeStamps[currentFrame + 1] / playSpeed) - (timeStamps[currentFrame] / playSpeed)); 
            }
            else
            {
                Character.localRotation = Quaternion.Euler(0, 90, 0);
            }

            bool wasGrounded = isgrounded[currentFrame];
            bool wasGrinding = isgrinding[currentFrame];

            if (Time.time - StartedPlaying >= nextFrameTime)
            {
                currentFrame++;
                lastFrameChangeTime = Time.time;

                if (footik[currentFrame] >= 1) FootPlacement.ReplayOnGround();
                else FootPlacement.ReplayInAir();

                anim.SetBool("IsGrounded", isgrounded[currentFrame]);
                anim.SetBool("Switch", switchStance[currentFrame]);
                anim.SetBool("Crouch", crouch[currentFrame]);
                anim.SetBool("IsGrinding", isgrinding[currentFrame]);

                if (!wasGrounded && isgrounded[currentFrame])
                {
                    PlayRandomClip(land);
                    SetRandomClip(rolling);
                    audioSource.Play();
                }

                if (!wasGrinding && isgrinding[currentFrame])
                {
   
                    SetRandomClip(grind);
                    audioSource.Play();
                } else if (wasGrinding && !isgrinding[currentFrame] && !isgrounded[currentFrame])
                {
                    audioSource.Stop();
                }



                if (triggers[currentFrame] != "none")
                {
                    string name = triggers[currentFrame];

                    PlayRandomClip(pop);
                    if (name.ToLower().Contains("heel") || name.ToLower().Contains("laser"))
                        anim.SetTrigger("PopHeel");
                    else if (name.ToLower().Contains("flip") && !name.ToLower().Contains("laser"))
                        anim.SetTrigger("PopFlip");
                    else
                        anim.SetTrigger("PopOllie");
                    SkaterAnim.SetTrigger(triggers[currentFrame]);
                    FootPlacement.ReplayInAir();
                }
            }
        }
    }

    private float lastCamKeyFrameChange;
    public void LateUpdate()
    {
        if (isPlaying)
        {
            if (hasNextCamKeyFrame())
            {
                CameraKeyFrame curKF = CameraKeyFrames[currentCameraKeyFrame];
                CameraKeyFrame nextKF = CameraKeyFrames[currentCameraKeyFrame + 1];

                FreeCam.transform.position = Lerp(curKF.pos, nextKF.pos, lastCamKeyFrameChange, ((nextKF.time / playSpeed) - (curKF.time / playSpeed)));
                FreeCam.transform.rotation = Slerp(curKF.rot, nextKF.rot, lastCamKeyFrameChange, (nextKF.time / playSpeed) - (curKF.time / playSpeed));

                FreeCam.GetComponent<Camera>().fieldOfView = LerpValue(CameraKeyFrames[currentCameraKeyFrame].fov, CameraKeyFrames[currentCameraKeyFrame+1].fov, lastCamKeyFrameChange, (nextKF.time / playSpeed) - (curKF.time / playSpeed));

                if (Time.time - StartedPlaying >= nextKF.time / playSpeed)
                {
                    currentCameraKeyFrame++;
                    lastCamKeyFrameChange = Time.time;
                }
            }
        }


    }

    public void ReorganizeCamFrames()
    {
        List<CameraKeyFrame> keyframes = new List<CameraKeyFrame>();
        while (CameraKeyFrames.Count > 0)
        {
            float firstKfTime = float.MaxValue;
            int index = 0;
            int i = 0;
            foreach (CameraKeyFrame kf in CameraKeyFrames)
            {  
                if (firstKfTime > kf.time)
                {
                    index = i;
                    firstKfTime = kf.time;
                }
                i++;
            }

            keyframes.Add(CameraKeyFrames[index]);
            CameraKeyFrames.Remove(CameraKeyFrames[index]);


        }

        CameraKeyFrames = keyframes;
    }


    public bool hasNextCamKeyFrame()
    {
        return currentCameraKeyFrame + 1 < CameraKeyFrames.Count;
    }

    public void Play()
    {
        
        GoToFrame(currentFrame);
        StartedPlaying = Time.time - (timeStamps[currentFrame] / playSpeed);
        HideUI();
 
        isPlaying = !isPlaying;
        anim.speed = playSpeed;
        audioSource.pitch = playSpeed;

        SkaterAnim.speed = playSpeed;

        if (isgrounded[currentFrame])
        {
            SetRandomClip(rolling);
            audioSource.Play();
        }
    }

    public void SetFieldOfView()
    {
        FreeCam.GetComponent<Camera>().fieldOfView = FovSlider.value;
        FovSliderText.text = "FOV: " + FovSlider.value;
    }

    public void Reset()
    {
        timeStamps.Clear();
        PreviewMode = false;
        currentFrame = 0;
        currentCameraKeyFrame = 0;
        positions.Clear();
        rotations.Clear();
        isgrounded.Clear();
        triggers.Clear();
        footik.Clear();
        switchStance.Clear();
        boardrotation.Clear();
        crouch.Clear();
        ftruckrotation.Clear();
        isgrinding.Clear();
        btruckrotation.Clear();
        frontfootpos.Clear();
        backfootpos.Clear();
        foreach (Image i in kficons)
        {
            Destroy(i.gameObject);
        }
        foreach (CameraKeyFrame i in CameraKeyFrames)
        {
            Destroy(i.gameObject);
        }
        CameraKeyFrames.Clear();
        kficons.Clear();
    }
}
