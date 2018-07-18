﻿using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]

public class EnemyMovement : MonoBehaviour {

    // What to chase
    public Transform currentTarget;

    // How many times each second we will update our path
    public float updateRate = 2f;

    // Caching
    private Seeker seeker;
    private Rigidbody2D rb;

    // The calculated path
    public Path path;

    // The AI's speed per second
    float noSpeed = 0f;
    public float runSpeed = 3f;
    public float walkSpeed = 2f;
    public float moveSpeed = 0f;

    public float stoppingDistance = 3f;

    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    float nextWaypointDistance = 2f;

    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private bool searchingForPlayer = false;
    [HideInInspector]
    public bool stillSearching = false;
    public bool facingRight = true;
    bool isAiming = false;
    bool isRunning = false;
    bool isWalking = false;
    bool onGround = false;
    [HideInInspector]
    public bool playerPositionKnown = false;
    bool movingTowardsSound = false;

    public Transform groundCheck;
    float groundRadius = 0.1f;
    public LayerMask whatIsGround;

    float horizontalVelocity;

    float timer = 0f;

    public float continueSearchingTime = 5f;

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

    GameObject[] patrolPoints;
    public Transform[] enemyPatrolPoints;
    public Transform currentPatrolPoint;
    public int currentPatrolIndex;

    CapsuleCollider2D thisCollider;
    CapsuleCollider2D enemyCollider;

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

        thisCollider = gameObject.GetComponent<CapsuleCollider2D>();

        foreach (GameObject enemy in enemies)
        {
            enemyCollider = enemy.GetComponent<CapsuleCollider2D>();
        }

        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        foreach (GameObject patrolPoint in patrolPoints)
        {
            BoxCollider2D patrolPointCollider = patrolPoint.gameObject.GetComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(gameObject.GetComponent<CapsuleCollider2D>(), patrolPointCollider);
        }

