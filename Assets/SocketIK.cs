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
        this.transform.LookAt(limbEnd.transform);
        //this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, 0, this.transform.localEulerAngles.z);
    }
}
