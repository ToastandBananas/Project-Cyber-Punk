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

        public float damage = 20;

        public void Init()
        {
            currentHealth = maxHealth * startHealthPercent;
        }
    }

    public EnemyStats enemyStats = new EnemyStats();

    public Transform deathParticles;

    // public float shakeAmt = 0.1f;
    // public float shakeLength = 0.3f;

    [Header("Optional: ")][SerializeField] private StatusIndicator statusIndicator;

    void Start()
    {
        enemyStats.Init();

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.currentHealth, enemyStats.maxHealth); // Set health to max health at start
        }

        if (deathParticles == null)
        {
            Debug.LogError("No death particles referenced on Enemy");
        }
    }

    public void DamageEnemy(float damage)
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

    void OnCollisionEnter2D(Collision2D _colliderInfo) // Damage player on collision with enemy
    {
        Player _player = _colliderInfo.collider.GetComponent<Player>();

        if(_player != null)
        {
            _player.DamagePlayer(enemyStats.damage);
        }
    }
}
