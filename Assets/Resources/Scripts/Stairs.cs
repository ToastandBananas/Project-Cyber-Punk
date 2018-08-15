using UnityEngine;

public class Stairs : MonoBehaviour {

    Player player;

    public Transform targetDoor; // Where these stairs lead to
    
    BoxCollider2D stairsTriggerCollider;

    bool playerIsNearStairs = false;
    
    void Start()
    {
        player = Player.instance;

        stairsTriggerCollider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        UseStairs();
    }

    void UseStairs()
    {
        if (playerIsNearStairs == true)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                // Debug.Log("Attempting to use stairway door named: " + targetDoor);
                player.transform.position = new Vector3(targetDoor.transform.position.x, targetDoor.transform.position.y - 0.13f);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsNearStairs = true;
            // Debug.Log("Player is near stairs: " + playerIsNearStairs);
        }
        else if (collision.gameObject.tag == "Enemy" && collision.gameObject.GetComponent<EnemyMovement>().currentTarget == transform)
        {
            collision.gameObject.transform.position = new Vector3(targetDoor.transform.position.x, targetDoor.transform.position.y - 0.13f);
            if (collision.GetComponent<EnemyMovement>().currentState == EnemyMovement.State.SpreadOut 
                || (collision.GetComponent<EnemyMovement>().currentState == EnemyMovement.State.Attack && (collision.GetComponent<EnemyMovement>().currentRoom != player.GetComponent<PlayerController>().currentRoom || player.isVisibleByLight == false)))
            {
                collision.GetComponent<EnemyMovement>().currentState = EnemyMovement.State.Alert;
                collision.GetComponent<EnemyMovement>().currentTarget = null;
            }
        }
        else if (collision.tag == "Victim" && collision.GetComponent<VictimMovement>().currentTarget == transform)
        {
            collision.transform.position = new Vector3(targetDoor.transform.position.x, targetDoor.transform.position.y - 0.13f);
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
