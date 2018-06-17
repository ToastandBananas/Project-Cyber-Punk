using UnityEngine.UI;
using UnityEngine;

public class StatusIndicator : MonoBehaviour {

    [SerializeField] private RectTransform healthBarRect;
    [SerializeField] private Text healthText;

    void Start()
    {
        if (healthBarRect == null)
        {
            Debug.Log("STATUS INDICATOR: No health bar object referenced!");
        }
        if (healthText == null)
        {
            Debug.Log("STATUS INDICATOR: No health text object referenced!");
        }
    }

    public void SetHealth(float _current, float _max)
    {
        float _value = (float)_current / _max;

        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z); // Change size of health bar depending on current health
        healthText.text = _current + "/" + _max + " HP";
    }
}
