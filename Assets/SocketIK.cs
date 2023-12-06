using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketIK : MonoBehaviour
{
    public GameObject limbEnd;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookVector = limbEnd.transform.position - this.transform.position;
        this.transform.up = new Vector3(lookVector.x, lookVector.y, lookVector.z);
        Quaternion look = Quaternion.LookRotation(lookVector);
        //look *= Quaternion.Euler(0, 0, 180);
        //look *= Quaternion.AngleAxis(180, lookVector);
        //this.transform.up = new Vector3(lookVector.x, lookVector.y, lookVector.z);
        this.transform.rotation = look;
        //this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
        //lowerLimb.transform.rotation = this.transform.rotation;
        //this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, 0, this.transform.localEulerAngles.z);
    }
}
