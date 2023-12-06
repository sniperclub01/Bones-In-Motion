using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using Autodesk.Fbx;
using UnityEngine.UIElements;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;
using UnityEngine.Animations.Rigging;

public class CurveList
{
    public List<Keyframe> posx = new List<Keyframe>();
    public List<Keyframe> posy = new List<Keyframe>();
    public List<Keyframe> posz = new List<Keyframe>();

    public List<Keyframe> rotx = new List<Keyframe>();
    public List<Keyframe> roty = new List<Keyframe>();
    public List<Keyframe> rotz = new List<Keyframe>();
    public List<Keyframe> rotw = new List<Keyframe>();    
}

public struct Frame
{
    public float timestamp;
    public Dictionary<string, TransformData> boneTransforms;
}

public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
}
public class MotionCapture : MonoBehaviour
{
    
    public GameObject mirrorModel;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject recordIndicator;

    private List<Frame> animationFrames = new List<Frame>();
    private bool isRecording = false;
    private bool isPlaying = false;
    private float startTime = 0f;
    private AnimationClip lastClip;
 
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Input_Sources inputSource;

    // Start is called before the first frame update
    void Start()
    {
        mirrorModel.SetActive(false); // hide mirrored model to start

        trackedObject = GetComponent<SteamVR_TrackedObject>();
        inputSource = SteamVR_Input_Sources.Any;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(SteamVR_Actions.default_GrabGrip.GetStateDown(inputSource));
        bool busy = isPlaying || isRecording;
        //if ((Input.GetKeyDown(KeyCode.B) || SteamVR_Actions.default_GrabGrip.GetStateDown(inputSource)) && !isRecording) // B key or grab grib
        if ((Input.GetKeyDown(KeyCode.B) || SteamVR_Actions.default_record.GetStateDown(inputSource)) && !isRecording) // B key or grab grib
        {   
            StartRecording();
            recordIndicator.SetActive(true);
        }
        else if ((Input.GetKeyDown(KeyCode.N) || SteamVR_Actions.default_record.GetStateDown(inputSource)) && isRecording) // N key or grab grib
        {
            if (isRecording)
            {
                StopRecording();
                recordIndicator.SetActive(false);
                lastClip = CreateAnimation();
            }
        }
        if (Input.GetKeyDown(KeyCode.P) || SteamVR_Actions.default_play.GetStateDown(inputSource) 
        || SteamVR_Actions.default_SnapTurnLeft.GetStateDown(inputSource)) // P key or dpad left/right
        {
            // Need to record once before playing
            if (lastClip != null && !busy)
                PlayRecording(lastClip);
        }
        // Check if export button is clicked
        if (!isRecording)
        {
            checkExportClick(leftHand, lastClip);
            checkExportClick(rightHand, lastClip);
        }

        // StartRecording
        if (isRecording)
        {
            RecordFrame();
        }
    }

    void checkExportClick(GameObject hand, AnimationClip clip)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(hand.transform.position, hand.transform.forward, 1000.0f);

