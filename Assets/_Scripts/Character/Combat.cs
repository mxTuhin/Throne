using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public bool attackPressed;

    private float attackCounter = 0.5f;

    public bool attackAllowed=true;

    public static Combat instance; 
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        attackPressed = Input.GetButtonDown("Fire1");
        if (attackPressed && attackAllowed)
        {
            attackAllowed = false;
            PlayerController.instance.walkSpeed = 0;
            PlayerController.instance.runSpeed = 0;
            StartCoroutine(allowAttack());
            PlayerController.instance._animator.SetBool("isAttacking", true);
            // PlayerController.instance._animator.SetTrigger("slash");
            PlayerController.instance.animateCharacterAttack(attackCounter);
            attackCounter++;
            if (attackCounter >3.0f)
            {
                attackCounter = 0.5f;
            }
        }
        else
        {
            PlayerController.instance._animator.SetBool("isAttacking", false);
        }

    }

    IEnumerator allowAttack()
    {
        yield return new WaitForSeconds(0.45f);
        PlayerController.instance.walkSpeed = 4;
        PlayerController.instance.runSpeed = 8;
        attackAllowed = true;
    }
}
