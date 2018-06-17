using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [System.Serializable]
    public class PlayerStats
    {
        public float maxHealth = 100;
        public float startHealthPercent = 1f;

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
            GameMaster.KillPlayer(this);
        }

        statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
    }
}
