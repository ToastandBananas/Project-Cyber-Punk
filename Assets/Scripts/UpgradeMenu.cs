using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour {

    [SerializeField] private Text healthText;

    [SerializeField] private int upgradeCost = 50;

    private PlayerStats playerStats;

    void OnEnable()
    {
        playerStats = PlayerStats.instance;
        UpdateValues();
    }

    void UpdateValues()
    {
        healthText.text = "Health: " + playerStats.maxHealth.ToString();
    }

    public void UpgradeHealth()
    {
        if(GameMaster.Money < upgradeCost)
        {
            AudioManager.instance.PlaySound("NoMoney");
            return;
        }

        playerStats.maxHealth += 1;
        GameMaster.Money -= upgradeCost;
        AudioManager.instance.PlaySound("Money");
        playerStats.currentHealth = playerStats.maxHealth;
        UpdateValues();
    }
}
