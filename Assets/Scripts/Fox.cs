using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    public Collider2D standingCollider,crouchingCollider;
    public Transform groundCheckCollider;
    public Transform overheadCheckCollider;
    public LayerMask groundLayer;

    const float groundCheckRadius = 0.2f;
    const float overheadCheckRadius = 0.2f;
    [SerializeField] float speed = 2;
    [SerializeField] float jumpPower =500;
    public int totalJumps;
    int availableJumps;
    float horizontalValue;
    float runSpeedModifier = 2f;
    float crouchSpeedModifier = 0.5f;
    
    bool isGrounded=true;    
    bool isRunning;
    bool facingRight = true;
    bool crouchPressed;
    bool multipleJump;
    bool coyoteJump;

    void Awake()
    {
        availableJumps = totalJumps;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (CanMove()==false)
            return;

        //Store the horizontal value
        horizontalValue = Input.GetAxisRaw("Horizontal");

        //If LShift is clicked enable isRunning
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRunning = true;
        //If LShift is released disable isRunning
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;

        //If we press Jump button enable jump 
        if (Input.GetButtonDown("Jump"))
            Jump();

        //If we press Crouch button enable crouch 
        if (Input.GetButtonDown("Crouch"))
            crouchPressed = true;
        //Otherwise disable it
        else if (Input.GetButtonUp("Crouch"))
            crouchPressed = false;

        //Set the yVelocity Value
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalValue, crouchPressed);        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckCollider.position, groundCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(overheadCheckCollider.position, overheadCheckRadius);
    }

    bool CanMove()
    {
        bool can = true;

        if (FindObjectOfType<InteractionSystem>().isExamining)
            can = false;
        if (FindObjectOfType<InventorySystem>().isOpen)
            can = false;

        return can;
    }

    void GroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;
        //Check if the GroundCheckObject is colliding with other
        //2D Colliders that are in the "Ground" Layer
        //If yes (isGrounded true) else (isGrounded false)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;
            if (!wasGrounded)
            {
                availableJumps = totalJumps;
                multipleJump = false;

                AudioManager.instance.PlaySFX("landing");
            }                
        }    
        else
        {
            if (wasGrounded)
                StartCoroutine(CoyoteJumpDelay());
        }

        //As long as we are grounded the "Jump" bool
        //in the animator is disabled
        animator.SetBool("Jump", !isGrounded);
    }

    #region Jump
    IEnumerator CoyoteJumpDelay()
    {
        coyoteJump = true;
        yield return new WaitForSeconds(0.2f);
        coyoteJump = false;
    }

    void Jump()
    {
        if (isGrounded)
        {
            multipleJump = true;
            availableJumps--;

            rb.velocity = Vector2.up * jumpPower;
            animator.SetBool("Jump", true);
        }
        else
        {
            if(coyoteJump)
            {
                multipleJump = true;
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                animator.SetBool("Jump", true);
            }

            if(multipleJump && availableJumps>0)
            {
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                animator.SetBool("Jump", true);
            }
        }
    }
    #endregion

    void Move(float dir,bool crouchFlag)
    {
        #region Crouch

        //If we are crouching and disabled crouching
        //Check overhead for collision with Ground items
        //If there are any, remain crouched, otherwise un-crouch
        if(!crouchFlag)
        {
            if(Physics2D.OverlapCircle(overheadCheckCollider.position,overheadCheckRadius,groundLayer))
                crouchFlag = true;
        }

        animator.SetBool("Crouch", crouchFlag);
        standingCollider.enabled = !crouchFlag;
        crouchingCollider.enabled = crouchFlag;

        #endregion

        #region Move & Run
        //Set value of x using dir and speed
        float xVal = dir * speed * 100 * Time.fixedDeltaTime;
        //If we are running mulitply with the running modifier (higher)
        if (isRunning)
            xVal *= runSpeedModifier;
        //If we are running mulitply with the running modifier (higher)
        if (crouchFlag)
            xVal *= crouchSpeedModifier;
        //Create Vec2 for the velocity
        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        //Set the player's velocity
        rb.velocity = targetVelocity;
 
        //If looking right and clicked left (flip to the left)
        if(facingRight && dir < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
        //If looking left and clicked right (flip to rhe right)
        else if(!facingRight && dir > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = true;
        }

        //(0 idle , 4 walking , 8 running)
        //Set the float xVelocity according to the x value 
        //of the RigidBody2D velocity 
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion
    }    
}
