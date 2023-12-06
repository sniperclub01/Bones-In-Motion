using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lowerIK : MonoBehaviour
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
        Vector3 lookVector = limbEnd.transform.position - this.transform.position;
        this.transform.up = lookVector;
        lowerLimb.transform.rotation = this.transform.rotation;
        lowerLimb.transform.localEulerAngles = new Vector3(lowerLimb.transform.localEulerAngles.x, lowerLimb.transform.localEulerAngles.y, lowerLimb.transform.localEulerAngles.z);
        //Quaternion look = Quaternion.LookRotation(lookVector);
        //look *= Quaternion.AngleAxis(180, lookVector);
        //look *= Quaternion.Euler(-90, 0, 0);
        //look.AngleAxis(0, lookVector);
        //(limbEnd.transform);
        // Vector3 newAngles = this.transform.eulerAngles;
        //this.transform.rotation = look;
        //lowerLimb.transform.rotation = look;
        //lowerLimb.transform.eulerAngles = new Vector3lowerLimb.transform.eulerAngles.x//new Vector3(newAngles.x + 90, newAngles.y, newAngles.z);
        //this.transform.localEulerAngles = new Vector3((this.transform.localEulerAngles.x), this.transform.localEulerAngles.y, -180);
        //this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
        /*
        if (this.transform.localEulerAngles.x > 0)
            this.transform.localEulerAngles = new Vector3(-1* Mathf.Abs(this.transform.localEulerAngles.x), 180, this.transform.localEulerAngles.z);
        else
            this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x - 90, this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
        */
        //else ()
        //if (this.transform.localEulerAngles.y == 0 && this.transform.localEulerAngles.z == 0)
        //this.transform.localEulerAngles = new Vector3(-1*Mathf.Abs(this.transform.localEulerAngles.x), this.transform.localEulerAngles.y, -180);
        //lowerLimb.transform.eulerAngles = this.transform.eulerAngles;
        //lowerLimb.transform.localEulerAngles = new Vector3(lowerLimb.transform.localEulerAngles.x, 0, -180);
        //lowerLimb.transform.localEulerAngles = new Vector3(lowerLimb.transform.localEulerAngles.x, 0, lowerLimb.transform.localEulerAngles.z);
        //lowerLimb.transform.localEulerAngles = new Vector3(-lowerLimb.transform.localEulerAngles.x, 0, 0);
        //Vector3 scaledPos = this.transform.position;
    }
}
