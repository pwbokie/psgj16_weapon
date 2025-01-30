using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachment_Bayonet : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Enemy"))
        {
            collision2D.gameObject.GetComponent<Enemy>().TakeDamage();
        }
    }
}
