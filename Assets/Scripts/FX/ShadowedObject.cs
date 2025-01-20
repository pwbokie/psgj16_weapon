using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowedObject : MonoBehaviour
{
    private Sprite sprite;

    private GameObject shadowsParent;
    private GameObject shadow;
    
    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;

        shadowsParent = GameObject.Find("Shadows");
        shadow = new GameObject("Shadow_" + gameObject.name);

        SpriteRenderer shadowRenderer = shadow.AddComponent<SpriteRenderer>();
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
        shadow.transform.position = (Vector2)transform.position + Global.ShadowOffset;
        shadow.transform.rotation = transform.rotation;
        shadow.transform.localScale = transform.localScale;
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
