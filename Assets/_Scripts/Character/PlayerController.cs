using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float gravity = -20f;

    private CharacterController _characterController;

    private Vector3 velocity;

    [SerializeField] private LayerMask groundLayers;

    public bool isGrounded;

    private float horizontalMovement;
    private float verticalMovement;

    public float movementSpeed;
    public float walkSpeed;
    public float runSpeed;

    public float jumpHeight= 2.0f;
    private int jumpCounter = 0;
    public Transform[] groundChecks;
    public Transform[] blockChecks;
    
    private bool jumpPressed;
    private float jumpTimer;
    private float jumpGracePeriod = 0.2f;
    
    [HideInInspector]
    public Animator _animator;
    public bool animMove;


    public bool playerMoving = true;

    public float turnSmoothingTime = 0.1f;
    float turnSmoothVelocity;
    private bool runTrigger;

    public static PlayerController instance;

    public float health = 100;
    public Image lifeFront;
    public bool canBeAttacked = true;

    public GameObject gameOver;
    
    

    [SerializeField] private Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        movementSpeed = walkSpeed;
        _characterController = GetComponent<CharacterController>();
        _animator = gameObject.GetComponentInChildren<Animator>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.gameOver) return;
        if(GameManager.instance.pauseMenuStatus) return;
        // if (GameManager.instance.gameCompleted)
        // {
        //     _animator.SetBool("run", false);
        //     return;
        // }
        
        if(Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = runSpeed;
            runTrigger=true;
        }
        else
        {
            movementSpeed = walkSpeed;
            runTrigger = false;
        }

        
        isGrounded = false;
        foreach (var groundCheck in groundChecks)
        {
            if (Physics.CheckSphere(groundCheck.position, 0.1f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
                break;
            }
        }
            
        // isGrounded = _characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
            jumpCounter = 0;
            playerMoving = true;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // var blocked = false;
        // foreach (var blockCheck in blockChecks)
        // {
        //     if (Physics.CheckSphere(blockCheck.position, 0.1f, groundLayers, QueryTriggerInteraction.Ignore))
        //     {
        //         blocked = true;
        //         playerMoving = false;
        //         break;
        //     }
        // }

        jumpPressed = Input.GetButtonDown("Jump");
        if (jumpPressed)
        {
            jumpTimer = Time.time;
            _animator.SetBool("isJumping", true);
        }
        else
        {
            _animator.SetBool("isJumping", false);
        }
        if ( jumpCounter<1 && (jumpPressed || (jumpTimer > 0 && Time.time<jumpTimer+jumpGracePeriod)))
        {
            velocity.y += Mathf.Sqrt((jumpHeight * -1 * gravity));
            jumpCounter++;
            jumpTimer = -1;
        }
        
        
        Vector3 direction = new Vector3(horizontalMovement, 0f, verticalMovement).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothingTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            _characterController.Move(moveDir * movementSpeed * Time.deltaTime);
        }
        
        
        _characterController.Move(velocity * Time.deltaTime);
        
        if ((horizontalMovement > 0.05f || horizontalMovement<-0.05) || (verticalMovement > 0.05 || verticalMovement < -0.05))
        {
            _animator.SetBool("isMoving", true);
            if (runTrigger)
            {
                animateCharacter(1.0f, 1.0f);
            }
            else
            {
                animateCharacter(0f, 1.0f);
            }
            
            
        }

        // else if (horizontalMovement > 0.05f || horizontalMovement<-0.05)
        // {
        //     _animator.SetBool("isMoving", true);
        //     walkAnimate();;
        //     
        // }
        //
        // else if (verticalMovement > 0.05 || verticalMovement < -0.05)
        // {
        //     _animator.SetBool("isMoving", true);
        //     walkAnimate();
        // }
        else
        {
            _animator.SetBool("isMoving", false);
        }

        if (animMove)
        {
            _animator.SetBool("isMoving", true);
        }


    }
    
    private void FixedUpdate()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        
    }

    

    public void animateCharacter(float offset1, float offset2)
    {
        _animator.SetFloat("offset1", offset1, 0.05f, Time.deltaTime);
        _animator.SetFloat("offset2", offset2, 0.05f, Time.deltaTime);
    }
    
    public void animateCharacterAttack(float attack)
    {
        _animator.SetFloat("attack", attack);
    }

    public void TakeDamage()
    {
        if (canBeAttacked)
        {
            _animator.SetTrigger("isStunned");
            canBeAttacked = false;
            StartCoroutine(controlAttackAbosrb());
            health -= 10;
            lifeFront.fillAmount = health / 100;
            print(health);
        
            if (health <= 0)
            {
                health = 100;
                lifeFront.fillAmount = 100;
                gameOver.SetActive(true);
                StartCoroutine(deactivateGameOver());
            }
        }
        
    }

    

    IEnumerator controlAttackAbosrb()
    {
        yield return new WaitForSeconds(1.0f);
        canBeAttacked = true;
    }

    IEnumerator deactivateGameOver()
    {
        yield return new WaitForSeconds(2.0f);
        gameOver.SetActive(false);
    }
}
