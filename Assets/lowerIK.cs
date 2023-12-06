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
        this.transform.LookAt(limbEnd.transform);
        Vector3 newAngles = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3((newAngles.x % 90) + 90, newAngles.y, newAngles.z);
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
        lowerLimb.transform.eulerAngles = this.transform.eulerAngles;
        //lowerLimb.transform.localEulerAngles = new Vector3(lowerLimb.transform.localEulerAngles.x, 0, -180);
        //lowerLimb.transform.localEulerAngles = new Vector3(lowerLimb.transform.localEulerAngles.x, 0, lowerLimb.transform.localEulerAngles.z);
        //lowerLimb.transform.localEulerAngles = new Vector3(-lowerLimb.transform.localEulerAngles.x, 0, 0);
        //Vector3 scaledPos = this.transform.position;
    }
}
