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

    [Header("Room Info")]
    public int floorLevel;
    public int roomNumber;
    public int buildingNumber;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PatrolPoint")
        {
            collision.GetComponent<PatrolPoint>().currentFloorLevel = floorLevel;
            collision.GetComponent<PatrolPoint>().currentRoomNumber = roomNumber;
            collision.GetComponent<PatrolPoint>().currentRoom = gameObject.transform;
            collision.GetComponent<PatrolPoint>().currentBuildingNumber = buildingNumber;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerController>().currentFloorLevel = floorLevel;
            collision.GetComponent<PlayerController>().currentRoomNumber = roomNumber;
            collision.GetComponent<PlayerController>().currentRoom = gameObject.transform;
            collision.GetComponent<PlayerController>().currentBuildingNumber = buildingNumber;
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<EnemyMovement>().currentFloorLevel = floorLevel;
            collision.GetComponent<EnemyMovement>().currentRoomNumber = roomNumber;
            collision.GetComponent<EnemyMovement>().currentRoom = gameObject.transform;
            collision.GetComponent<EnemyMovement>().currentBuildingNumber = buildingNumber;
        }
        else if (collision.gameObject.tag == "Victim")
        {
            collision.GetComponent<VictimMovement>().currentFloorLevel = floorLevel;
            collision.GetComponent<VictimMovement>().currentRoomNumber = roomNumber;
            collision.GetComponent<VictimMovement>().currentRoom = gameObject.transform;
            collision.GetComponent<VictimMovement>().currentBuildingNumber = buildingNumber;
        }
        else if (collision.tag == "Light")
        {
            collision.GetComponent<LightRaycast>().floorLevel = floorLevel;
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
