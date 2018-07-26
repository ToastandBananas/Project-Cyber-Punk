using UnityEngine;

public class EnemyHearing : MonoBehaviour {

    Player player;
    Weapon playerEquippedWeapon;
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
	}

    void OnTriggerStay2D(Collider2D collision)
    {
        playerEquippedWeapon = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();

        if (collision.gameObject.tag == "SoundTrigger" && enemyMovementScript.currentState != EnemyMovement.State.Attack)
        {
            if (enemySightScript.CanPlayerBeSeen() == false && enemyScript.isDead == false)
            {
                soundTriggerColliderOrigin = collision.gameObject.transform;
                soundTriggerColliderParent = collision.gameObject.transform.root;

                if (soundTriggerColliderParent.tag == "Player" && playerEquippedWeapon.isSilenced == false)
                {
                    soundTriggerColliderFloorLevel = soundTriggerColliderParent.gameObject.GetComponent<PlayerController>().currentFloorLevel;
                    soundTriggerColliderRoomNumber = soundTriggerColliderParent.gameObject.GetComponent<PlayerController>().currentRoomNumber;
                    soundTriggerColliderRoom = soundTriggerColliderParent.gameObject.GetComponent<PlayerController>().currentRoom;

                    enemyMovementScript.currentState = EnemyMovement.State.CheckSound;
                }
                else if (soundTriggerColliderParent.tag == "Enemy" && soundTriggerColliderParent.GetComponentInChildren<EnemyWeapon>().isSilenced == false)
                {
                    soundTriggerColliderFloorLevel = soundTriggerColliderParent.gameObject.GetComponent<EnemyMovement>().currentFloorLevel;
                    soundTriggerColliderRoomNumber = soundTriggerColliderParent.gameObject.GetComponent<EnemyMovement>().currentRoomNumber;
                    soundTriggerColliderRoom = soundTriggerColliderParent.gameObject.GetComponent<EnemyMovement>().currentRoom;

                    enemyMovementScript.currentState = EnemyMovement.State.CheckSound;
                }
            }
        }
    }
}
