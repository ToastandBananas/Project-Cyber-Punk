using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster gm;

    void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 3;

    public CameraShake cameraShake;

    // Cache
    private AudioManager audioManager;

    void Start()
    {
        if (cameraShake == null)
        {
            Debug.LogError("No camera shake referenced in GameMaster.");
        }

        // Caching
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found in the scene");
        }
    }

    public IEnumerator _RespawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);

        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm._RespawnPlayer());
    }

    public static void KillEnemy(Enemy enemy)
    {
        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        // Death sound
        audioManager.PlaySound(_enemy.deathSoundName);

        // Add particles
        Transform _clone = Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity) as Transform;
        Destroy(_clone.gameObject, 3f);

        // Camera Shake
        // cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);
        Destroy(_enemy.gameObject);
    }
}
