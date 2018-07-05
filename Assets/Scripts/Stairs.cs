using UnityEngine;

public class Stairs : MonoBehaviour {

    Player player;

    public Transform targetDoor;

    BoxCollider2D playerCollider;
    BoxCollider2D stairsTriggerCollider;

    bool playerIsNearStairs = false;
    
    void Start()
    {
        player = Player.instance;

        playerCollider = player.GetComponent<BoxCollider2D>();
        stairsTriggerCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        UseStairs();
    }

    void UseStairs()
    {
        if (playerIsNearStairs == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Debug.Log("Attempting to use stairway door named: " + targetDoor);
                player.transform.position = new Vector3(targetDoor.transform.position.x, targetDoor.transform.position.y - 0.13f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsNearStairs = true;
            // Debug.Log("Player is near stairs: " + playerIsNearStairs);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsNearStairs = false;
            // Debug.Log("Player is near stairs: " + playerIsNearStairs);
        }
    }
}
