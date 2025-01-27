using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
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
    public GameObject HealthBarPrefab;
    private GameObject parent;

    private GameObject HealthBar;
    private float parentHeight;
    
    public void Awake()
    {
        parent = GameObject.Find("HealthBars");
        parentHeight = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y/2f * gameObject.transform.localScale.y;
        HealthBar = Instantiate<GameObject>(HealthBarPrefab, new Vector3(transform.position.x, parentHeight + transform.position.y + 0.01f, 0f), Quaternion.identity);
        HealthBar.transform.localScale = gameObject.transform.localScale;

    }

    public void UpdateEnemy()
    {
        HealthBar.transform.position = new Vector3(transform.position.x, parentHeight + transform.position.y + .3f, 0f);
        HealthBar.transform.rotation = quaternion.identity;

        HealthBar.transform.GetChild(0).GetChild(0).transform.localScale = new Vector3((float)CurrentHealth/(float)MaxHealth * 2f, 1f, 1f);
    }

    public void TakeDamage()
    {
        CurrentHealth -= 1;
        if(CurrentHealth == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        Destroy(HealthBar);
        FindAnyObjectByType<PlayerController>().KilledSomething();
        GetComponent<ShadowedObject>().DestroyThisAndItsShadow();
    }
}
