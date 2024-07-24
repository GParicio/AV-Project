using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private const int fovZoom = 40;
    private const int fovNoZoom = 90;
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
    public float dashSpeed = 20f; // Speed of the dash
    public float dashTime = 0.2f; // How long the dash lasts
    private bool isDashing = false; // To check if the player is currently dashing
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    public float dashCooldown = 2f; // Cooldown duration in seconds
    private float lastDashTime = -Mathf.Infinity; // Initialize to a very early time
    public TextMeshProUGUI dashCooldownText; // Assign this in the Inspector
    public float fallLimitY = -50;
    public AudioClip fallSound;
    public AudioClip[] knifeAttackSounds;
    public GameObject flashlight;
    public Camera playerCamera;
    private bool playerFell = false;

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

        // Check if player is below -50 on the y axis
        if (transform.position.y < fallLimitY)
        {
            controller.enabled = false;
            transform.position = new Vector3(0, 30, 0);
            controller.enabled = true;
            //play a sfx
            // Play sound effect
            playerFell = true;
        }

        if (playerFell && transform.position.y < 10)
        {
            AudioSource.PlayClipAtPoint(fallSound, transform.position);
            playerFell = false;
        }

        //Toggle flashlight
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.SetActive(!flashlight.activeSelf);
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

        //Dash when pressing E
        if (Input.GetKeyDown(KeyCode.E) && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash()); //I should encapsulate all of this mess
        }
        //When holding down right click
        if(Input.GetMouseButtonDown(1))
        {
            playerCamera.fieldOfView = fovZoom;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            playerCamera.fieldOfView = fovNoZoom;
        }

        if (!isDashing && Time.time < lastDashTime + dashCooldown)
        {
            dashCooldownText.enabled = true;
            dashCooldownText.text = "Dash Ready in: " + Mathf.Ceil(lastDashTime + dashCooldown - Time.time).ToString();
        }
        else
        {
            dashCooldownText.enabled = false;
        }

        //Mouse1 to attack with knife
        if (Input.GetMouseButtonDown(0))
        {
            //Attack with knife
            //Debug.Log("Attacking with knife");
            int randomIndex = Random.Range(0, 2);
            AudioSource.PlayClipAtPoint(knifeAttackSounds[randomIndex], transform.position);
            //randomize the attack sound
            
        }

        IEnumerator Dash()
        {
            lastDashTime = Time.time; // Mark the start of the dash
            float startTime = Time.time; // Remember the time when the dash started
            isDashing = true;
            while (Time.time < startTime + dashTime)
            {
                controller.Move(move * dashSpeed * Time.deltaTime); // Move the player at dash speed
                yield return null; // Wait for the next frame
            }

            isDashing = false;
        }

        IEnumerator Wait1()
        {
            Debug.Log("before");
            yield return new WaitForSeconds(1);
            Debug.Log("after");
        }

        //Falling down
        velocity.y += gravity * Time.deltaTime;

        //Executing the jump
        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != transform.position)
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
