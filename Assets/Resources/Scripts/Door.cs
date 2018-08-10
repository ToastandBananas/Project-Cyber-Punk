using UnityEngine;

public class Door : MonoBehaviour {

    Player player;
    Transform door;

    Vector3 doorPosition;
    Vector3 playerPosition;
    
    BoxCollider2D doorTriggerCollider;
    PolygonCollider2D doorCollider;

    SFPolygon SFPolygonScript;

    bool isNearDoor = false;

    Animator doorAnim;

    public enum DoorState
    {
        Closed = 0,
        OpenLeft = 1,
        OpenRight = 2
    }

    DoorState startDoorState;
    DoorState currentDoorState;

    // Use this for initialization
    void Start () {
        player = Player.instance;
        door = transform.parent;
        
        doorTriggerCollider = GetComponent<BoxCollider2D>();
        doorCollider = door.GetComponent<PolygonCollider2D>();

        SFPolygonScript = door.GetComponent<SFPolygon>();

        startDoorState = DoorState.Closed;
        currentDoorState = startDoorState;
        
        doorPosition = door.transform.position;

        doorAnim = door.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        OpenOrCloseDoor();

        if (currentDoorState == DoorState.Closed)
        {
            doorAnim.SetInteger("doorState", 0);
            doorCollider.enabled = true;
            SFPolygonScript.enabled = true;
        }
        else if (currentDoorState == DoorState.OpenLeft)
        {
            doorAnim.SetInteger("doorState", 1);
            doorCollider.enabled = false;
            SFPolygonScript.enabled = false;
        }
        else if (currentDoorState == DoorState.OpenRight)
        {
            doorAnim.SetInteger("doorState", 2);
            doorCollider.enabled = false;
            SFPolygonScript.enabled = false;
        }
        // Debug.Log("Current door state: " + currentDoorState);
	}

    void OpenOrCloseDoor()
    {
        playerPosition = player.transform.position;

        if (isNearDoor == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentDoorState == DoorState.Closed)
                {
                    if (doorPosition.x > playerPosition.x)
                    {
                        currentDoorState = DoorState.OpenRight;
                    }
                    else if (doorPosition.x < playerPosition.x)
                    {
                        currentDoorState = DoorState.OpenLeft;
                    }
                }
                else if (currentDoorState == DoorState.OpenLeft || currentDoorState == DoorState.OpenRight)
                {
                    currentDoorState = DoorState.Closed;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            isNearDoor = true;
        }

        if (coll.gameObject.tag == "Enemy" && coll.isTrigger == false)
        {
            if (currentDoorState == DoorState.Closed && coll.gameObject.GetComponent<EnemyMovement>().currentState != EnemyMovement.State.Idle)
            {
                if (doorPosition.x > coll.gameObject.transform.position.x)
                {
                    currentDoorState = DoorState.OpenRight;
                }
                else if (doorPosition.x < coll.gameObject.transform.position.x)
                {
                    currentDoorState = DoorState.OpenLeft;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            isNearDoor = false;
        }
    }
}