        currentPatrolIndex = 0;
        currentPatrolPoint = enemyPatrolPoints[currentPatrolIndex];

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
    }

    void Update()
    {
        if (currentState == State.Patrol)
        {
            PatrolWaypoints();
        }
        else if (currentState == State.CheckSound)
        {
            DetermineTargetCheckSoundState();
        }
        else if (currentState == State.Attack)
        {
            DetermineTargetAttackState();
        }
    }

    void FixedUpdate()
    {

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Enemy>().isDead)
            {
                Physics2D.IgnoreCollision(thisCollider, enemyCollider);
            }
            else if (!facingRight && enemy.GetComponent<EnemyMovement>().facingRight)
            {
                Physics2D.IgnoreCollision(thisCollider, enemyCollider);
            }
            else if (facingRight && enemy.GetComponent<EnemyMovement>().facingRight == false)
            {
                Physics2D.IgnoreCollision(thisCollider, enemyCollider);
            }
            else
            {
                Physics2D.IgnoreCollision(thisCollider, enemyCollider, false);
            }
        }

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
            anim.SetInteger("currentState", 2);

            CheckIfAiming();
            DetermineMoveSpeed();
            Flip();
            GroundCheck();
        }
        else if (currentState == State.CheckSound)
        {
            anim.SetInteger("currentState", 3);

            CheckIfAiming();
            DetermineMoveSpeed();
            Flip();
            GroundCheck();
        }
        else if (currentState == State.Attack) // Attack state is determined in the EnemySenses script, when the player can be seen
        {
            anim.SetInteger("currentState", 4);

            CheckIfAiming();
            DetermineMoveSpeed();
            Flip();
            GroundCheck();
        }
    }

    void Flip()
    {
        if (currentTarget != null)
        {
            if (facingRight == true && transform.position.x > currentTarget.transform.position.x)
            {
                enemyLocation.x *= -1;
                transform.localScale = enemyLocation;
                armRotationScript.enemyRotationOffset = 180;
                facingRight = false;
            }
            else if (facingRight == false && transform.position.x < currentTarget.transform.position.x)
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

    void DetermineMoveSpeed()
    {
        if (enemyScript.isDead == false)
        {
            if ((enemySightScript.CanPlayerBeSeen() == true && enemyScript.distanceToPlayer < stoppingDistance) || currentState == State.Idle)
            {
                moveSpeed = noSpeed;
                isRunning = false;
                isWalking = false;
            }
            else if (enemyScript.distanceToPlayer > stoppingDistance && player.isDead == false && playerPositionKnown == true || playerPositionKnown == true && enemySightScript.CanPlayerBeSeen() == false || currentState == State.CheckSound)
            {
                moveSpeed = runSpeed;
                isRunning = true;
                isWalking = false;
            }
            else
            {
                moveSpeed = walkSpeed;
                isRunning = false;
                isWalking = true;
            }

            anim.SetFloat("moveSpeed", moveSpeed);
            anim.SetBool("isRunning", isRunning);
            anim.SetBool("isWalking", isWalking);
        }
    }

    void MoveTowardsTarget()
    {
        if (currentTarget == player.transform && (Vector2.Distance(transform.position, currentTarget.position) > stoppingDistance || (enemySightScript.CanPlayerBeSeen() == false && playerPositionKnown == true)))
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, transform.position.y), moveSpeed * Time.fixedDeltaTime);
        }
        else if (currentTarget != player.transform)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, transform.position.y), moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void PatrolWaypoints()
    {
        if (Vector3.Distance (transform.position, currentPatrolPoint.position) < .1f) // Cycles through only the patrol points that are assigned to each enemy in the inspector
        {
            if (currentPatrolIndex + 1 < enemyPatrolPoints.Length)
            {
                currentPatrolIndex++;
            }
            else
            {
                currentPatrolIndex = 0;
            }
            currentPatrolPoint = enemyPatrolPoints[currentPatrolIndex]; 
        }

        if (currentFloorLevel == currentPatrolPoint.GetComponent<PatrolPoint>().currentFloorLevel)
        {
            currentTarget = currentPatrolPoint;

            MoveTowardsTarget();
        }
        else if (currentPatrolPoint.GetComponent<PatrolPoint>().currentRoom == null)
        {
            Transform targetRoomName = currentPatrolPoint.GetComponent<PatrolPoint>().nearestRoom;
            if (currentFloorLevel - 1 > 1)
            {
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsDownTo.transform;
            }
            else if (currentFloorLevel - 1 == 1)
            {
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsDownTo.transform;
            }
            else if (currentFloorLevel + 1 < 0)
            {
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsUpTo.transform;
            }
            else if (currentFloorLevel + 1 == 0)
            {
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsUpTo.transform;
            }

            MoveTowardsTarget();
        }
        else if (currentFloorLevel + 1 == currentPatrolPoint.GetComponent<PatrolPoint>().currentFloorLevel) // If sound is one level above enemy
        {
            Transform targetRoomName = currentPatrolPoint.GetComponent<PatrolPoint>().currentRoom;
            currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsUpTo.transform;

            MoveTowardsTarget();
        }
        else if (currentFloorLevel - 1 == currentPatrolPoint.GetComponent<PatrolPoint>().currentFloorLevel) // If sound is one level below enemy
        {
            Transform targetRoomName = currentPatrolPoint.GetComponent<PatrolPoint>().currentRoom;
            currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsDownTo.transform;

            MoveTowardsTarget();
        }
        else if (currentFloorLevel + 2 == currentPatrolPoint.GetComponent<PatrolPoint>().currentFloorLevel) // If sound is two levels above enemy
        {
            Transform targetRoomName = currentPatrolPoint.GetComponent<PatrolPoint>().currentRoom;
            currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsUpTo.transform;

            MoveTowardsTarget();
        }
        else if (currentFloorLevel - 2 == currentPatrolPoint.GetComponent<PatrolPoint>().currentFloorLevel) // If sound is two levels below enemy
        {
            Transform targetRoomName = currentPatrolPoint.GetComponent<PatrolPoint>().currentRoom;
            currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsDownTo.transform;

            MoveTowardsTarget();
        }

    }

    private void DetermineTargetCheckSoundState()
    {
        if (enemyHearingScript.soundTriggerColliderRoom != null)
        {
            if (currentFloorLevel == enemyHearingScript.soundTriggerColliderFloorLevel)
            {
                currentTarget = enemyHearingScript.soundTriggerColliderRoom;

                MoveTowardsTarget();
            }
            else if (enemyHearingScript.soundTriggerColliderRoomNumber == 0)
            {
                Transform targetRoomName = enemyHearingScript.soundTriggerColliderRoom.gameObject.GetComponent<Room>().nearestRoom;
                if (currentFloorLevel - 1 > 1)
                {
                    currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsDownTo.transform;
                }
                else if (currentFloorLevel - 1 == 1)
                {
                    currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsDownTo.transform;
                }
                else if (currentFloorLevel + 1 < 0)
                {
                    currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsUpTo.transform;
                }
                else if (currentFloorLevel + 1 == 0)
                {
                    currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsUpTo.transform;
                }

                MoveTowardsTarget();
            }
            else if (currentFloorLevel + 1 == enemyHearingScript.soundTriggerColliderFloorLevel) // If sound is one level above enemy
            {
                Transform targetRoomName = enemyHearingScript.soundTriggerColliderRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsUpTo.transform;

                MoveTowardsTarget();
            }
            else if (currentFloorLevel - 1 == enemyHearingScript.soundTriggerColliderFloorLevel) // If sound is one level below enemy
            {
                Transform targetRoomName = enemyHearingScript.soundTriggerColliderRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsDownTo.transform;

                MoveTowardsTarget();
            }
            else if (currentFloorLevel + 2 == enemyHearingScript.soundTriggerColliderFloorLevel) // If sound is two levels above enemy
            {
                Transform targetRoomName = enemyHearingScript.soundTriggerColliderRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsUpTo.transform;

                MoveTowardsTarget();
            }
            else if (currentFloorLevel - 2 == enemyHearingScript.soundTriggerColliderFloorLevel) // If sound is two levels below enemy
            {
                Transform targetRoomName = enemyHearingScript.soundTriggerColliderRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsDownTo.transform;

                MoveTowardsTarget();
            }
        }
    }

    private void DetermineTargetAttackState()
    {
        if (enemyScript.isDead == false && player.isDead == false)
        {
            if ((enemySightScript.CanPlayerBeSeen() == true || playerPositionKnown == true) && currentFloorLevel == playerControllerScript.currentFloorLevel)
            {
                currentTarget = player.transform;

                MoveTowardsTarget();
            }
            else if (playerControllerScript.currentRoomNumber == 0) // Room 0 means the player is outside
            {
                Transform targetRoomName = playerControllerScript.currentRoom.gameObject.GetComponent<Room>().nearestRoom;
                if (currentFloorLevel - 1 > 1)
                {
                    currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsDownTo.transform;
                }
                else if (currentFloorLevel - 1 == 1)
                {
                    currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsDownTo.transform;
                }
                else if (currentFloorLevel + 1 < 0)
                {
                    currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsUpTo.transform;
                }
                else if (currentFloorLevel + 1 == 0)
                {
                    currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsUpTo.transform;
                }

                MoveTowardsTarget();
            }
            else if (currentFloorLevel + 1 == playerControllerScript.currentFloorLevel) // If player is one level above enemy
            {
                Transform targetRoomName = playerControllerScript.currentRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsUpTo.transform;

                MoveTowardsTarget();
            }
            else if (currentFloorLevel - 1 == playerControllerScript.currentFloorLevel) // If player is one level below enemy
            {
                Transform targetRoomName = playerControllerScript.currentRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsDownTo.transform;

                MoveTowardsTarget();
            }
            else if (currentFloorLevel + 2 == playerControllerScript.currentFloorLevel) // If player is two levels above enemy
            {
                Transform targetRoomName = playerControllerScript.currentRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsUpTo.transform;

                MoveTowardsTarget();
            }
            else if (currentFloorLevel - 2 == playerControllerScript.currentFloorLevel) // If player is two levels below enemy
            {
                Transform targetRoomName = playerControllerScript.currentRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsDownTo.transform;

                MoveTowardsTarget();
            }
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