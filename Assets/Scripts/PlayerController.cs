using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 2.3f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] string landingSoundName = "LandingFootsteps";

    float speedFactor = 2.3f;
    float moveSpeed;

    bool isRunning = false;
    bool onGround = false;
    public bool facingRight = true;
    public bool isAiming = false;
    private bool inAir = false;

    public Transform groundCheck;
    float groundRadius = 0.1f;
    public LayerMask whatIsGround;

    SpriteRenderer playerSpriteRenderer;
    Animator playerAnim;
    Rigidbody2D rigidBody;
    public GameObject arm;

    AudioManager audioManager;

    void Start()
    {
        playerSpriteRenderer = GetComponent <SpriteRenderer>();
        playerAnim = gameObject.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.freezeRotation = true;

        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("No audio manager found.");
        }
    }

    void Update()
    {
        Flip();
        CheckIfAiming();
        MoveHorizontally();
    }

    void FixedUpdate()
    {
        PlayLandingSoundUponLanding();
        Jump();
        GetPlayerVelocity();
    }

    private void PlayLandingSoundUponLanding()
    {
        //bool wasGrounded = onGround;

        /*if (wasGrounded != onGround && onGround == true)
        {
            audioManager.PlaySound(landingSoundName);
        }*/

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, whatIsGround);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                onGround = true;
        }

        if (!onGround && !inAir)
        {
            inAir = true;
        }

        if (onGround && inAir)
        {
            inAir = false;
            audioManager.PlaySound(landingSoundName);
        }
    }

    void GetPlayerVelocity()
    {
        playerAnim.SetFloat("verticalSpeed", rigidBody.velocity.y);
    }

    void Jump()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        playerAnim.SetBool("onGround", onGround);

        if (onGround && Input.GetButtonDown("Jump"))
        {
            rigidBody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    void MoveHorizontally()
    {
        float x = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift) && onGround == true)
        {
            speedFactor = runSpeed;
            isRunning = true;
            playerAnim.SetBool("isRunning", isRunning);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speedFactor = walkSpeed;
            isRunning = false;
            playerAnim.SetBool("isRunning", isRunning);
        }

        moveSpeed = x * speedFactor;
        Vector2 move = new Vector2(moveSpeed, rigidBody.velocity.y);
        rigidBody.velocity = move;
        playerAnim.SetFloat("moveSpeed", Mathf.Abs(moveSpeed));

        //moveSpeed = move * Time.deltaTime * speedFactor;
        //transform.Translate(moveSpeed, 0, 0);
        //playerAnim.SetFloat("moveSpeed", Mathf.Abs(moveSpeed));

        //if (x < 0 && facingRight) Flip();
        //if (x > 0 && !facingRight) Flip();
    }

    void Flip()
    {
        //facingRight = !facingRight;

        if (playerSpriteRenderer != null)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                playerSpriteRenderer.flipX = true;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))

            {
                playerSpriteRenderer.flipX = false;
            }
        }
    }

    void CheckIfAiming()
    {
        if (Input.GetButton("Fire2")) // Holding right click
        {
            arm.SetActive(true);
            isAiming = true;
            playerAnim.SetBool("isAiming", isAiming);
        }
        else
        {
            arm.SetActive(false);
            isAiming = false;
            playerAnim.SetBool("isAiming", isAiming);
        }
    }
}
