using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mite : Enemy
{
    private Rigidbody2D mite;
    private float visionDistance = 0.085f;
    private float moveSpeed = 4f;
    private float wanderSpeed = 1.5f;

    private bool isWandering = false;
    private bool isChasing = false;
    // Start is called before the first frame update
    void Start()
    {
        mite = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 playerWorldPosition = Camera.main.ScreenToWorldPoint(playerPosition);

        
        Vector3 mitePosition = Camera.main.ScreenToWorldPoint(mite.transform.position);

        if(Vector2.Distance(mitePosition, playerWorldPosition) < visionDistance)
        {
            Vector2 direction = (playerWorldPosition - mitePosition).normalized;

            if(!isChasing)
                StartCoroutine(Chase(direction));

        }
        else if(!isWandering)
        {
            StartCoroutine(Wander());
        }
        UpdateEnemy();
    }

    IEnumerator Chase(Vector2 direction)
    {

        while(true)
        {
            isChasing = true;
            if(Math.Abs(direction.x) < 0.1f && Math.Abs(direction.y) < 1f)
                    mite.velocity = Vector2.zero;
            else if(direction.x > 0)
                    mite.velocity = new Vector2(1 * moveSpeed, 0);
            else
                    mite.velocity = new Vector2(-1 * moveSpeed, 0);
            yield return new WaitForSeconds(3f);
            isChasing = false;
            yield return new WaitForSeconds(4f);
        }
        
    }

    IEnumerator Wander()
    {
        while(!isChasing)
        {
            isWandering = true;
            mite.velocity = new Vector2((Random.Range(0, 2) == 0 ? -1 : 1) * wanderSpeed, 0);

            yield return new WaitForSeconds(4f);
            mite.velocity = Vector2.zero;

            yield return new WaitForSeconds(4f);
            isWandering = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DealDamage(collision.gameObject);
        }
    }
}
