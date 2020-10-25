using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClipping : MonoBehaviour {

    float actualDistance;
    public float wantedDistance = 10;
    public float adjustmentSpeed = 10.0f;
    public Vector3 wantedPos = new Vector3(0, 1.7f, -5.0f);
    public GameObject Player;
    public LayerMask camCollisionMask;


	// Use this for initialization
	void Start () {
        actualDistance = wantedDistance;
       
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //update wanted camera positions real world coordinates
        Vector3 wantedCamPos = Player.transform.position + wantedPos;
        
        Ray ray = new Ray(Player.transform.position, this.transform.position - Player.transform.position);
        RaycastHit hit = new RaycastHit();
        //check if current camera can see player
        if (Physics.Raycast(ray,out hit, Vector3.Distance(wantedCamPos, Player.transform.position),camCollisionMask))
        {
            //zoom in
            if (hit.distance < actualDistance)
            {
                if (actualDistance - adjustmentSpeed*Time.deltaTime < hit.distance)
                {
                    actualDistance = hit.distance;
                }
                else
                {
                    actualDistance -= adjustmentSpeed * Time.deltaTime;
                }
            }//else zoom out
            else
            {
                if (actualDistance + adjustmentSpeed * Time.deltaTime > hit.distance)
                {
                    actualDistance = hit.distance;
                }
                else
                {
                    actualDistance += adjustmentSpeed * Time.deltaTime;
                }
            }
        }else if (actualDistance < wantedDistance)
        {
            actualDistance += adjustmentSpeed * Time.deltaTime;
        }        
        //update camera distance
        this.transform.localPosition = wantedPos.normalized*actualDistance;


    }
}
