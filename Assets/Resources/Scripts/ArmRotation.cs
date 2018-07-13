using UnityEngine;
using System.Collections;

public class ArmRotation : MonoBehaviour {

    [SerializeField] public int enemyRotationOffset = 360;
    [SerializeField] public int playerRotationOffset = 360;

    Player player;
    EnemySight enemySightScript;
    EnemyMovement enemyMovementScript;

    Transform enemy;
    Transform enemySight;

    public enum WhoIsControlling
    {
        PlayerControl,
        EnemyControl
    }

    public WhoIsControlling whoIsControlling;

    void Start()
    {
        player = Player.instance;

        if (whoIsControlling == WhoIsControlling.EnemyControl)
        {
            enemy = transform.root;
            enemySight = enemy.Find("Sight");

            enemySightScript = enemySight.GetComponent<EnemySight>();
            enemyMovementScript = enemy.GetComponent<EnemyMovement>();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (whoIsControlling == WhoIsControlling.EnemyControl)
        {
            EnemyArmRotation();
        }
        else if (whoIsControlling == WhoIsControlling.PlayerControl)
        {
            PlayerArmRotation();
        }
        else
        {
            Debug.LogError("Who Is Controlling the arm rotation is not set in the transform");
            return;
        }
    }

    void EnemyArmRotation()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 enemyPosition = enemy.transform.position;

        Vector3 difference = playerPosition - enemyPosition; // Subtracting the enemy's position from the player's position
        difference.Normalize(); // Normalize the vector. Meaning that the sum of the vector will be equal to 1.

        if (enemySightScript.CanPlayerBeSeen() == true || enemyMovementScript.playerPositionKnown == true)
        {
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; // Find the angle in degrees.
            transform.rotation = Quaternion.Euler(0f, 0f, (rotationZ + enemyRotationOffset));
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void PlayerArmRotation()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; // Subtracting position of the player from the mouse position.
        difference.Normalize(); // Normalize the vector. Meaning that the sum of the vector will be equal to 1.

        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; // Find the angle in degrees.
        transform.rotation = Quaternion.Euler(0f, 0f, (rotationZ + playerRotationOffset));
    }
}
