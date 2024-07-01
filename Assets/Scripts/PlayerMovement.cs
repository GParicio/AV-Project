using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f * 2f;
    public float jumpHeight = 3f;

    //public Transform groundCheck;
    //public float groundDistance = 0.4f;
    //public LayerMask groundMask;
    //unused variables from the manual isGrounded check

    Vector3 velocity;
    bool isGrounded;
    bool isMoving;
    int NJumped;
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // ground check
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isGrounded = controller.isGrounded; //Check if the player is grounded with the CharacterController method
        //Resseting the default velocity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;   //Resetting the velocity
            NJumped = 0;    //Resetting the number of jumps
        }

        //Getting the inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Extract horizontal components of right and forward vectors (ignore y-axis)
        Vector3 horizontalRight = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 horizontalForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

        // Creating the moving vector using horizontal components
        Vector3 move = horizontalRight * x + horizontalForward * z;
        controller.Move(move * speed * Time.deltaTime);

        //Check if the player can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            NJumped++;
        }

        if (Input.GetButtonDown("Jump") && !isGrounded && NJumped < 2)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            NJumped++;
        }

        //Falling down
        velocity.y += gravity * Time.deltaTime;

        //Executing the jump
        controller.Move(velocity * Time.deltaTime);

        if(lastPosition != transform.position)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;
    }
}
