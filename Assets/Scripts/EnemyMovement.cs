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
    public float nextWaypointDistance = 1f;

    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private bool searchingForPlayer = false;
    public bool stillSearching = false;
    public bool facingRight = true;
    bool isAiming = false;
    bool isRunning = false;
    bool isWalking = false;
    bool onGround = false;
    public bool playerPositionKnown = false;

    public Transform groundCheck;
    float groundRadius = 0.1f;
    public LayerMask whatIsGround;

    float horizontalVelocity;

    float timer = 0f;

    public float continueSearchingTime = 15f;

    Vector3 enemyLocation;

    Animator anim;
    SpriteRenderer[] childSpriteRenderer;

    Player player;
    EnemySenses enemySenses;
    Enemy enemy;

    public static EnemyMovement instance;

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

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        currentState = startState;

        player = Player.instance;
        enemySenses = EnemySenses.instance;
        enemy = Enemy.instance;

        childSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();

        enemyLocation = transform.localScale;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
            }
            return;
        }

        if (facingRight == false)
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            EnemyArmRotation.rotationOffset = 180;
            facingRight = false;
        }

        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(RunTowardsPlayer());
    }

    void Update()
    {
        CheckCurrentState();
    }

    void CheckCurrentState()
    {
        if (currentState == State.Dead)
        {
            CheckIfAiming();
            anim.SetInteger("currentState", 0);
            return;
        }
        else if (currentState == State.Idle)
        {
            anim.SetInteger("currentState", 1);
        }
        else if (currentState == State.Patrol)
        {
            anim.SetInteger("currentState", 2);
        }
        else if (currentState == State.CheckSound)
        {
            anim.SetInteger("currentState", 3);
        }
        else if (currentState == State.Attack) // Attack state is determined in the EnemySenses script, when the player can be seen
        {
            anim.SetInteger("currentState", 4);

            CheckIfShouldFollowPlayer();
            FlipDuringAttackState();
            CalculateWaypointMovement();
            DetermineMoveSpeed();
            CheckIfAiming();
            CheckIfRunning();
            GroundCheck();
        }
    }

    private void CheckIfShouldFollowPlayer()
    {
        if (enemy.isDead == false)
        {
            if ((target != null && player.isDead == true) || (target != null && enemySenses.CanPlayerBeSeen() == false && stillSearching == false))
            {
                searchingForPlayer = false;
                playerPositionKnown = false;
                return;
            }
            else if ((target != null && stillSearching == true && enemySenses.CanPlayerBeSeen() == false) || (target != null && stillSearching == false && enemySenses.CanPlayerBeSeen() == true))
            {
                if (!searchingForPlayer)
                {
                    searchingForPlayer = true;
                    playerPositionKnown = true;
                    StartCoroutine(RunTowardsPlayer());
                }
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
        if (enemy.isDead == false && target == player.transform)
        {
            horizontalVelocity = rb.velocity.x;

            if (facingRight == true && horizontalVelocity < -1f) // Facing right and moving left
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                EnemyArmRotation.rotationOffset = 180;
                facingRight = false;
            }
            else if (facingRight == false && horizontalVelocity > 1f) // Facing left and moving right
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                EnemyArmRotation.rotationOffset = 360;
                facingRight = true;
            }
            else if (facingRight == true && transform.position.x > player.transform.position.x)
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                EnemyArmRotation.rotationOffset = 180;
                facingRight = false;
            }
            else if (facingRight == false && transform.position.x < player.transform.position.x)
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                EnemyArmRotation.rotationOffset = 360;
                facingRight = true;
            }
        }
    }

    void Flip()
    {
        if (enemy.isDead == false)
        {
            horizontalVelocity = rb.velocity.x;

            if (facingRight == true && horizontalVelocity < -1f) // Facing right and moving left
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                EnemyArmRotation.rotationOffset = 180;
                facingRight = false;
            }
            else if (facingRight == false && horizontalVelocity > 1f) // Facing left and moving right
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                EnemyArmRotation.rotationOffset = 360;
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

            for (int i = 1; i < childSpriteRenderer.Length; ++i) // Enable Arm and Weapon sprite renderers if aiming...Start with i = 1 to skip the parent sprite (the player's body)
            {
                childSpriteRenderer[i].enabled = true;
            }

            anim.SetBool("isAiming", isAiming);
        }
        else
        {
            isAiming = false;
            playerPositionKnown = false;

            for (int i = 1; i < childSpriteRenderer.Length; ++i) // Disable Arm and Weapon sprite renderers if not aiming...Start with i = 1 to skip the parent sprite (the player's body)
            {
                childSpriteRenderer[i].enabled = false;
            }

            anim.SetBool("isAiming", isAiming);
        }
    }

    void CheckIfRunning()
    {
        if (enemy.distanceToPlayer > 3f && ((enemySenses.CanPlayerBeSeen() == false && stillSearching == true) || enemySenses.CanPlayerBeSeen() == true))
        {
            isRunning = true;
            anim.SetBool("isRunning", isRunning);
        }
        else if (enemy.distanceToPlayer < 3f)
        {
            isRunning = false;
            anim.SetBool("isRunning", isRunning);
        }
    }

    void CalculateWaypointMovement()
    {
        if (enemy.isDead == false)
        {
            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                if (pathIsEnded)
                    return;

                // Debug.Log("End of path reached.");
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
        if (enemy.isDead == false)
        {
            if ((enemySenses.CanPlayerBeSeen() == true && enemy.distanceToPlayer < 3f) || (enemySenses.CanPlayerBeSeen() == false && stillSearching == false))
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

    IEnumerator RunTowardsPlayer()
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
        StartCoroutine(RunTowardsPlayer());
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

    IEnumerator OnTriggerExit2D() 
    {
        if (currentState == State.Attack)
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

            if (timer >= continueSearchingTime)
            {
                stillSearching = false;
                playerPositionKnown = false;
            }
        }
    }

    void OnTriggerEnter2D()
    {
        timer = 0.0f;
    }
}