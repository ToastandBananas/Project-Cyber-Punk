using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]

public class EnemyMovement : MonoBehaviour {

    // What to chase
    public Transform currentTarget;

    // How many times each second we will update our path
    public float updateRate = 2f;

    // Caching
    //private Seeker seeker;
    private Rigidbody2D rb;

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
    [HideInInspector] public bool stillSearching = false;
    public bool facingRight = true;
    bool isAiming = false;
    bool isRunning = false;
    bool isWalking = false;
    bool onGround = false;

    [HideInInspector] public bool playerPositionKnown = false;
    bool movingTowardsSound = false;

    public Transform groundCheck;
    float groundRadius = 0.1f;
    public LayerMask whatIsGround;

    float horizontalVelocity;

    float timer = 0f;

    public float continueSearchingForPlayerTime = 5f;

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

    BoxCollider2D[] playerWeapons;

    public GameObject[] rooms;
    public GameObject patrolPointPrefab;
    public GameObject newTarget;
    GameObject randomRoom;
    GameObject patrolPointContainer;
    
    public Transform[] enemyPatrolPoints;
    public Transform currentPatrolPoint;
    public int currentPatrolIndex;

    CapsuleCollider2D thisEnemyCollider;

    public int currentFloorLevel = 1;
    public int currentRoomNumber;
    public Transform currentRoom;

    float alertStateTimer;
    float alertStateTimerLength = 15.0f;

    public bool mayBeAlert = true;
    public float mayBeAlertTimer;
    float mayBeAlertTimerLength = 30.0f;

    public enum State
    {
        Dead = 0,
        Idle = 1,
        Patrol = 2,
        CheckSound = 3,
        Attack = 4,
        Alert = 5,
        SpreadOut = 6
    }

    public State startState;
    public State currentState;
    public State defaultState = State.Idle;

    void Start()
    {
        alertStateTimer = alertStateTimerLength;
        mayBeAlertTimer = mayBeAlertTimerLength;

        arm = gameObject.transform.Find("EnemyArm");
        enemySight = gameObject.transform.Find("Sight");
        enemyHearing = gameObject.transform.Find("Hearing");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        thisEnemyCollider = gameObject.GetComponent<CapsuleCollider2D>();

        rooms = GameObject.FindGameObjectsWithTag("Room");
        patrolPointContainer = GameObject.Find("PatrolPoints");

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
        
        anim = GetComponent<Animator>();

        enemyLocation = transform.localScale;
        
        rb = GetComponent<Rigidbody2D>();

        if (facingRight == false)
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            armRotationScript.enemyRotationOffset = 180;
            facingRight = false;
        }

