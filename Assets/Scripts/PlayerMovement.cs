using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool onWall = false;
    private bool onStickyWall = false;
    private float lastTimeGrounded;
    private int jumpCounter;
    private bool jumping = false;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private Transform isGroundedChecker;
    [SerializeField]
    private Transform wallChecker;
    [SerializeField]
    private float groundRadius;
    [SerializeField]
    private float wallRadius;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private LayerMask stickyLayer;
    [SerializeField]
    private float rememberGroundedFor;
    [SerializeField]
    private float fallMultiplier = 2.5f;
    [SerializeField]
    private float lowJumpMultiplier = 2f;
    [SerializeField]
    private int defaultJumpCount = 1;
    [SerializeField]
    private CameraController cameraController;
    [SerializeField]
    private AudioClip jumpingSound;
    [SerializeField]
    private AudioClip walkingSound;
    [SerializeField]
    private AudioClip fallingSound;
    [SerializeField]
    private AudioClip impactSound;
    [SerializeField]
    private AudioSource audioSource;
    private bool isFalling = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!cameraController.collectibleDetected)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            MoveCharacter();
            Jump();
            AdjustFalling();
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        checkIfGrounded();
        checkOnWall();
    }

    private void MoveCharacter()
    {
        //left and right -> x-axis
        float x = Input.GetAxisRaw("Horizontal");
        float moveBy = x * movementSpeed;
        if (!onWall || (onWall && isGrounded))
        {
            if (!jumping)
            {
                if (Input.GetAxis("Horizontal") > 0)
                {
                    animator.Play("MoveBlobRight");
                    if (!audioSource.clip != walkingSound)
                    {
                        //audioSource.clip = walkingSound;
                        //audioSource.Play();
                    }
                }
                else if (Input.GetAxis("Horizontal") < 0)
                {
                    //audioSource.Pause();
                    animator.Play("MoveBlobLeft");
                    if (!audioSource.clip != walkingSound)
                    {
                        //audioSource.clip = walkingSound;
                        //audioSource.Play();
                    }
                }
                else
                {
                    animator.Play("IdleBlob");
                    audioSource.Stop();
                    //audioSource.clip = null;
                }
            }
            rb.velocity = new Vector2(moveBy, rb.velocity.y);                        
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (onStickyWall || isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || jumpCounter > 0))
        {
            jumping = true;
            audioSource.PlayOneShot(jumpingSound, 0.5f);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.Play("Jump");
            jumpCounter--;
        }
    }

    private void checkIfGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, groundRadius, groundLayer);

        if (collider != null)
        {
            if(isFalling)
            {
                //impact
                audioSource.PlayOneShot(impactSound, 0.5f);
                animator.Play("Landing");
                StartCoroutine(WaitAnimationOverAndDoThings());
                isFalling = false;               
            }
            isGrounded = true;
            jumpCounter = defaultJumpCount;
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }

    private void checkOnWall()
    {
        Collider2D collider = Physics2D.OverlapCircle(wallChecker.position, wallRadius, wallLayer);

        if (collider != null)
        {
            //Debug.Log("Wall!");
            onWall = true;
            jumpCounter = defaultJumpCount;
        }
        else
        {
            onWall = false;
        }

        Collider2D secondCollider = Physics2D.OverlapCircle(wallChecker.position, wallRadius, stickyLayer);
        
        if(secondCollider != null)
        {
            Debug.Log("Irgh, sticky");
            onStickyWall = true;
            jumpCounter = defaultJumpCount;
            isFalling = false;
        }
        else
        {
            onStickyWall = false;
        }
    }

    void AdjustFalling()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
            if (!isFalling && !isGrounded)
            {
                audioSource.PlayOneShot(fallingSound, 0.5f);
                animator.Play("Midair");
                isFalling = true;
            }
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private IEnumerator WaitAnimationOverAndDoThings()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.5f);
        jumping = false;
    }

}
