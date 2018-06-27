using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 2.3f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpPower = 7f;
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

    Player player;
    
    SpriteRenderer[] childSpriteRenderer;

    Vector3 playerLocation;

    AudioManager audioManager;

    void Start()
    {
        player = Player.instance;

        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerAnim = gameObject.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.freezeRotation = true;

        childSpriteRenderer = GetComponentsInChildren<SpriteRenderer>(); //(typeof(SpriteRenderer)) as SpriteRenderer;
        
        playerLocation = transform.localScale;

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, whatIsGround);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                onGround = true;
                playerAnim.SetBool("onGround", onGround);
            }
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

        if (onGround && Input.GetButtonDown("Jump") && player.isDead == false)
        {
            rigidBody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    void MoveHorizontally()
    {
        if (player.isDead == false)
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
        }
    }

    void Flip()
    {
        if (playerSpriteRenderer != null && player.isDead == false)
        {
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && facingRight == true)
            {
                playerLocation.x *= -1;
                transform.localScale = playerLocation;
                ArmRotation.rotationOffset = 180;
                facingRight = false;
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && facingRight == false)

            {
                playerLocation.x *= -1;
                transform.localScale = playerLocation;
                ArmRotation.rotationOffset = 360;
                facingRight = true;
            }
        }
    }

    void CheckIfAiming()
    {
        if (Input.GetButton("Fire2") && player.isDead == false) // Holding right click
        {
            isAiming = true;

            int i;
            for (i = 1; i < childSpriteRenderer.Length; ++i) // Enable Arm and Weapon sprite renderers if aiming...Start with i = 1 to skip the parent sprite (the player's body)
            {
                childSpriteRenderer[i].enabled = true;
            }

            playerAnim.SetBool("isAiming", isAiming);
        }
        else
        {
            isAiming = false;

            int i;
            for (i = 1; i < childSpriteRenderer.Length; ++i) // Disable Arm and Weapon sprite renderers if not aiming...Start with i = 1 to skip the parent sprite (the player's body)
            {
                childSpriteRenderer[i].enabled = false;
            }

            playerAnim.SetBool("isAiming", isAiming);
        }
    }
}
