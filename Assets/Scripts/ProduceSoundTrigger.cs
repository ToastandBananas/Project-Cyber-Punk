using UnityEngine;

public class ProduceSoundTrigger : MonoBehaviour {

    public GameObject soundTriggerPrefab;

    CircleCollider2D soundTriggerCollider;

    Transform parent;

    EnemyHearing enemyHearingScript;
    AudioManager audioManager;

    // Use this for initialization
    void Start () {
        parent = gameObject.transform;

        soundTriggerCollider = soundTriggerPrefab.GetComponent<CircleCollider2D>();

        audioManager = AudioManager.instance;
    }

    public void SoundTrigger(float soundRadius)
    {
        GameObject soundTrigger = Instantiate(soundTriggerPrefab, parent) as GameObject;
        soundTriggerCollider.radius = soundRadius;

        Destroy(soundTrigger.gameObject, .5f);
    }
}
