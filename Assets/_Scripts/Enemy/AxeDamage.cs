using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        var hit = collision.gameObject.GetComponentInParent<PlayerController>();
        if (hit != null)
        {
            PlayerController.instance.TakeDamage();
            print("Character Hit");
        }
    }
}
