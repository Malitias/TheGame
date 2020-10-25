using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    public CharacterController controller;
    public GameObject playerModel;
    public CameraSettings camSettings;

    public float maxSpeed = 50.0f;
    public bool faceCameraDir = true;

    public bool grounded = true;
    public bool jumpTry = false;
    public bool airborne = false;

    private bool canMove = true;
    Vector3 moveVector;
    Vector3 saveVelocity;
    public float jumpVelocity;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!airborne && Input.GetButtonDown("Jump"))
        {
            jumpTry = true;
        }

    }

    private void FixedUpdate()
    {
        //-----------HORIZONTAL MOVEMENT-----------
        moveVector = PlayerInputDirection() * maxSpeed;
        if (canMove)
        {
            //save velocity for airborne movement
            saveVelocity = moveVector;

            //rotate towards the cameras direction or the direction of the movement
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal"))>0  || Mathf.Abs(Input.GetAxisRaw("Vertical"))>0)
            {
                float rotationSpd = (90 + 10 * Quaternion.Angle(playerModel.transform.rotation, this.transform.rotation)) * Time.deltaTime;
                if (faceCameraDir)
                {         
                    playerModel.transform.rotation = Quaternion.RotateTowards(playerModel.transform.rotation, this.transform.rotation, rotationSpd);
                    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, playerModel.transform.rotation, rotationSpd);
                }else
                {
                    if (moveVector!= Vector3.zero)
                    {
                        Quaternion save = this.transform.rotation;
                        playerModel.transform.rotation = Quaternion.RotateTowards(playerModel.transform.rotation, Quaternion.LookRotation(moveVector, transform.up), rotationSpd);
                        this.transform.rotation = save;
                    }
                }   
            }
        }
        int invert = 1;
        if (camSettings.invertY)
        {
            invert = -1;
        }
        Vector3 cameraRotate = new Vector3(0, Input.GetAxis("Mouse X")*camSettings.sensitivityMouseX * invert, 0);
        this.transform.Rotate(cameraRotate);


        //-----------JUMPING PART-----------
        //allow jumping if player is grounded 

        if (playerModel.GetComponent<CharacterController>().isGrounded)
        {
            //base gravity to stay grounded
            jumpVelocity = -1.0f;
            canMove = true;
            airborne = false;

            //set velocity on button press
            if (jumpTry)
            {
                jumpVelocity = 15.0f;
                jumpTry = false;
                airborne = true;
            }            
        }
        else
        {
            canMove = false;


            if (jumpVelocity > 0.0f)
            {
                jumpVelocity -= 50.0f * Time.deltaTime;
            }
            else
            {
                jumpVelocity -= 100.0f * Time.deltaTime;
            }
            //TODO: REDUCE mobility while airborne
            saveVelocity = Vector3.MoveTowards(saveVelocity, PlayerInputDirection()*maxSpeed,10.69f*Time.deltaTime);
            moveVector = saveVelocity;
            //define a place where attacks or a double jump could be implemented

        }
        moveVector.y = jumpVelocity;

        controller.Move(moveVector * Time.deltaTime);
    }

    private Vector3 PlayerInputDirection()
    {
        Vector3 newMove = Vector3.zero;
        
        newMove += transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        //make sure diagonal movement is not faster
        if (newMove.magnitude > 1)
        {
            newMove = newMove.normalized;
        }

        return newMove;
    }
}
