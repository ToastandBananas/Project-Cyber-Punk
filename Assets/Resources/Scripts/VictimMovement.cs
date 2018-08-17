using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimMovement : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheck;
    float groundRadius = 0.1f;
    public LayerMask whatIsGround;
    bool onGround = false;

    [Header("Target Info")]
    public Transform currentTarget;

    public float distanceToPlayer;
    public float stoppingDistance = 1f;

    [Header("Movement Stats")]
    public float runSpeed = 3f;
    public float moveSpeed = 0f;
    float noSpeed = 0f;

    public bool facingRight = true;
    Vector3 victimLocalScale;

    bool isTeleporting = false;

    [Header("Location")]
    public int currentFloorLevel = 1;
    public int currentRoomNumber;
    public int currentBuildingNumber;
    public Transform currentRoom;

    [Header("Materials")]
    public Material defaultMaterial;
    public Material highlightMaterial;

    CapsuleCollider2D thisCollider;
    CapsuleCollider2D playerCollider;
    GameObject[] enemies;
    BoxCollider2D[] playerWeapons;

    Player player;
    PlayerController playerControllerScript;

    SpriteRenderer spriteRenderer;
    Animator anim;

    [Header("Panic State Variables")]
    public GameObject[] rooms;
    public GameObject patrolPointPrefab;
    public GameObject newTarget;
    GameObject randomRoom;
    GameObject patrolPointContainer;

    Transform levelExitTrigger;
    LevelExit levelExitScript;

    public enum State
    {
        Dead = 0,
        Imprisoned = 1,
        Idle = 2,
        Follow = 3,
        Panic = 4,
        ExitLevel = 5
    }

    [Header("States")]
    public State startState = State.Imprisoned;
    public State currentState;
    public State defaultState = State.Idle;

    // Use this for initialization
    void Start () {
        currentState = startState;

        player = Player.instance;
        playerControllerScript = player.GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        rooms = GameObject.FindGameObjectsWithTag("Room");
        patrolPointContainer = GameObject.Find("PatrolPoints");

        // Collisions to ignore:
        thisCollider = GetComponent<CapsuleCollider2D>();
        playerCollider = player.GetComponent<CapsuleCollider2D>();
        Physics2D.IgnoreCollision(thisCollider, playerCollider);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(thisCollider, enemy.GetComponent<CapsuleCollider2D>());
        }
        playerWeapons = GameObject.Find("WeaponPool").transform.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D playerWeaponCollider in playerWeapons)
        {
            Physics2D.IgnoreCollision(playerWeaponCollider, thisCollider);
        }

        victimLocalScale = transform.localScale;

        if (facingRight == false)
        {
            victimLocalScale.x *= -1;
            transform.localScale = victimLocalScale;
            facingRight = false;
        }

        levelExitTrigger = GameObject.Find("LevelExitTrigger").transform;
        levelExitScript = levelExitTrigger.GetComponent<LevelExit>();
    }
	
	// Update is called once per frame
	void Update () {
		if (currentState == State.Follow)
        {
            DetermineTargetFollowState();
        }
        else if (currentState == State.Panic)
        {
            Panic();
        }
        else if (currentState == State.ExitLevel)
        {
            currentTarget = levelExitTrigger;
            MoveTowardsTarget();
        }
	}

    void FixedUpdate()
    {
        CalculateDistanceToPlayer();

        if (currentState == State.Dead)
        {
            anim.SetInteger("currentState", 0);
        }
        else if (currentState == State.Imprisoned)
        {
            DetermineMoveSpeed();
            FreeFromImprisonment();
            anim.SetInteger("currentState", 1);
        }
        else if (currentState == State.Idle)
        {
            DetermineMoveSpeed();
            GroundCheck();
            anim.SetInteger("currentState", 2);
        }
        else if (currentState == State.Follow)
        {
            DetermineMoveSpeed();
            Flip();
            GroundCheck();
            anim.SetInteger("currentState", 3);
        }
        else if (currentState == State.Panic)
        {
            DetermineMoveSpeed();
            Flip();
            GroundCheck();
            anim.SetInteger("currentState", 4);
        }
        else if (currentState == State.ExitLevel)
        {
            DetermineMoveSpeed();
            Flip();
            GroundCheck();
            anim.SetInteger("currentState", 5);
        }
    }

    private void FreeFromImprisonment()
    {
        if (currentRoom == playerControllerScript.currentRoom && distanceToPlayer <= 1)
        {
            spriteRenderer.material = highlightMaterial;
            if (Input.GetKeyDown(KeyCode.E)) // Maybe require a key?
            {
                // Freedom! And then follow the player...
                currentState = State.Follow;
                spriteRenderer.material = defaultMaterial;
                levelExitScript.victimsFollowing++;
            }
        }
        else
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

    private void Panic()
    {
        //panicStateTimer -= Time.deltaTime;

        if (currentRoom == player.GetComponent<PlayerController>().currentRoom)
        {
            currentState = State.Follow;
            Destroy(newTarget);
            //panicStateTimer = panicStateTimerLength;
        }

        if (currentRoom != null && newTarget == null)
        {
            newTarget = Instantiate(patrolPointPrefab, patrolPointContainer.transform);

            randomRoom = rooms[UnityEngine.Random.Range(1, rooms.Length)];
            while ((randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber == currentBuildingNumber)
                    || (randomRoom.GetComponent<Room>().floorLevel == currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber != currentBuildingNumber)
                    || (randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber != currentBuildingNumber)) // If you edit this, also edit the MoveTowardsTarget script
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

        if (currentFloorLevel != newTarget.GetComponent<PatrolPoint>().currentFloorLevel || currentBuildingNumber != newTarget.GetComponent<PatrolPoint>().currentBuildingNumber)
        {
            randomRoom = rooms[UnityEngine.Random.Range(1, rooms.Length)];
            while ((randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber == currentBuildingNumber)
                    || (randomRoom.GetComponent<Room>().floorLevel == currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber != currentBuildingNumber)
                    || (randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber != currentBuildingNumber)) // If you edit this, also edit the MoveTowardsTarget script
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

    private void DetermineTargetFollowState()
    {
        if (currentFloorLevel == playerControllerScript.currentFloorLevel)
        {
            currentTarget = player.transform;
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
        }
        else if (currentTarget == null || (currentTarget.tag != "StairsUp" && currentTarget.tag != "StairsDown"))
        {
            if (currentFloorLevel + 1 == playerControllerScript.currentFloorLevel) // If player is one level above enemy
            {
                Transform targetRoomName = playerControllerScript.currentRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsUpTo.transform;
            }
            else if (currentFloorLevel - 1 == playerControllerScript.currentFloorLevel) // If player is one level below enemy
            {
                Transform targetRoomName = playerControllerScript.currentRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().nearestStairsDownTo.transform;
            }
            else if (currentFloorLevel + 2 == playerControllerScript.currentFloorLevel) // If player is two levels above enemy
            {
                Transform targetRoomName = playerControllerScript.currentRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsUpTo.transform;
            }
            else if (currentFloorLevel - 2 == playerControllerScript.currentFloorLevel) // If player is two levels below enemy
            {
                Transform targetRoomName = playerControllerScript.currentRoom;
                currentTarget = targetRoomName.gameObject.GetComponent<Room>().secondNearestStairsDownTo.transform;
            }
        }

        if (currentTarget != null)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        if (currentTarget == player.transform && distanceToPlayer > stoppingDistance && currentFloorLevel == playerControllerScript.currentFloorLevel)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, transform.position.y), moveSpeed * Time.fixedDeltaTime);
        }
        else if (currentTarget != player.transform)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, transform.position.y), moveSpeed * Time.fixedDeltaTime);
        }

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f) // If enemy makes it to its target while in Alert or CheckSound state
        {
            if (currentState == State.Panic)
            {
                randomRoom = rooms[UnityEngine.Random.Range(1, rooms.Length)];
                while ((randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber == currentBuildingNumber)
                    || (randomRoom.GetComponent<Room>().floorLevel == currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber != currentBuildingNumber)
                    || (randomRoom.GetComponent<Room>().floorLevel != currentFloorLevel && randomRoom.GetComponent<Room>().buildingNumber != currentBuildingNumber)) // If you edit this, also edit the SearchRandomly script
                {
                    randomRoom = rooms[UnityEngine.Random.Range(0, rooms.Length)];
                }
                newTarget.transform.position = new Vector3(randomRoom.transform.position.x + UnityEngine.Random.Range(-randomRoom.GetComponent<SpriteRenderer>().bounds.size.x / 2, randomRoom.GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y);
            }
            else if (currentState == State.ExitLevel)
            {
                currentState = State.Idle; // To prevent this next bit of code from being executed multiple times

                // Play teleport animation and get rid of (destroy) victim game object
                isTeleporting = true;
                anim.SetBool("isTeleporting", isTeleporting);
                Destroy(gameObject, 0.5f);

                // Increase victimsSaved count
                levelExitScript.victimsSaved++;
                if (levelExitScript.victimsSaved == levelExitScript.victimCount)
                    levelExitScript.RescueVictimsMissionComplete();
            }
        }
    }

    private void Flip()
    {
        if (currentTarget != null)
        {
            if (facingRight == true && transform.position.x > currentTarget.transform.position.x)
            {
                victimLocalScale.x *= -1;
                transform.localScale = victimLocalScale;
                facingRight = false;
            }
            else if (facingRight == false && transform.position.x < currentTarget.transform.position.x)
            {
                victimLocalScale.x *= -1;
                transform.localScale = victimLocalScale;
                facingRight = true;
            }
        }
    }

    private void DetermineMoveSpeed()
    {
        if (currentState == State.Imprisoned || currentState == State.Idle)
        {
            moveSpeed = noSpeed;
        }
        else if (currentState == State.Follow || currentState == State.Panic || currentState == State.ExitLevel)
        {
            if ((currentState == State.Follow && distanceToPlayer <= stoppingDistance) || (currentState == State.ExitLevel && Vector2.Distance(currentTarget.position, transform.position) <= 0.1))
            {
                moveSpeed = noSpeed;
            }
            else
            {
                moveSpeed = runSpeed;
            }
        }
        anim.SetFloat("moveSpeed", moveSpeed);
    }

    private void CalculateDistanceToPlayer()
    {
        if (currentState != State.Dead)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            // print("Distance to Player: " + distanceToPlayer + " units");
        }
    }

    private void GroundCheck()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("onGround", onGround);
    }
}
