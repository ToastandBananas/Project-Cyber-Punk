using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public Transform[] backgrounds;     // Array of all the backgrounds to be parallaxed
    private float[] parallaxScales;     // The proportion of the camera's movement to move the backgrounds by
    public float smoothing = 1f;        // How smooth the parallax scrolling is going to be. Make sure to set above 0

    private Transform cam;              // Reference to the main cameras transform
    private Vector3 previousCamPos;     // The position of the camera in the previous frame

    void Awake()
    {
        cam = Camera.main.transform;
    }

    // Use this for initialization
    void Start () {
        previousCamPos = cam.position;

        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++) // Assigning corresponding parallaxScales
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < backgrounds.Length; i++)
        {
            // The parallax is the opposite of the camera movement because the previous frame multiplied by the scale
            float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScales[i];

            // Set a target x position which is the current position plus the parallax
            float backgroundTargetPosX = backgrounds[i].position.x + parallaxX;

            // Set a target y position which is the current position plus the parallax
            float backgroundTargetPosY = backgrounds[i].position.y + parallaxY;

            // Create a target position which is the background's current position with its target x position
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgroundTargetPosY, backgrounds[i].position.z);

            // Fade between current position and target position using lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        // Set the previousCamPos to the camera's position at the end of the frame
        previousCamPos = cam.position;
	}
}
