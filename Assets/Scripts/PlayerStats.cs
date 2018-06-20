using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static PlayerStats instance;

    public float maxHealth = 1;
    [Header("Note: 1.0 = 100%")] [Range(0.1f, 10.0f)] public float startHealthPercent = 1f;

    private float _currentHealth;
    public float currentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0f, maxHealth); }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        currentHealth = maxHealth;
    }
}