        playerWeapons = GameObject.Find("WeaponPool").transform.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D playerWeaponCollider in playerWeapons)
        {
            Physics2D.IgnoreCollision(playerWeaponCollider, thisEnemyCollider);
        }
    }

    void Update()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy.gameObject != gameObject)
            {
                if (enemy.GetComponent<Enemy>().isDead == false && ((facingRight && enemy.GetComponent<EnemyMovement>().facingRight) || (!facingRight && enemy.GetComponent<EnemyMovement>().facingRight == false)))
                {
                    Physics2D.IgnoreCollision(thisEnemyCollider, enemy.GetComponent<CapsuleCollider2D>(), false); // Turn on colliders when they are the same direction, so that they don't overlap
                }
                else
                {
                    Physics2D.IgnoreCollision(thisEnemyCollider, enemy.GetComponent<CapsuleCollider2D>()); // Prevent enemies from colliding when moving in opposite direction or when dead
                }
            }
        }

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
        else if (currentState == State.Alert)
        {
            SearchRandomly();
        }
        else if (currentState == State.SpreadOut)
        {
            SpreadOut();
        }

        if (mayBeAlert == false)
        {
            mayBeAlertTimer -= Time.deltaTime;

            if (mayBeAlertTimer <= 0.0f)
            {
                mayBeAlert = true;
                mayBeAlertTimer = mayBeAlertTimerLength;
            }
        }
    }

    void FixedUpdate()
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
        else if (currentState == State.Alert)
        {
            anim.SetInteger("currentState", 5);

            CheckIfAiming();
            DetermineMoveSpeed();
            Flip();
            GroundCheck();
        }
        else if (currentState == State.SpreadOut)
        {
            anim.SetInteger("currentState", 6);

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
        if (currentState == State.Attack || currentState == State.Alert || currentState == State.CheckSound || currentState == State.SpreadOut)
        {
            isAiming = true;

            if (currentState == State.Attack)
            {
                playerPositionKnown = true;
            }

            if (childrenSpriteRenderer != null)
            {
                for (int i = 1; i < childrenSpriteRenderer.Length; ++i) // Enable Arm and Weapon sprite renderers if aiming...Start with i = 1 to skip the parent sprite (the enemy's body)
                {
                    childrenSpriteRenderer[i].enabled = true;
                }
            }
            else
            {
                childrenSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();
            }

            anim.SetBool("isAiming", isAiming);
        }
        else
        {
            isAiming = false;
            playerPositionKnown = false;

            if (childrenSpriteRenderer != null)
            {
                for (int i = 1; i < childrenSpriteRenderer.Length; ++i) // Disable Arm and Weapon sprite renderers if not aiming...Start with i = 1 to skip the parent sprite (the enemy's body)
                {
                    childrenSpriteRenderer[i].enabled = false;
                }
            }
            else
            {
                childrenSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();
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
            else if (enemyScript.distanceToPlayer > stoppingDistance && player.isDead == false && playerPositionKnown == true 
                        || playerPositionKnown == true && enemySightScript.CanPlayerBeSeen() == false 
                        || currentState == State.CheckSound || currentState == State.Alert || currentState == State.SpreadOut)
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

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f) // If enemy makes it to its target while in Alert or CheckSound state
        {
            if (currentState == State.Alert)
            {
                randomRoom = rooms[UnityEngine.Random.Range(1, rooms.Length)];
                while (randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel)
                {
                    randomRoom = rooms[UnityEngine.Random.Range(0, rooms.Length)];
                }
                newTarget.transform.position = new Vector3(randomRoom.transform.position.x + UnityEngine.Random.Range(-randomRoom.GetComponent<SpriteRenderer>().bounds.size.x / 2, randomRoom.GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y);
            }
            else if (currentState == State.CheckSound && currentTarget == newTarget.transform)
            {
                Destroy(newTarget);
                currentState = State.Alert;
            }
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
            if (newTarget == null)
            {
                newTarget = Instantiate(patrolPointPrefab, patrolPointContainer.transform);
            }

            if (currentFloorLevel == enemyHearingScript.soundTriggerColliderFloorLevel)
            {
                newTarget.transform.position = new Vector3(enemyHearingScript.soundTriggerColliderRoom.position.x, transform.position.y);
                currentTarget = newTarget.transform;

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
            else if (currentTarget == null || (currentTarget.tag != "StairsUp" && currentTarget.tag != "StairsDown"))
            {
                if (currentFloorLevel + 1 == playerControllerScript.currentFloorLevel) // If player is one level above enemy
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
            else
            {
                MoveTowardsTarget();
            }
        }
    }

    void SearchRandomly()
    {
        alertStateTimer -= Time.deltaTime;

        if (alertStateTimer <= 0.0f)
        {
            currentState = defaultState;
            Destroy(newTarget);
            alertStateTimer = alertStateTimerLength;
        }

        if (currentRoom != null && newTarget == null)
        {
            newTarget = Instantiate(patrolPointPrefab, patrolPointContainer.transform);

            randomRoom = rooms[UnityEngine.Random.Range(1, rooms.Length)];
            while (randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel)
            {
                randomRoom = rooms[UnityEngine.Random.Range(0, rooms.Length)];
            }
            newTarget.transform.position = new Vector3(randomRoom.transform.position.x + UnityEngine.Random.Range((-randomRoom.GetComponent<SpriteRenderer>().bounds.size.x / 2) + 0.25f, (randomRoom.GetComponent<SpriteRenderer>().bounds.size.x / 2)) - 0.25f, transform.position.y);
            
            currentTarget = newTarget.transform;
        }
        else if ((newTarget != null && currentTarget == null) || (newTarget != null && currentTarget != newTarget.transform))
        {
            currentTarget = newTarget.transform;
        }

        if (currentFloorLevel != newTarget.GetComponent<PatrolPoint>().currentFloorLevel)
        {
            randomRoom = rooms[UnityEngine.Random.Range(1, rooms.Length)];
            while (randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel)
            {
                randomRoom = rooms[UnityEngine.Random.Range(0, rooms.Length)];
            }
            newTarget.transform.position = new Vector3(randomRoom.transform.position.x + UnityEngine.Random.Range((-randomRoom.GetComponent<SpriteRenderer>().bounds.size.x / 2) + 0.25f, (randomRoom.GetComponent<SpriteRenderer>().bounds.size.x / 2)) - 0.25f, transform.position.y);
        }

        if (currentTarget != null)
        {
            MoveTowardsTarget();
        }
    }

    void SpreadOut()
    {
        if (UnityEngine.Random.Range(0, 2) == 0 && currentTarget == null)
        {
            print("Enemy staying on same floor.");
            currentState = State.Alert;
            mayBeAlert = false;
        }
        else if (currentTarget == null)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                print("Enemy trying to move to floor above first.");
                if (GameObject.Find("Floor" + (currentFloorLevel + 1) + "Room" + currentRoomNumber) != null)
                {
                    currentTarget = GameObject.Find("Floor" + (currentFloorLevel + 1) + "Room" + currentRoomNumber).GetComponent<Room>().nearestStairsUpTo;
                }
                else if (GameObject.Find("Floor" + (currentFloorLevel + 2) + "Room" + currentRoomNumber) != null)
                {
                    currentTarget = GameObject.Find("Floor" + (currentFloorLevel + 2) + "Room" + currentRoomNumber).GetComponent<Room>().secondNearestStairsUpTo;
                }
                else if (GameObject.Find("Floor" + (currentFloorLevel - 1) + "Room" + currentRoomNumber) != null)
                {
                    currentTarget = GameObject.Find("Floor" + (currentFloorLevel - 1) + "Room" + currentRoomNumber).GetComponent<Room>().nearestStairsDownTo;
                }
                else if (GameObject.Find("Floor" + (currentFloorLevel - 2) + "Room" + currentRoomNumber) != null)
                {
                    currentTarget = GameObject.Find("Floor" + (currentFloorLevel - 2) + "Room" + currentRoomNumber).GetComponent<Room>().secondNearestStairsDownTo;
                }
                else
                {
                    currentState = State.Alert;
                    mayBeAlert = false;
                }
                MoveTowardsTarget();
            }
            else
            {
                print("Enemy trying to move to floor below first.");
                if (GameObject.Find("Floor" + (currentFloorLevel - 1) + "Room" + currentRoomNumber) != null)
                {
                    currentTarget = GameObject.Find("Floor" + (currentFloorLevel - 1) + "Room" + currentRoomNumber).GetComponent<Room>().nearestStairsDownTo;
                }
                else if (GameObject.Find("Floor" + (currentFloorLevel - 2) + "Room" + currentRoomNumber) != null)
                {
                    currentTarget = GameObject.Find("Floor" + (currentFloorLevel - 2) + "Room" + currentRoomNumber).GetComponent<Room>().secondNearestStairsDownTo;
                }
                else if (GameObject.Find("Floor" + (currentFloorLevel + 1) + "Room" + currentRoomNumber) != null)
                {
                    currentTarget = GameObject.Find("Floor" + (currentFloorLevel + 1) + "Room" + currentRoomNumber).GetComponent<Room>().nearestStairsUpTo;
                }
                else if (GameObject.Find("Floor" + (currentFloorLevel + 2) + "Room" + currentRoomNumber) != null)
                {
                    currentTarget = GameObject.Find("Floor" + (currentFloorLevel + 2) + "Room" + currentRoomNumber).GetComponent<Room>().secondNearestStairsUpTo;
                }
                else
                {
                    currentState = State.Alert;
                    mayBeAlert = false;
                }
                MoveTowardsTarget();
            }
        }
        else
        {
            MoveTowardsTarget();
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

            while (timer < continueSearchingForPlayerTime)
            {
                playerPositionKnown = true;
                yield return new WaitForSeconds(1f);
                timer++;
                // Debug.Log("Timer: " + timer);
            }

            if (timer >= continueSearchingForPlayerTime && enemyScript.enemyStats.currentHealth > 0)
            {
                stillSearching = false;
                playerPositionKnown = false;
                currentState = State.Alert;
            }
        }
    }

    void OnTriggerEnter2D()
    {
        timer = 0.0f;
    }
}