using UnityEngine;

public class EnemyArmRotation : MonoBehaviour {

    [SerializeField] public static int rotationOffset = 360;

    Player player;
    Enemy enemy;
    EnemySenses enemySenses;

    void Start()
    {
        player = Player.instance;
        enemy = Enemy.instance;
        enemySenses = EnemySenses.instance;
    }

    // Update is called once per frame
    void Update () {
        Vector3 playerPosition = player.transform.position;
        Vector3 enemyPosition = enemy.transform.position;

        Vector3 difference = playerPosition - enemyPosition; // Subtracting the enemy's position from the player's position
        difference.Normalize(); // Normalize the vector. Meaning that the sum of the vector will be equal to 1.

        if (enemySenses.CanPlayerBeSeen() == true)
        {
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; // Find the angle in degrees.
            transform.rotation = Quaternion.Euler(0f, 0f, (rotationZ + rotationOffset));
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
