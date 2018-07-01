using System.Collections;
using UnityEngine;

public class EnemyHearing : MonoBehaviour {

    Player player;
    EnemySight enemySightScript;
    GameObject enemySight;

    BoxCollider2D playerCollider;
    CircleCollider2D enemyHearingTriggerCollider;

	// Use this for initialization
	void Start () {
        player = Player.instance;
        enemySight = GameObject.Find("Sight");
        enemySightScript = enemySight.GetComponent<EnemySight>();

        playerCollider = player.GetComponent<BoxCollider2D>();
        enemyHearingTriggerCollider = gameObject.GetComponent<CircleCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, enemyHearingTriggerCollider);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
