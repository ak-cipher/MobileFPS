using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;


    public RigidbodyFirstPersonController rigidbodyFirstPersonController;


    public FixedTouchField fixedTouch;

    public Animator animator;




    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool isGrounded;
    public Vector3 velocity;
    public Rigidbody rb;

    //camera rotation
    public GameObject cam;
    [SerializeField]
    float cameraSenstivity = 5f;

    //Player Body rotation
    float xRotation;
    [SerializeField]
    float mouseSenstivity = 1f;



    private void Start()
    {
        rigidbodyFirstPersonController = this.gameObject.GetComponent<RigidbodyFirstPersonController>();
        animator = GetComponent<Animator>();
    }

    
    private void FixedUpdate()
    {
        // Player Movement
        rigidbodyFirstPersonController.joystickInput.x = joystick.Horizontal;
        rigidbodyFirstPersonController.joystickInput.y = joystick.Vertical;

        //Animator
        animator.SetFloat("Horizontal", joystick.Horizontal);
        animator.SetFloat("Vertical", joystick.Vertical);

        if(Mathf.Abs(joystick.Horizontal) > 0.9f || Mathf.Abs(joystick.Vertical) > 0.9f)
        {
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 6f;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 4f;
            animator.SetBool("IsRunning", false);
        }
        

        float mouseX = 0;
        float mouseY = 0;

        mouseX = fixedTouch.TouchDist.x;
        mouseY = fixedTouch.TouchDist.y;

        mouseX *= mouseSenstivity;
        mouseY *= cameraSenstivity;

        //player left right rotation
        Vector3 rotationVector = new Vector3(0, mouseX, 0);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationVector));
        //this.gameObject.transform.Rotate(rotationVector);


        // clamping the up down rotation and applying limit 
        xRotation -= mouseY * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -70, 70);

        // rotatin the camera if there is a camera object in the player prefab
        if (cam != null)
        {

            cam.transform.localEulerAngles = new Vector3(xRotation, 0, 0);
        }
    }



    
    



}


