using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [System.Serializable]
    public class EnemyStats
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
            currentHealth = maxHealth * startHealthPercent;
        }
    }

    public EnemyStats enemyStats = new EnemyStats();

    [Header("Optional: ")][SerializeField] private StatusIndicator statusIndicator;

    void Start()
    {
        enemyStats.Init();

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.currentHealth, enemyStats.maxHealth); // Set health to max health at start
        }
    }

    public void DamageEnemy(int damage)
    {
        enemyStats.currentHealth -= damage;

        if (enemyStats.currentHealth <= 0)
        {
            GameMaster.KillEnemy(this);
        }

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.currentHealth, enemyStats.maxHealth); // Subtract damage from health if damage doesn't kill enemy
        }
    }
}
