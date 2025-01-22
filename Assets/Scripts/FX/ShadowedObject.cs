using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowedObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer shadowRenderer;
    private Sprite sprite;

    private GameObject shadowsParent;
    private GameObject shadow;
    
    public void Awake()
    {
        if (GetComponent<SpriteRenderer>() != null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        else {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the object or its children.");
            return;
        }

        sprite = spriteRenderer.sprite;

        shadowsParent = GameObject.Find("Shadows");
        shadow = new GameObject("Shadow_" + gameObject.name);

        shadowRenderer = shadow.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = sprite;
        shadowRenderer.color = Global.ShadowColor;

        Material shadowMaterial = Resources.Load<Material>("Materials/FlatColor");
        if (shadowMaterial != null)
        {
            shadowRenderer.sharedMaterial = shadowMaterial;
        }
        else
        {
            Debug.LogError("Failed to load shadow material. Ensure it's in a 'Resources/Materials' folder.");
        }

        shadow.transform.parent = shadowsParent.transform;

        shadowRenderer.sortingOrder = Global.ShadowLayer;
    }

    public void Update()
    {
        shadow.transform.position = (Vector2)spriteRenderer.transform.position + Global.ShadowOffset;
        shadow.transform.rotation = spriteRenderer.transform.rotation;
        shadow.transform.localScale = spriteRenderer.transform.localScale;
        // for animations
        shadowRenderer.sprite = spriteRenderer.sprite;
    }

    public void DestroyThisAndItsShadow()
    {
        Destroy(shadow);
        Destroy(gameObject);
    }

    public void DestroyThisAndItsShadow(float delay)
    {
        Destroy(shadow, delay);
        Destroy(gameObject, delay);
    }
}
