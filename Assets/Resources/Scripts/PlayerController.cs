using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] float walkSpeed = 2.3f;
    [SerializeField] float runSpeed = 4f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] string landingSoundName = "LandingFootsteps";

    float speedFactor = 2.3f;
    float moveSpeed;

    bool isRunning = false;
    bool onGround = false;
    public bool facingRight = true;
    public bool isAiming = false;
    private bool inAir = false;
    public bool isTeleporting = false;
    public bool isReappearing = false;

    public Transform groundCheck;
    float groundRadius = 0.1f;
    public LayerMask whatIsGround;

    public int currentFloorLevel;
    public int currentRoomNumber;
    public int currentBuildingNumber;
    public Transform currentRoom;

    SpriteRenderer playerSpriteRenderer;
    [HideInInspector] public Animator playerAnim;
    Rigidbody2D rigidBody;

    Player player;
    Transform arm;
    ArmRotation armRotationScript;

    GameObject[] patrolPoints;
    GameObject[] lights;

    public static PlayerController instance;
    
    SpriteRenderer[] childSpriteRenderer;

    Vector3 playerLocation;

    AudioManager audioManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        player = Player.instance;

        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        foreach (GameObject patrolPoint in patrolPoints)
        {
            BoxCollider2D patrolPointCollider = patrolPoint.gameObject.GetComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(player.GetComponent<CapsuleCollider2D>(), patrolPointCollider);
        }

        lights = GameObject.FindGameObjectsWithTag("Light");
        foreach (GameObject light in lights)
        {
            Physics2D.IgnoreCollision(player.GetComponent<CapsuleCollider2D>(), light.GetComponent<BoxCollider2D>());
        }

        arm = gameObject.transform.Find("Arm");
        armRotationScript = arm.GetComponent<ArmRotation>();

        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerAnim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.freezeRotation = true;

        childSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        playerLocation = transform.localScale;

        currentFloorLevel = 1;
        currentRoomNumber = 0;
        currentRoom = GameObject.Find("OutsideLeftCollider").transform;

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
        childSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        PlayLandingSoundUponLanding();
        Jump();
        GetPlayerVerticalVelocity();
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

    void GetPlayerVerticalVelocity()
    {
        playerAnim.SetFloat("verticalSpeed", rigidBody.velocity.y);
    }

    void Jump()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        playerAnim.SetBool("onGround", onGround);

        if (onGround && Input.GetButtonDown("Jump") && player.isDead == false && !isTeleporting && !isReappearing)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpPower);
        }
    }

    void MoveHorizontally()
    {
        if (player.isDead == false && !isTeleporting && !isReappearing)
        {
            float x = Input.GetAxisRaw("Horizontal");
            
            if (onGround == true)
            {
                speedFactor = runSpeed;
                isRunning = true;
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
        if (playerSpriteRenderer != null && player.isDead == false && !isTeleporting && !isReappearing)
        {
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && facingRight == true)
            {
                playerLocation.x *= -1;
                transform.localScale = playerLocation;
                armRotationScript.playerRotationOffset = 180;
                facingRight = false;
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && facingRight == false)

            {
                playerLocation.x *= -1;
                transform.localScale = playerLocation;
                armRotationScript.playerRotationOffset = 360;
                facingRight = true;
            }
        }
    }

    void CheckIfAiming()
    {
        if (Input.GetButton("Fire2") && player.isDead == false && !isTeleporting && !isReappearing) // Holding right click
        {
            isAiming = true;

            for (int i = 1; i < childSpriteRenderer.Length; ++i) // Enable Arm and Weapon sprite renderers if aiming...Start with i = 1 to skip the parent sprite (the player's body)
            {
                if (childSpriteRenderer[i] != null)
                {
                    childSpriteRenderer[i].enabled = true;
                }
            }

            playerAnim.SetBool("isAiming", isAiming);
        }
        else
        {
            isAiming = false;

            for (int i = 1; i < childSpriteRenderer.Length; ++i) // Disable Arm and Weapon sprite renderers if not aiming...Start with i = 1 to skip the parent sprite (the player's body)
            {
                if (childSpriteRenderer[i] != null)
                {
                    childSpriteRenderer[i].enabled = false;
                }
            }

            playerAnim.SetBool("isAiming", isAiming);
        }
    }
}
