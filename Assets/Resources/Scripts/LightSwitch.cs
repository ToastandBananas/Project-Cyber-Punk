using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    Transform lightFixture; // Light fixture this light switch is paired with
    SFLight lightRenderer;

    SpriteRenderer spriteRenderer;
    public Material defaultMaterial;
    public Material highlightMaterial;

    public bool lightOn = false;

	// Use this for initialization
	void Start () {
        lightFixture = transform.parent;
        lightRenderer = lightFixture.GetComponentInChildren<SFLight>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (lightRenderer.gameObject.activeSelf == true)
        {
            lightOn = true;
        }
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        print(collision.tag);
        if (collision.tag == "Player")
        {
            print("Yep thats the player");
            spriteRenderer.material = highlightMaterial;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (lightOn)
                {
                    lightOn = false;
                    lightRenderer.gameObject.SetActive(false);
                }
                else
                {
                    lightOn = true;
                    lightRenderer.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            spriteRenderer.material = defaultMaterial;
        }
    }
}
