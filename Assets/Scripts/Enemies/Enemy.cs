using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int CurrentHealth;
    public int MaxHealth;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer healthBarRenderer;
    private Sprite sprite;
    public GameObject HealthBar;
    private GameObject parent;
    
    public void Awake()
    {
        parent = new GameObject();
        float parentHeight = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        parent = gameObject;
        parent.transform.position = new Vector3(parent.transform.position.x, parentHeight + parent.transform.position.y + 10f, 0f);
        _ = Instantiate<GameObject>(HealthBar, parent.transform); 
    }

    public void Update()
    {
        /*HealthBar.transform.position = (Vector2)spriteRenderer.transform.position + Vector2.one;
        HealthBar.transform.rotation = quaternion.identity;
        HealthBar.transform.localScale = spriteRenderer.transform.localScale;
        */
    }

    public void TakeDamage()
    {
        CurrentHealth -= 1;
        if(CurrentHealth == 0)
            Destroy(gameObject);
    }

    


}
