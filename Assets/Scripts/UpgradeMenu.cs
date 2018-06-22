using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour {

    [SerializeField] private Text healthText;

    [SerializeField] private int upgradeCost = 50;

    StatusIndicator statusIndicator;

    Player player;

    void OnEnable()
    {
        statusIndicator = StatusIndicator.instance;
        player = Player.instance;
        UpdateValues();
    }

    void UpdateValues()
    {
        healthText.text = "Health: " + player.playerStats.maxHealth.ToString();
    }

    public void UpgradeHealth()
    {
        if(GameMaster.Money < upgradeCost)
        {
            AudioManager.instance.PlaySound("NoMoney");
            return;
        }
        
        player.playerStats.maxHealth += 1;
        GameMaster.Money -= upgradeCost;
        AudioManager.instance.PlaySound("Money");
        player.playerStats.currentHealth = player.playerStats.maxHealth;
        //player.statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        UpdateValues();
    }
}
