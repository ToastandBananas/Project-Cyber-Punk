using UnityEngine;

public class ProduceSoundTrigger : MonoBehaviour {

    GameObject soundTriggerPrefab;

    CircleCollider2D soundTriggerCollider;

    Transform parent;

    AudioManager audioManager;

    // Use this for initialization
    void Start () {
        soundTriggerPrefab = (GameObject)Resources.Load("Prefabs/Weapons/SoundTrigger", typeof(GameObject));
        soundTriggerCollider = soundTriggerPrefab.GetComponent<CircleCollider2D>();

        parent = gameObject.transform;

        audioManager = AudioManager.instance;
    }

    public void SoundTrigger(float soundRadius)
    {
        soundTriggerCollider.radius = soundRadius;
        GameObject soundTrigger = Instantiate(soundTriggerPrefab, parent) as GameObject;
        Destroy(soundTrigger.gameObject, .5f);
    }
}
