using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]

public class EnemyMovement : MonoBehaviour {

    // What to chase
    public Transform target;

    // How many times each second we will update our path
    public float updateRate = 2f;

    // Caching
    private Seeker seeker;
    private Rigidbody2D rb;

    // The calculated path
    public Path path;

    // The AI's speed per second
    float noSpeed = 0f;
    public float runSpeed = 1100f;
    public float walkSpeed = 700f;
    public float moveSpeed = 0f;

    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 2f;

    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private bool searchingForPlayer = false;
    public bool stillSearching = false;
    public bool facingRight = true;
    bool isAiming = false;
    public bool isRunning = false;
    public bool isWalking = false;
    bool onGround = false;
    public bool playerPositionKnown = false;
    public bool movingTowardsSound = false;

    public Transform groundCheck;
    float groundRadius = 0.1f;
    public LayerMask whatIsGround;

    float horizontalVelocity;

    float timer = 0f;

    public float continueSearchingTime = 15f;

    [HideInInspector]
    public Vector3 enemyLocation;

    Animator anim;
    SpriteRenderer[] childrenSpriteRenderer;

    Transform arm;

    Player player;
    PlayerController playerControllerScript;
    EnemySight enemySightScript;
    EnemyHearing enemyHearingScript;
    Enemy enemyScript;
    ArmRotation armRotationScript;

    GameObject[] enemies;
    Transform enemySight;
    Transform enemyHearing;

    GameObject[] stairwaysGoingUp;
    GameObject[] stairwaysGoingDown;

    BoxCollider2D thisCollider;
    BoxCollider2D enemyCollider;

    public int currentFloorLevel = 1;
    public int currentRoomNumber;
    public Transform currentRoom;

    public enum State
    {
        Dead = 0,
        Idle = 1,
        Patrol = 2,
        CheckSound = 3,
        Attack = 4
    }

    public State startState;
    public State currentState;
    public State defaultState = State.Idle;

