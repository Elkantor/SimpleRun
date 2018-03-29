using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScript : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Sprite sprite;
    private Texture2D texture;
    private Color newColor;
    private float red = 0.9F, green = 0.5F, blue = 0.1F;
    private float minimumR, maximumR,t, minimumG, maximumG;
    // Use this for initialization
    void Start () {
		spriteRenderer= GetComponent<SpriteRenderer>();
        texture = new Texture2D(20, 20);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 20, 20), Vector2.zero);
        spriteRenderer.sprite = sprite;

        //Min et max du rouge et vert pour le Lerp
        minimumR = 0.6f;
        maximumR = 1f;
        minimumG = 0.2f;
        maximumG = 0.6f;
        t = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++) //Goes through each pixel
            {
                Color pixelColour;
                red = Mathf.Lerp(minimumR, maximumR, t);
                green = Mathf.Lerp(minimumG, maximumG, t);
                pixelColour = new Color(red,green,blue,1);
                t += 0.25f * Time.deltaTime/500;

                if (t > 1.0f)
                {
                    float temp = maximumR;
                    maximumR = minimumR;
                    minimumR = temp;
                    temp = maximumG;
                    maximumG = minimumG;
                    minimumG = temp;
                    t = 0.0f;
                }
                texture.SetPixel(x, y, pixelColour);
            }
        }
        texture.Apply();
        
    }
}
