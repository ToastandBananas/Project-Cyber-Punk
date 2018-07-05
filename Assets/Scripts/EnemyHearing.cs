using UnityEngine;

public class EnemyHearing : MonoBehaviour {

    Player player;
    Transform enemy;
    Transform enemySight;

    Enemy enemyScript;
    EnemySight enemySightScript;

    [HideInInspector] public Transform soundTriggerColliderOrigin;
    [HideInInspector] public Transform soundTriggerColliderParent;
    public Transform soundTriggerColliderRoom;
    public int soundTriggerColliderFloorLevel;
    public int soundTriggerColliderRoomNumber;

    GameObject[] hearingObjects;

    BoxCollider2D playerCollider;
    // BoxCollider2D enemyCollider;
    CircleCollider2D thisHearingTriggerCollider;
    CircleCollider2D otherHearingTriggerCollider;
    CircleCollider2D soundTriggerCollider;

    ProduceSoundTrigger produceSoundTriggerScript;
    EnemyMovement enemyMovementScript;

    // Use this for initialization
    void Start () {
        player = Player.instance;
        enemy = transform.root;
        enemySight = enemy.Find("Sight");

        hearingObjects = GameObject.FindGameObjectsWithTag("Hearing");
        
        foreach(GameObject hearingObject in hearingObjects)
        {
            otherHearingTriggerCollider = hearingObject.GetComponent<CircleCollider2D>();
        }

        enemySightScript = enemySight.GetComponent<EnemySight>();
        enemyScript = enemy.GetComponent<Enemy>();
        produceSoundTriggerScript = GetComponent<ProduceSoundTrigger>();
        enemyMovementScript = enemy.GetComponent<EnemyMovement>();

        playerCollider = player.GetComponent<BoxCollider2D>();
        // enemyCollider = gameObject.transform.root.gameObject.GetComponent<BoxCollider2D>();
        thisHearingTriggerCollider = gameObject.GetComponent<CircleCollider2D>();
        thisHearingTriggerCollider.radius = enemyScript.enemyStats.hearingRadius;

        Physics2D.IgnoreCollision(thisHearingTriggerCollider, otherHearingTriggerCollider);
	}

    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Enemy Hearing colliding with: " + collision);

        if (collision.gameObject.tag == "SoundTrigger" && enemyMovementScript.currentState != EnemyMovement.State.Attack)
        {
            if (enemySightScript.CanPlayerBeSeen() == false && enemyScript.isDead == false)
            {
                soundTriggerColliderOrigin = collision.gameObject.transform;
                soundTriggerColliderParent = collision.gameObject.transform.root;

                if (soundTriggerColliderParent.tag == "Player")
                {
                    soundTriggerColliderFloorLevel = soundTriggerColliderParent.gameObject.GetComponent<PlayerController>().currentFloorLevel;
                    soundTriggerColliderRoom = soundTriggerColliderParent.gameObject.GetComponent<PlayerController>().currentRoom;
                    // Debug.Log("Sound Parent: " + soundTriggerColliderParent);
                }

                enemyMovementScript.currentState = EnemyMovement.State.CheckSound;
            }
        }
    }
}
