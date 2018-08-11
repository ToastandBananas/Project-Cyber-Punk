using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimMovement : MonoBehaviour
{
    public Transform currentTarget;

    public float distanceToPlayer;
    public float stoppingDistance = 1f;

    float noSpeed = 0f;
    public float runSpeed = 3f;
    public float moveSpeed = 0f;

    public bool facingRight = true;
    Vector3 victimLocalScale;

    public int currentFloorLevel = 1;
    public int currentRoomNumber;
    public Transform currentRoom;

    Player player;
    PlayerController playerControllerScript;

    Animator anim;

    public enum State
    {
        Dead = 0,
        Imprisoned = 1,
        Idle = 2,
        Follow = 3
        // Panic state when can't find player?
    }

    public State startState;
    public State currentState;
    public State defaultState = State.Idle;

    // Use this for initialization
    void Start () {
        player = Player.instance;
        playerControllerScript = player.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        victimLocalScale = transform.localScale;

        if (facingRight == false)
        {
            victimLocalScale.x *= -1;
            transform.localScale = victimLocalScale;
            facingRight = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (currentState == State.Follow)
        {
            DetermineTargetFollowState();
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
            anim.SetInteger("currentState", 1);
        }
        else if (currentState == State.Idle)
        {
            DetermineMoveSpeed();
            anim.SetInteger("currentState", 2);
        }
        else if (currentState == State.Follow)
        {
            DetermineMoveSpeed();
            Flip();
            anim.SetInteger("currentState", 3);
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
        else if (currentState == State.Follow)
        {
            if (distanceToPlayer <= stoppingDistance)
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
}
