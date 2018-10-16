using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
///things to work on:
///                     Create a basic enemy 
/// </summary>
public class PlayerController : MonoBehaviour {


    private bool isDashing = false;

    private float MAX_SPEED = 1.25f;
    private int ROLL_DISTANCE = 40;
    private float DODGE_DURATION;
    private float DODGE_COOLDOWN = .25f;
    private Vector2 dodgevect;
    private float currentSpeed;
    private float groundAccelTime = .01f;
    private float groundDecelTime = .02f;
    private float accelSpeed;
    private float decelSpeed;

    private Rigidbody2D myBody;
    private Transform myTrans;
    private Camera myCam;

    // Use this for initialization

    private void Start()
    {
        Variables();
    }
    private void FixedUpdate()
    {
        print(myBody.velocity.magnitude);
        FollowMouse();

        Movement();



    }
    private void Variables()
    {
        MAX_SPEED = 5f;
        myBody = GetComponent<Rigidbody2D>();
        myTrans = GetComponent<Transform>();
        myCam = Camera.main;

    }

    private void FollowMouse()
    {
        float camDist = myCam.transform.position.y - myTrans.position.y;
        //get mouse position in the wordlspace
        Vector3 mousePos = myCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDist));
        //gets angle in radians
        float angleRad = Mathf.Atan2(mousePos.y - myTrans.position.y, mousePos.x - myTrans.position.x);
        float angle = (180 / Mathf.PI) * angleRad;
        myBody.rotation = angle;
    }
    private void Movement()
    {
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");

        //move player
        Vector2 movement = new Vector2(hMove, vMove);
        myBody.velocity = movement.normalized * MAX_SPEED * Time.deltaTime * 50;


        //check for space key to roll player
        if (Input.GetKeyDown(KeyCode.Space) && movement != Vector2.zero && isDashing == false) //vector2.zero should be  (0,0) which movement will be if you're not moving.
        {
            
            DODGE_DURATION = 2f;
            isDashing = true;//[frames for which to dodge here, man.]
            dodgevect = movement.normalized;
            float time = 0.0f;
            while(time <= DODGE_DURATION)
            {
                time += Time.deltaTime;
                myBody.velocity = dodgevect.normalized * ROLL_DISTANCE;
            }
            Invoke("DashRefresh", DODGE_COOLDOWN);

        }

        //What all this does is set a timer for the dodge and record what direction you're dodging in, if you're moving, trying to dodge, and not standing perfectly still.


        /*        if (DODGE_DURATION != 0.0)
                {

                    while (time <= DODGE_DURATION)
                    {
                        time += Time.deltaTime; 
                         //Yes, it's already supposed to have a magnitude of one, but it's not a bad idea to be safe.

                    }

                }*/



        //                                      ACCELERATION BASED MOVEMENT SCRAPPED                   

        /* if (Input.GetKey(KeyCode.A))//Start moving left
         {
             accelSpeed = Mathf.SmoothDamp(myBody.velocity.x, -MAX_SPEED, ref accelSpeed, groundAccelTime);
             myBody.velocity = new Vector2(accelSpeed, myBody.velocity.y);
         }
         if (Input.GetKey(KeyCode.D))//start moving right
         {
             accelSpeed = Mathf.SmoothDamp(myBody.velocity.x, MAX_SPEED, ref accelSpeed, groundAccelTime);
             myBody.velocity = new Vector2(accelSpeed, myBody.velocity.y);
         }
         if (Input.GetKey(KeyCode.W))//start moving up
         {
             accelSpeed = Mathf.SmoothDamp(myBody.velocity.y, MAX_SPEED, ref accelSpeed, groundAccelTime);
             myBody.velocity = new Vector2(myBody.velocity.x, accelSpeed);
         }
         if (Input.GetKey(KeyCode.S))//start moving down
         {
             accelSpeed = Mathf.SmoothDamp(myBody.velocity.y, -MAX_SPEED, ref accelSpeed, groundAccelTime);
             myBody.velocity = new Vector2(myBody.velocity.x, accelSpeed);
         }
         if (Input.GetKeyUp(KeyCode.A))//slow down left
         {
             decelSpeed = Mathf.Clamp(Mathf.SmoothDamp(myBody.velocity.x, MAX_SPEED, ref decelSpeed, groundDecelTime), Mathf.NegativeInfinity, 0);
             myBody.velocity = new Vector2(decelSpeed, myBody.velocity.y);
         }
         if (Input.GetKeyUp(KeyCode.D))//slowdown right
         {
             decelSpeed = Mathf.Clamp(Mathf.SmoothDamp(myBody.velocity.x, -MAX_SPEED, ref decelSpeed, groundDecelTime), 0, Mathf.Infinity);
             myBody.velocity = new Vector2(decelSpeed, myBody.velocity.y);
         }
         if (Input.GetKeyUp(KeyCode.W))//slowdown up
         {
             decelSpeed = Mathf.Clamp(Mathf.SmoothDamp(myBody.velocity.y, -MAX_SPEED, ref decelSpeed, groundDecelTime), 0, Mathf.Infinity);
             myBody.velocity = new Vector2(myBody.velocity.x, decelSpeed);
         }
         if (Input.GetKeyUp(KeyCode.S))//slowdown down
         {
             decelSpeed = Mathf.Clamp(Mathf.SmoothDamp(myBody.velocity.y, MAX_SPEED, ref decelSpeed, groundDecelTime), Mathf.NegativeInfinity, 0);
             myBody.velocity = new Vector2(myBody.velocity.x, decelSpeed);
         } */

    }
    void DashRefresh()
    {
        isDashing = false;
    }
}
