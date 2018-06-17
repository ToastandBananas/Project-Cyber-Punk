using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 2.3f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpPower = 5f;

    float speedFactor = 2.3f;
    float moveSpeed;

    bool isRunning = false;
    bool onGround = false;
    public bool facingRight = true;
    public bool isAiming = false;

    public Transform groundCheck;
    float groundRadius = 0.1f;
    public LayerMask whatIsGround;

    SpriteRenderer playerSpriteRenderer;
    Animator playerAnim;
    Rigidbody2D rigidBody;
    public GameObject arm;

    void Start()
    {
        playerSpriteRenderer = GetComponent <SpriteRenderer>();
        playerAnim = gameObject.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.freezeRotation = true;
        arm = transform.Find("Arm").gameObject;
    }

    void Update()
    {
        Flip();
        CheckIfAiming();
    }

    void FixedUpdate()
    {
        MoveHorizontally();
        Jump();
        GetPlayerVelocity();
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
