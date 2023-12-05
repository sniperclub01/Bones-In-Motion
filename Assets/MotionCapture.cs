using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using Autodesk.Fbx;
using UnityEngine.UIElements;

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
    private List<Frame> animationFrames = new List<Frame>();
    private bool isRecording = false;
    private float startTime = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartRecording();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StopRecording();
        }

        // StartRecording
        if (isRecording)
        {
            RecordFrame();
        }
        // Stop Recording
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

        ExportAnimation();
    }

    public void ExportAnimation()
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
        AnimationClip clip = CreateAnimation();
        SaveAnimation(clip, "Assets/TestingAnim.anim");

        GameObject duplicateUser = Instantiate(gameObject);

        ApplyAnimation(duplicateUser, clip);

        // Export
        ExportFBXWithRecording(duplicateUser, "Assets/TestingFBX.fbx");
        
        Destroy(duplicateUser);
         


        // For Testing
        /*ApplyAnimation(gameObject, clip);

        ExportFBXWithRecording(gameObject, "Assets/TestingFBX.fbx");*/

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

        Dictionary<string, CurveList> curveDict = new Dictionary<string, CurveList>();
        foreach (var frame in animationFrames)
        {
            foreach (var boneTransform in frame.boneTransforms)
            {
                AddKeyframe(curveDict, boneTransform.Key, frame.timestamp-startTime, boneTransform.Value.position, boneTransform.Value.rotation);
            }     
        }

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
        if (!curveDict.ContainsKey(boneName))
        {
            curveDict[boneName] = new CurveList();
        }

        curveDict[boneName].posx.Add(new Keyframe(time, pos.x));
        curveDict[boneName].posy.Add(new Keyframe(time, pos.y));
        curveDict[boneName].posz.Add(new Keyframe(time, pos.z));
        
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

    void ApplyAnimation(GameObject target, AnimationClip clip)
    {
        Animation anim = target.AddComponent<Animation>();
        anim.AddClip(clip, clip.name);
        //anim.Play(clip.name);
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


}
