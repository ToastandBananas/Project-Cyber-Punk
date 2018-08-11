using System.Collections;
using UnityEngine;

public class Room : MonoBehaviour {

    public Transform stairsUp;
    public Transform stairsDown;

    public Transform nearestStairsUpTo;
    public Transform nearestStairsDownTo;

    public Transform secondNearestStairsUpTo;
    public Transform secondNearestStairsDownTo;

    [Header("Only use if this 'room' is outside:")]
    public Transform nearestRoom;

    public int floorLevel;
    public int roomNumber;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PatrolPoint")
        {
            collision.GetComponent<PatrolPoint>().currentFloorLevel = floorLevel;
            collision.GetComponent<PatrolPoint>().currentRoomNumber = roomNumber;
            collision.GetComponent<PatrolPoint>().currentRoom = gameObject.transform;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerController>().currentFloorLevel = floorLevel;
            collision.GetComponent<PlayerController>().currentRoomNumber = roomNumber;
            collision.GetComponent<PlayerController>().currentRoom = gameObject.transform;
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<EnemyMovement>().currentFloorLevel = floorLevel;
            collision.GetComponent<EnemyMovement>().currentRoomNumber = roomNumber;
            collision.GetComponent<EnemyMovement>().currentRoom = gameObject.transform;
        }
        else if (collision.gameObject.tag == "Victim")
        {
            collision.GetComponent<VictimMovement>().currentFloorLevel = floorLevel;
            collision.GetComponent<VictimMovement>().currentRoomNumber = roomNumber;
            collision.GetComponent<VictimMovement>().currentRoom = gameObject.transform;
        }
    }

    /*void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerController>().currentFloorLevel = 1;
            collision.GetComponent<PlayerController>().currentRoomNumber = 0;
            collision.GetComponent<PlayerController>().currentRoom = null;
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<EnemyMovement>().currentFloorLevel = 1;
            collision.GetComponent<EnemyMovement>().currentRoomNumber = 0;
            collision.GetComponent<EnemyMovement>().currentRoom = null;
        }
    }*/
}
