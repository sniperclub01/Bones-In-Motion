using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmLowerIK : MonoBehaviour
{
    public GameObject limbEnd;
    public GameObject lowerLimb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(limbEnd.transform);
        lowerLimb.transform.position = this.transform.position;
        /*Vector3 lookVector = limbEnd.transform.position - this.transform.position;
        this.transform.up = lookVector;
        lowerLimb.transform.rotation = this.transform.rotation;
        lowerLimb.transform.localEulerAngles = new Vector3(lowerLimb.transform.localEulerAngles.x, lowerLimb.transform.localEulerAngles.y, lowerLimb.transform.localEulerAngles.z);
        */
    }
}
