using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mite : Enemy
{
    private Rigidbody2D mite;
    private float visionDistance = 0.085f;
    private float moveSpeed = 4f;
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

            Debug.Log("X: " + direction.x + "\nY: " + direction.y);
            Debug.Log("X Max: " + 0.1f + "\nY Max: " + 1f);
            if(Math.Abs(direction.x) < 0.1f && Math.Abs(direction.y) < 1f)
                mite.velocity = Vector2.zero;
            else if(direction.x > 0)
                mite.velocity = new Vector2(1 * moveSpeed, 0);
            else
                mite.velocity = new Vector2(-1 * moveSpeed, 0);

        }
        UpdateEnemy();
    }
}
