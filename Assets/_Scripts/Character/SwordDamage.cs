using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public GameObject deathParticle;
    public AudioClip slashSound;
    private void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject.GetComponentInParent<EnemyAI>();
        if (hit != null && !Combat.instance.attackAllowed)
        {
            hit.damageCount++;
            StartCoroutine(EnemyAI.instance.stunAnimate());
            print("Sword Attack: "+hit.damageCount);
            AudioSource.PlayClipAtPoint(slashSound, hit.gameObject.transform.position);
            if (hit.damageCount>= 5)
            {
                GameObject deathP = Instantiate(deathParticle, hit.transform.position, hit.transform.rotation) as GameObject;
                // Destroy(hit.gameObject);
                hit.gameObject.SetActive(false);
            }
        }
    }

    
}