        for (int i = 0; i < hits.Length; ++i)
        {
            //int id = hits[i].gameObject.GetInstanceID();
            if (hits[i].collider.gameObject.tag == "UI")
            {
                if (hits[i].collider.gameObject.name == "ExportButton")
                {
                    if (SteamVR_Actions.default_GrabPinch.GetState(inputSource))
                    {
                        ExportAnimation(clip);
                    }
                }
            }
        }
    }


    // Record our Frames
    void RecordFrame()
    {
        Frame frame = new Frame
        {
            timestamp = Time.time,
            boneTransforms = new Dictionary<string, TransformData>()
        };

        foreach (Transform child in transform)
        {
            TraverseHierarchyAndRecordTransforms(child, frame.boneTransforms, "");
        }

        if (animationFrames.Count == 0)
        {
            startTime = Time.time;
        }
        // Store our frame
        animationFrames.Add(frame);
    }

    // Traverse Hierarchy for bones
    void TraverseHierarchyAndRecordTransforms(Transform currentTransform, Dictionary<string, TransformData> transforms, string hierarchy)
    {
        if (currentTransform != transform)
        {
            string bonePath = hierarchy + currentTransform.name;
            // Record transform data for current bone
            transforms[bonePath] = new TransformData
            {
                position = currentTransform.localPosition,
                rotation = currentTransform.localRotation
            };

            // Recursion to traverse
            foreach (Transform child in currentTransform)
            {
                TraverseHierarchyAndRecordTransforms(child, transforms, bonePath + "/");
            }
        }
    }

    public void StartRecording()
    {
        // Clear our last recorded data
        animationFrames.Clear();

        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public void ExportAnimation(AnimationClip clip)
    {
        /*foreach (var frame in animationFrames)
            {
                Debug.Log($"Timestamp: {frame.timestamp}");

                foreach (var boneTransform in frame.boneTransforms)
                {
                    Debug.Log($"Bone: {boneTransform.Key}, Position: {boneTransform.Value.position}, Rotation: {boneTransform.Value.rotation}");
                }
            }*/

        // Convert our animated frames to a suitable format
        // Use AnimationClip
        DateTime time = DateTime.Now;
        string timeStr = time.ToString("MM-dd-mm-ss");
        SaveAnimation(clip, "Assets/"+timeStr+".anim");

        GameObject duplicateUser = Instantiate(gameObject);
        duplicateUser.GetComponent<BoneRenderer>().enabled = false;
        duplicateUser.GetComponent<MotionCapture>().enabled = false;

        ApplyAnimation(duplicateUser, clip);

        // Export
        ExportFBXWithRecording(duplicateUser, "Assets/"+timeStr+"_FBX.fbx");
        
        Destroy(duplicateUser);
    }

    AnimationClip CreateAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "CurrentRecording";
        clip.legacy = true;
        ModelImporterClipAnimation clipSettings = new ModelImporterClipAnimation
        {
            name = clip.name,
            wrapMode = clip.wrapMode,

        };

        // Put bone transforms into curve list. This helps reorganize the loop structure from for-frame to for-joint so it's easy to make curves
        Dictionary<string, CurveList> curveDict = new Dictionary<string, CurveList>();
        foreach (var frame in animationFrames)
        {
            foreach (var boneTransform in frame.boneTransforms)
            {
                AddKeyframe(curveDict, boneTransform.Key, frame.timestamp-startTime, boneTransform.Value.position, boneTransform.Value.rotation);
            }     
        }

        // Loop over each joint, convert keyframes of transform values to curves, then attach to clip
        foreach (var entry in curveDict)
        {
            clip.SetCurve(entry.Key, typeof(Transform), "localPosition.x", keysToCurve(entry.Value.posx));
            clip.SetCurve(entry.Key, typeof(Transform), "localPosition.y", keysToCurve(entry.Value.posy));
            clip.SetCurve(entry.Key, typeof(Transform), "localPosition.z", keysToCurve(entry.Value.posz));
            
            clip.SetCurve(entry.Key, typeof(Transform), "localRotation.x", keysToCurve(entry.Value.rotx));
            clip.SetCurve(entry.Key, typeof(Transform), "localRotation.y", keysToCurve(entry.Value.roty));
            clip.SetCurve(entry.Key, typeof(Transform), "localRotation.z", keysToCurve(entry.Value.rotz));
            clip.SetCurve(entry.Key, typeof(Transform), "localRotation.w", keysToCurve(entry.Value.rotw));
        }

        return clip;
    }

    // Converts a list of key frames to an animation curve
    AnimationCurve keysToCurve(List<Keyframe> list)
    {
        AnimationCurve curve = new AnimationCurve();
        foreach (var keyframe in list)
        {
            curve.AddKey(keyframe);
        }
        return curve;
    }

    void SaveAnimation(AnimationClip clip, string path)
    {
        UnityEditor.AssetDatabase.CreateAsset(clip, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log("Animation Saved!");
    }

    void AddKeyframe(Dictionary<string, CurveList> curveDict, string boneName, float time, Vector3 pos, Quaternion rot)
    {
        // Initial entry in curve dictionary
        if (!curveDict.ContainsKey(boneName))
        {
            curveDict[boneName] = new CurveList();
        }

        // Add position keyframe
        curveDict[boneName].posx.Add(new Keyframe(time, pos.x));
        curveDict[boneName].posy.Add(new Keyframe(time, pos.y));
        curveDict[boneName].posz.Add(new Keyframe(time, pos.z));
        
        // Add rotation keyframe
        curveDict[boneName].rotx.Add(new Keyframe(time, rot.x));
        curveDict[boneName].roty.Add(new Keyframe(time, rot.y));
        curveDict[boneName].rotz.Add(new Keyframe(time, rot.z));
        curveDict[boneName].rotw.Add(new Keyframe(time, rot.w));
    }

    AnimationCurve CreateCurve(float time, float value)
    {
        Keyframe keyframe = new Keyframe(time, value);
        return new AnimationCurve(keyframe);
    }

    Animation ApplyAnimation(GameObject target, AnimationClip clip)
    {
        Animation anim = target.AddComponent<Animation>();
        anim.AddClip(clip, clip.name);
        return anim;
    }

    void ExportFBXWithRecording(GameObject duplicateUser, string path)
    {
        ModelExporter.ExportObject(path, duplicateUser);

        /*
         * using (FbxManager fbxManager = FbxManager.Create ()) {
         * fbxManager.SetIOSettings
         * https://forum.unity.com/threads/export-fbx-at-runtime-using-autodesk-fbx.712259/
         * https://forum.unity.com/threads/autodesk-fbx-4-1-0-build-error.1237825/
         * https://docs.unity3d.com/Packages/com.autodesk.fbx@4.0/manual/index.html#contents
         * */
    }

    // Play animation clip on mirrored model
    void PlayRecording(AnimationClip clip)
    {
        mirrorModel.SetActive(true); // show model
        Animation anim = ApplyAnimation(mirrorModel, clip);
        isPlaying = true;
        anim.Play(clip.name);

        StartCoroutine(StopMirrorModel(clip.length, anim)); // hide model after clip finishes
    }

    // Helper function to clean up model after animation
    IEnumerator StopMirrorModel (float time, Animation anim)
    {
        yield return new WaitForSeconds(time);

        mirrorModel.SetActive(false); // hide model
        Destroy(anim); // destroy animation
        isPlaying = false;
    }
}
