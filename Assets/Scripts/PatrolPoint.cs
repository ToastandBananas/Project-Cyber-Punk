using UnityEngine;

public class PatrolPoint : MonoBehaviour {

    public int currentFloorLevel;
    public int currentRoomNumber;
    public Transform currentRoom;

    [Header("Only use if patrol point is outside:")]
    public Transform nearestRoom;

    void Start()
    {
        currentFloorLevel = 1;
    }
}
