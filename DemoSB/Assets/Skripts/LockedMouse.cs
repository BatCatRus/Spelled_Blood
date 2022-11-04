using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedMouse : MonoBehaviour
{
 
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
