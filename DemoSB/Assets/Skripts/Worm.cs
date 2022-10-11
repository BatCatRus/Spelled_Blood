using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Entity
{
    private void Start()
    {
        lives = 3;
    }

 //       private void OnCollisionEnter2D(Collision2D collision)
 //       {
 //           if (collision.gameObject == Hero.Instance.gameObject)
 //           {
 //               Hero.Instance.GetDamage();
 //               lives--;
 //               Debug.Log("у херни " + lives);
 //           }
//
 //           if (lives < 1)
 //               Die();
 //       }
}
