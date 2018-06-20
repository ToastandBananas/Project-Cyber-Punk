using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour {

    public int fallBoundary = -20;

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "DamageVoice";

    private AudioManager audioManager;

    [SerializeField] private StatusIndicator statusIndicator;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = PlayerStats.instance;

        if (statusIndicator == null)
        {
            Debug.LogError("No status indicator referenced on Player.");
        }
        else
        {
            statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        }

        GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audio manager in scene.");
        }
    }

    void Update()
    {
        if (transform.position.y <= fallBoundary)
        {
            DamagePlayer(1000000000);
        }
    }

    void RegenHealth()
    {
        playerStats.currentHealth += 1;
        statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
    }

    void OnUpgradeMenuToggle(bool active)
    {
        // Handle what happens when the upgrade menu is toggled
        GetComponent<PlayerController>().enabled = !active;
        Weapon _weapon = GetComponentInChildren<Weapon>();
        if(_weapon != null)
        {
            _weapon.enabled = !active;
        }
    }

    public void DamagePlayer(float damage)
    {
        playerStats.currentHealth -= damage;
        
        if(playerStats.currentHealth <= 0)
        {
            // Play death sound
            audioManager.PlaySound(deathSoundName);

            // Kill player
            GameMaster.KillPlayer(this);
        }
        else
        {
            // Play damage sound
            audioManager.PlaySound(damageSoundName);
        }

        statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
    }
}
