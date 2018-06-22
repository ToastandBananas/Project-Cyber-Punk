using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : MonoBehaviour {

    [SerializeField] public static int rotationOffset = 360;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; // Subtracting position of the player from the mouse position.
        difference.Normalize(); // Normalize the vector. Meaning that the sum of the vector will be equal to 1.

        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; // Find the angle in degrees.
        transform.rotation = Quaternion.Euler(0f, 0f, (rotationZ + rotationOffset));
	}
}
