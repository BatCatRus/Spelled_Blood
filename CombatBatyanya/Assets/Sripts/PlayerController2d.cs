using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2d : MonoBehaviour
{ 
    public float movementForce;
    public float maxHorizontalMovementSpeed;

    private float movementDirection;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
        {
            movementDirection = Input.GetAxis("Horizontal");
        }
    private void FixedUpdate()
        {
            Vector2 forceToAdd = new Vector2(movementForce * movementDirection, 0);
            rb.AddForce(forceToAdd);
        }
}
