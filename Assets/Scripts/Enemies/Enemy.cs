using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int Health;


    public void TakeDamage()
    {
        Health -= 1;
        if(Health == 0)
            Destroy(gameObject);
    }
}
