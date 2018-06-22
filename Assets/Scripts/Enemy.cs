using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour {

    [System.Serializable]
    public class EnemyStats
    {
        public float maxHealth = 1;
        [Header("Note: 1.0 = 100%")] [Range(0.1f, 10.0f)] public float startHealthPercent = 1f;
        public float actualMaxHealth;

        private float _currentHealth;
        public float currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0f, actualMaxHealth); }
        }

        public float damage = 1;

        public void Init()
        {
            currentHealth = actualMaxHealth;
        }
    }

    public EnemyStats enemyStats = new EnemyStats();

    public Transform deathParticles;

    public string deathSoundName = "Explosion";

    public int moneyDrop = 10;

    // public float shakeAmt = 0.1f;
    // public float shakeLength = 0.3f;

    [Header("Optional: ")][SerializeField] private StatusIndicator statusIndicator;

    void Start()
    {
        enemyStats.actualMaxHealth = enemyStats.maxHealth * enemyStats.startHealthPercent;
        enemyStats.Init();

        /*if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.currentHealth, enemyStats.maxHealth); // Set health to max health at start
        }*/

        GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;

        if (deathParticles == null)
        {
            Debug.LogError("No death particles referenced on Enemy");
        }
    }

    void OnUpgradeMenuToggle(bool active)
    {
        // Handle what happens when the upgrade menu is toggled
        if (this != null)
        {
            GetComponent<EnemyMovement>().enabled = !active;
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
