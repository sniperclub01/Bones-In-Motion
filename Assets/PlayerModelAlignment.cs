using UnityEngine;

public class PlayerModelAlignment : MonoBehaviour
{
    public Transform leftFootTracker;
    public Transform rightFootTracker;
    public Transform waistTracker;
    public Transform headTracker;
    public Transform leftController;
    public Transform rightController;
    public Transform playerModel;

    void Update()
    {
        // Update position and rotation of the player model based on the head tracker
        if (headTracker != null)
        {
            playerModel.position = headTracker.position;
            playerModel.rotation = Quaternion.LookRotation(headTracker.forward, Vector3.up);
        }

        // Update positions of the hands and feet
        UpdateBodyPart(leftFootTracker, "mixamorig:LeftFoot");
        UpdateBodyPart(rightFootTracker, "mixamorig:RightFoot");
        UpdateBodyPart(leftController, "mixamorig:LeftHand");
        UpdateBodyPart(rightController, "mixamorig:RightHand");
        UpdateBodyPart(waistTracker, "mixamorig:Hips");
    }

    void UpdateBodyPart(Transform tracker, string bodyPartName)
    {
        Transform bodyPart = FindChildRecursive(playerModel, bodyPartName);

        if (tracker != null && bodyPart != null)
        {
            // Update position and rotation of the body part based on the tracker
            bodyPart.position = tracker.position;
            bodyPart.rotation = tracker.rotation;
        }
        else
        {
            Debug.LogWarning("Body part not found: " + bodyPartName);
        }
    }

    Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform result = FindChildRecursive(child, childName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