    void Start()
    {
        arm = gameObject.transform.Find("EnemyArm");
        enemySight = gameObject.transform.Find("Sight");
        enemyHearing = gameObject.transform.Find("Hearing");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        thisCollider = gameObject.GetComponent<BoxCollider2D>();

        foreach (GameObject enemy in enemies)
        {
            enemyCollider = enemy.GetComponent<BoxCollider2D>();
        }

        Physics2D.IgnoreCollision(thisCollider, enemyCollider);

        stairwaysGoingUp = GameObject.FindGameObjectsWithTag("StairsUp");
        stairwaysGoingDown = GameObject.FindGameObjectsWithTag("StairsDown");

        currentState = startState;

        player = Player.instance;
        playerControllerScript = player.GetComponent<PlayerController>();
        enemySightScript = enemySight.GetComponent<EnemySight>();
        enemyHearingScript = enemyHearing.GetComponent<EnemyHearing>();
        enemyScript = gameObject.GetComponent<Enemy>();
        armRotationScript = arm.GetComponent<ArmRotation>();

        childrenSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();

        enemyLocation = transform.localScale;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (facingRight == false)
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            armRotationScript.enemyRotationOffset = 180;
            facingRight = false;
        }

        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
            }
            return;
        }

        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(MoveTowardsTheTarget());
    }

    void Update()
    {
        CheckCurrentStateUpdate();
    }

    void FixedUpdate()
    {
        // CheckCurrentStateFixedUpdate();
    }

    void CheckCurrentStateUpdate()
    {
        if (currentState == State.Dead)
        {
            CheckIfAiming();
            anim.SetInteger("currentState", 0);
            return;
        }
        else if (currentState == State.Idle)
        {
            CheckIfAiming();
            DetermineMoveSpeed();
            anim.SetInteger("currentState", 1);
        }
        else if (currentState == State.Patrol)
        {
            CheckIfAiming();
            Flip();
            anim.SetInteger("currentState", 2);
        }
        else if (currentState == State.CheckSound)
        {
            anim.SetInteger("currentState", 3);

            DetermineTarget();
            CheckIfAiming();
            CheckIfRunning();
            DetermineMoveSpeed();
            Flip();
            GroundCheck();
        }
        else if (currentState == State.Attack) // Attack state is determined in the EnemySenses script, when the player can be seen
        {
            anim.SetInteger("currentState", 4);
            target = player.transform;

            CheckIfShouldFollowPlayer();
            FlipDuringAttackState();
            DetermineMoveSpeed();
            CheckIfAiming();
            CheckIfRunning();
            GroundCheck();
        }
    }

    void CheckCurrentStateFixedUpdate()
    {
        if (currentState == State.CheckSound || currentState == State.Attack)
        {
            CalculateWaypointMovement();
        }
    }

    private void CheckIfShouldFollowPlayer()
    {
        if (enemyScript.isDead == false && player.isDead == false)
        {
            if ((target != null && player.isDead == true) || (target != null && enemySightScript.CanPlayerBeSeen() == false && stillSearching == false))
            {
                searchingForPlayer = false;
                playerPositionKnown = false;
                return;
            }
            else if ((target != null && stillSearching == true && enemySightScript.CanPlayerBeSeen() == false) || (target != null && stillSearching == false && enemySightScript.CanPlayerBeSeen() == true))
            {
                //if (!searchingForPlayer)
                //{
                    //searchingForPlayer = true;
                    playerPositionKnown = true;
                    MoveTowardsTarget();
                    // StartCoroutine(MoveTowardsTheTarget());
                //}
            }
            else if (target == null)
            {
                if (!searchingForPlayer)
                {
                    searchingForPlayer = true;
                    playerPositionKnown = false;
                }
                return;
            }
        }
    }

    void FlipDuringAttackState()
    {
        horizontalVelocity = rb.velocity.x;

        if (facingRight == true && horizontalVelocity < -1f) // Facing right and moving left
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            armRotationScript.enemyRotationOffset = 180;
            facingRight = false;
        }
        else if (facingRight == false && horizontalVelocity > 1f) // Facing left and moving right
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            armRotationScript.enemyRotationOffset = 360;
            facingRight = true;
        }
        else if (facingRight == true && transform.position.x > player.transform.position.x)
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            armRotationScript.enemyRotationOffset = 180;
            facingRight = false;
        }
        else if (facingRight == false && transform.position.x < player.transform.position.x)
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            armRotationScript.enemyRotationOffset = 360;
            facingRight = true;
        }
    }

    void Flip()
    {
        if (enemyScript.isDead == false)
        {
            horizontalVelocity = rb.velocity.x;

            if (facingRight == true && horizontalVelocity < -1f) // Facing right and moving left
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                armRotationScript.enemyRotationOffset = 180;
                facingRight = false;
            }
            else if (facingRight == false && horizontalVelocity > 1f) // Facing left and moving right
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                armRotationScript.enemyRotationOffset = 360;
                facingRight = true;
            }
        }
    }

    void CheckIfAiming()
    {
        if (currentState == State.Attack)
        {
            isAiming = true;
            playerPositionKnown = true;

            for (int i = 1; i < childrenSpriteRenderer.Length; ++i) // Enable Arm and Weapon sprite renderers if aiming...Start with i = 1 to skip the parent sprite (the enemy's body)
            {
                childrenSpriteRenderer[i].enabled = true;
            }

            anim.SetBool("isAiming", isAiming);
        }
        else
        {
            isAiming = false;
            playerPositionKnown = false;

            for (int i = 1; i < childrenSpriteRenderer.Length; ++i) // Disable Arm and Weapon sprite renderers if not aiming...Start with i = 1 to skip the parent sprite (the enemy's body)
            {
                childrenSpriteRenderer[i].enabled = false;
            }

            anim.SetBool("isAiming", isAiming);
        }
    }

    void CheckIfRunning()
    {
        if (enemyScript.distanceToPlayer > 3f && player.isDead == false && playerPositionKnown == true || currentState == State.CheckSound)
        {
            isRunning = true;
            anim.SetBool("isRunning", isRunning);
        }
        else if (enemyScript.distanceToPlayer < 3f || player.isDead == true)
        {
            isRunning = false;
            anim.SetBool("isRunning", isRunning);
        }
    }

    void CalculateWaypointMovement()
    {
        if (enemyScript.isDead == false)
        {
            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                if (pathIsEnded)
                    return;

                Debug.Log("End of path reached.");
                if (currentState == State.CheckSound && target == player.transform)
                {
                    currentState = defaultState;
                    isRunning = false;
                    anim.SetBool("isRunning", isRunning);
                }
                pathIsEnded = true;
                return;
            }
            pathIsEnded = false;

            // Direction to the next waypoint
            Vector2 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            dir *= moveSpeed * Time.fixedDeltaTime;

            // Move the AI
            rb.AddForce(dir, fMode);

            // Debug.Log("Dir: " + dir);

            float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (dist < nextWaypointDistance)
            {
                currentWaypoint++;
                return;
            }
        }
    }

    void DetermineMoveSpeed()
    {
        if (enemyScript.isDead == false)
        {
            if ((enemySightScript.CanPlayerBeSeen() == true && enemyScript.distanceToPlayer < 3f) || currentState == State.Idle)
            {
                moveSpeed = noSpeed;
                isWalking = false;
            }
            else if (isRunning == true)
            {
                moveSpeed = runSpeed;
                isWalking = false;
            }
            else
            {
                moveSpeed = walkSpeed;
                isWalking = true;
            }

            anim.SetFloat("moveSpeed", moveSpeed);
            anim.SetBool("isWalking", isWalking);
        }
    }

    void MoveTowardsTarget()
    {
        if (target == player.transform && Vector2.Distance(transform.position, target.position) > 3)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), 2 * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), 2 * Time.deltaTime);
        }
    }

    IEnumerator MoveTowardsTheTarget()
    {
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
            }
            yield return false;
        }

        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate); // Update path towards player after this amount of time
        StartCoroutine(MoveTowardsTheTarget());
    }

    void DetermineTarget()
    {
        if (enemyHearingScript.soundTriggerColliderRoom != null)
        {
            if (currentFloorLevel == enemyHearingScript.soundTriggerColliderFloorLevel)
            {
                target = enemyHearingScript.soundTriggerColliderRoom;

                MoveTowardsTarget();
                // StartCoroutine(MoveTowardsTheTarget());
            }
            else if (currentFloorLevel < enemyHearingScript.soundTriggerColliderFloorLevel)
            {
                Transform targetRoomName = enemyHearingScript.soundTriggerColliderRoom;
                target = targetRoomName.gameObject.GetComponent<Room>().nearestStairsUpTo.transform;

                MoveTowardsTarget();
                // StartCoroutine(MoveTowardsTheTarget());
            }
            else if (currentFloorLevel > enemyHearingScript.soundTriggerColliderFloorLevel)
            {
                Transform targetRoomName = enemyHearingScript.soundTriggerColliderRoom;
                target = targetRoomName.gameObject.GetComponent<Room>().nearestStairsDownTo.transform;

                MoveTowardsTarget();
                // StartCoroutine(MoveTowardsTheTarget());
            }
        }
    }

    public void OnPathComplete (Path p)
    {
        // Debug.Log("We got a path. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void GroundCheck()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("onGround", onGround);
    }

    IEnumerator OnTriggerExit2D(Collider2D other) 
    {
        if (currentState == State.Attack && other.name == "Player")
        {
            // If the player leaves the enemy's trigger, they will continue following the player for a set amount of time.
            stillSearching = true;

            while (timer < continueSearchingTime)
            {
                playerPositionKnown = true;
                yield return new WaitForSeconds(1f);
                timer++;
                // Debug.Log("Timer: " + timer);
            }

            if (timer >= continueSearchingTime && enemyScript.enemyStats.currentHealth > 0)
            {
                stillSearching = false;
                playerPositionKnown = false;
                currentState = defaultState;
            }
        }
    }

    void OnTriggerEnter2D()
    {
        timer = 0.0f;
    }
}