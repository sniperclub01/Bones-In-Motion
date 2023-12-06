using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmUpperIK : MonoBehaviour
{
    public GameObject end;
    public GameObject upper;
    public GameObject upperLimb;
    public GameObject lower;
    public GameObject lowerLimb;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float constant = (float)0.5;
        float hypotenuse = Vector3.Distance(end.transform.position, this.transform.position);
        float upperLength = upperLimb.transform.lossyScale.y * constant;
        float lowerLength = lowerLimb.transform.lossyScale.y * constant;
        Vector3 current = upper.transform.eulerAngles;

        float shoulderAngle = Mathf.Rad2Deg * Mathf.Acos(((Mathf.Pow(lowerLength, 2) - Mathf.Pow(upperLength, 2) - Mathf.Pow(hypotenuse, 2)) / (-2 * upperLength * hypotenuse)));

        //float shoulderAngle = (180 / Mathf.PI) * Mathf.Acos((Mathf.Pow(upperLength,2) + Mathf.Pow(elbow2hand, 2) - Mathf.Pow(lowerLength, 2)) 
        // / (2 * upperLength * elbow2hand)); 


        //float constant = (float)1.0;
        Vector3 newPos = new Vector3(90 - shoulderAngle, upper.transform.localEulerAngles.y, upper.transform.localEulerAngles.z);
        this.transform.localEulerAngles = newPos;
        upperLimb.transform.eulerAngles = this.transform.eulerAngles;

    }
}
