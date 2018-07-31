using UnityEngine;

public class ProduceSoundTrigger : MonoBehaviour {

    public GameObject soundTriggerPrefab;

    CircleCollider2D soundTriggerCollider;

    Transform parent;

    EnemyHearing enemyHearingScript;

    // Use this for initialization
    void Start () {
        parent = gameObject.transform;

        soundTriggerCollider = soundTriggerPrefab.GetComponent<CircleCollider2D>();
    }

    public void SoundTrigger(float soundRadius)
    {
        GameObject soundTrigger = Instantiate(soundTriggerPrefab, parent) as GameObject;
        soundTriggerCollider.radius = soundRadius;

        Destroy(soundTrigger.gameObject, 0.1f);
    }
}
