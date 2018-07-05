using System.Collections;
using UnityEngine;

public class Room : MonoBehaviour {

    public Transform stairsUp;
    public Transform stairsDown;

    public Transform nearestStairsUpTo;
    public Transform nearestStairsDownTo;

    public int floorLevel;
    public int roomNumber;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision)
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
    }
}
