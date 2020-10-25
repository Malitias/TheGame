using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotaion : MonoBehaviour {

    public CameraSettings camSettings;

    float camRotate = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        int invert = 1;
        if (camSettings.invertY) {
            invert = -1;
        }
        camRotate -= Input.GetAxis("Mouse Y") * camSettings.sensitivityMouseY * invert;
        if (camRotate > 80)
        {
            camRotate = 80;
        }    
        if (camRotate < -80)
        {
            camRotate = -80;
        }         
        this.transform.localRotation = Quaternion.Euler(camRotate,0,0);
    }
}
