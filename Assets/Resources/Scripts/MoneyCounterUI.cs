using UnityEngine.UI;
using UnityEngine;

public class MoneyCounterUI : MonoBehaviour {

    private Text moneyText;

	void Awake () {
        moneyText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        moneyText.text = "CASH: $" + GameMaster.Money.ToString();
	}
}
