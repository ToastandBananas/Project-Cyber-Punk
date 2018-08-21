using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class BackgroundTiling : MonoBehaviour
{
    public float offsetX = 0.5f; // The offset so that we don't get any weird errors

    // These are for checking if we need to instantiate stuff
    public bool hasRightBuddy = false;
    public bool hasLeftBuddy = false;

    public bool reverseScale = false; // Used if the object is not tileable

    private float spriteWidth = 0f;   // The width of our element
    private Camera cam;
    private Transform thisTransform;

    void Awake()
    {
        cam = Camera.main;
        thisTransform = transform;
    }

    // Use this for initialization
    void Start ()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = spriteRenderer.bounds.size.x; //spriteRenderer.sprite.bounds.size.x;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Does it still need buddies? If not, do nothing
		if (hasLeftBuddy == false || hasRightBuddy == false)
        {
            // Calculate the camera's extend (half the width) of what the camera can see in the world coordinates
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;

            // Calculate the x position where the camera can see the edge of the sprite (element)
            float edgeVisiblePositionRight = (thisTransform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (thisTransform.position.x - spriteWidth / 2) + camHorizontalExtend;

            // Checking if we can see the edge of the element and then calling MakeNewBuddy if we can
            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && hasRightBuddy == false)
            {
                MakeNewBuddy(1);
                hasRightBuddy = true;
            }
            else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && hasLeftBuddy == false)
            {
                MakeNewBuddy(-1);
                hasLeftBuddy = true;
            }
        }
	}

    // A function that creates a buddy on the side required
    void MakeNewBuddy(int rightOrLeft)
    {
        // Calculating the new position for our new buddy
        Vector3 newPosition = new Vector3(thisTransform.position.x + spriteWidth * rightOrLeft, thisTransform.position.y, thisTransform.position.z);

        // Instantiating out new buddy and storing him in a variable
        Transform newBuddy = Instantiate(thisTransform, newPosition, thisTransform.rotation, thisTransform.parent) as Transform;

        // If not tileable, reverse the x size of our object to get rid of ugly seams
        if (reverseScale == true)
        {
            newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
        }

        //newBuddy.SetParent(thisTransform);

        if (rightOrLeft > 0)
        {
            newBuddy.GetComponent<BackgroundTiling>().hasLeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<BackgroundTiling>().hasRightBuddy = true;
        }
    }
}
