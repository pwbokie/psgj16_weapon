using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mite : Enemy
{
    private Rigidbody2D mite;
    // Start is called before the first frame update
    void Start()
    {
        mite = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemy();
    }
}
