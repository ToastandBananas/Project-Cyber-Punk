using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [System.Serializable]
    public class PlayerStats
    {
        public float maxHealth = 1;
        [Header("Note: 1.0 = 100%")] [Range(0.1f, 10.0f)] public float startHealthPercent = 1f;

        private float _currentHealth;
        public float currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0f, maxHealth); }
        }

        public void Init()
        {
            currentHealth = maxHealth;
        }
    }

    public PlayerStats playerStats = new PlayerStats();

    public int fallBoundary = -20;

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "DamageVoice";

    private AudioManager audioManager;

    [SerializeField] private StatusIndicator statusIndicator;

    void Start()
    {
        playerStats.Init();
        if (statusIndicator == null)
        {
            Debug.LogError("No status indicator referenced on Player.");
        }
        else
        {
            statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        }

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
