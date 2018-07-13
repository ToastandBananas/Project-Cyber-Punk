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

    CapsuleCollider2D playerCollider;
    CircleCollider2D soundTriggerCollider;

    ProduceSoundTrigger produceSoundTriggerScript;
    EnemyMovement enemyMovementScript;

    // Use this for initialization
    void Start () {
        player = Player.instance;
        enemy = transform.root;
        enemySight = enemy.Find("Sight");

        enemySightScript = enemySight.GetComponent<EnemySight>();
        enemyScript = enemy.GetComponent<Enemy>();
        produceSoundTriggerScript = GetComponent<ProduceSoundTrigger>();
        enemyMovementScript = enemy.GetComponent<EnemyMovement>();

        playerCollider = player.GetComponent<CapsuleCollider2D>();
	}

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SoundTrigger" && enemyMovementScript.currentState != EnemyMovement.State.Attack)
        {
            if (enemySightScript.CanPlayerBeSeen() == false && enemyScript.isDead == false)
            {
                soundTriggerColliderOrigin = collision.gameObject.transform;
                soundTriggerColliderParent = collision.gameObject.transform.root;

                if (soundTriggerColliderParent.tag == "Player")
                {
                    soundTriggerColliderFloorLevel = soundTriggerColliderParent.gameObject.GetComponent<PlayerController>().currentFloorLevel;
                    soundTriggerColliderRoomNumber = soundTriggerColliderParent.gameObject.GetComponent<PlayerController>().currentRoomNumber;
                    soundTriggerColliderRoom = soundTriggerColliderParent.gameObject.GetComponent<PlayerController>().currentRoom;
                }

                enemyMovementScript.currentState = EnemyMovement.State.CheckSound;
            }
        }
    }
}
