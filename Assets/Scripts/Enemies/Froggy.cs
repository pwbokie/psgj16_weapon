using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Froggy : Enemy
{
    private Rigidbody2D froggy;
    
    // Start is called before the first frame update
    void Start()
    {
        froggy = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemy();
    }
}
