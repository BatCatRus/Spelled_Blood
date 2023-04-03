using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*Данный код на языке C# является скриптом для управления персонажем в игре. В первых строках кода подключаются необходимые библиотеки. 
Далее идут переменные, которые используются в ходе выполнения скрипта, например, переменные для определения направления движения персонажа, таймеры для различных действий, флаги для определения состояния персонажа и т.д.
Затем в скрипте определяется класс PlayerController2D, который наследуется от MonoBehaviour, и в котором описываются методы, используемые для управления персонажем.
В методе Start происходит инициализация необходимых компонентов объекта, на котором находится скрипт, и установка начальных значений переменных.
Метод Update вызывается один раз за каждый кадр и в нем проверяется состояние кнопок управления, направление движения персонажа, проигрывание анимации и т.д.
Метод FixedUpdate вызывается с фиксированной частотой и в нем происходит применение физики к персонажу, проверка состояния окружения и т.д.
Данный скрипт также содержит множество других методов, которые используются для проверки и управления различными состояниями персонажа, например, методы CheckIfWallSliding(), CheckLedgeClimb(), CheckSurroundings() и т.д.*/

public class PlayerController2D : MonoBehaviour
{
    /*Переменные, начинающиеся с private, используются только внутри класса.
    Они описывают состояние персонажа и его параметры, 
    например, isGrounded - находится ли персонаж на земле, 
    amountOfJumpsLeft - количество оставшихся прыжков, jumpForce - сила прыжка, и т.д.*/
    
    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;
    private float knockbackStartTime;
    [SerializeField]
    private float knockbackDuration;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private int lastWallJumpDirection;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool isTouchingLedge;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private bool isDashing;
    private bool knockback;

    [SerializeField]
    private Vector2 knockbackSpeed;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;

    private Rigidbody2D rb;
    private Animator anim;
    
    /*Переменные, начинающиеся с public, могут быть доступны в других классах.Они также описывают параметры 
      персонажа, но могут быть изменены в инспекторе Unity, например, movementSpeed - скорость передвижения, 
      groundCheck - позиция, откуда осуществляется проверка на землю, и т.д.*/
    
    public int amountOfJumps = 1; 

    public float movementSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet = 0.15f;
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCoolDown;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;

    public LayerMask whatIsGround;

    // Метод Start() вызывается при запуске скрипта. В нем получаем ссылки на компоненты Rigidbody2D и Animator, а также инициализируем переменную amountOfJumpsLeft.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // Метод Update() вызывается каждый кадр и отвечает за обработку ввода и обновление анимации персонажа. В нем вызываются также другие методы, такие как CheckIfCanJump(), CheckIfWallSliding(), и т.д.
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
        CheckLedgeClimb();
        CheckDash();
        CheckKnockback();
    }

    //Метод FixedUpdate() вызывается каждый раз, когда происходит обновление физики. В нем вызываются методы ApplyMovement() и CheckSurroundings(), которые отвечают за движение персонажа и проверку окружения.
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

   /*Методы CheckIfWallSliding() и CheckLedgeClimb() отвечают за проверку, находится ли персонаж на стене или на лестнице, соответственно. 
    Если персонаж находится на стене, то устанавливается флаг isWallSliding.
    Если персонаж находится на лестнице, то устанавливается флаг canClimbLedge, что позволяет персонажу забираться на нее.*/

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            canMove = false;
            canFlip = false;

            anim.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    /*Функция "FinishLedgeClimb()" сбрасывает значения переменных "canClimbLedge", "canMove" и "canFlip", 
    * чтобы игрок мог продолжать движение. Позиция игрока устанавливается в "ledgePos2", 
    * которая является конечной позицией, куда игрок переместился после зажима кнопки "вверх". 
    * Затем в функции "CheckLedgeClimb()" значение переменной "canClimbLedge" устанавливается обратно в "false", чтобы игрок мог продолжать движение.*/
    
    public void FinishLedgeClimb()                              
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);
    }

    //Метод CheckSurroundings() проверяет, находится ли персонаж на земле или находится ли он у стены.
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    //Функция CheckIfCanJump() проверяет, может ли игрок совершить прыжок в данный момент, и изменяет соответствующие переменные.
    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (isTouchingWall)
        {
            checkJumpMultiplier = false;
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }

    }

    //Функция CheckMovementDirection() определяет направление движения игрока на основе ввода пользователя и изменяет соответствующие переменные.
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    //Функция UpdateAnimations() обновляет анимацию игрока на основе текущего состояния, такого как направление движения и нахождение на земле.
    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("IsWallSliding", isWallSliding);
    }

    //Функция CheckInput() проверяет, нажимает ли игрок клавиши для передвижения, прыжка или выполнения других действий.
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || (amountOfJumpsLeft > 0 && !isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time >= (lastDash + dashCoolDown))
                AttemptToDash();
        }

    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    //Функция CheckDash() проверяет, может ли игрок использовать способность "Dash" и, если это так, выполняет соответствующие действия.
    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0.0f);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }

            if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }

        }
    }

    //Функция CheckJump() обрабатывает прыжок игрока, учитывая текущее состояние и изменяет его скорость и направление движения.
    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            //WallJump
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    //Эта функция описывает обычный прыжок игрока в игре.
    //В целом, эта функция обрабатывает действия, необходимые для выполнения обычного прыжка игрока в игре.
    private void NormalJump()
    {
        if (canNormalJump)//Сначала проверяется, может ли игрок выполнить обычный прыжок с помощью переменной canNormalJump.
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); //Если можно, то происходит присваивание новой скорости игроку с помощью свойства velocity объекта Rigidbody2D. Игрок движется только по вертикали, так как x-координата скорости не меняется.
            amountOfJumpsLeft--; //Затем уменьшается количество доступных прыжков (amountOfJumpsLeft), что означает, что игрок может выполнять только определенное количество прыжков, прежде чем коснется земли.
            jumpTimer = 0;//Затем переменная jumpTimer устанавливается в 0, что означает, что прошло 0 секунд с момента прыжка, и isAttemptingToJump устанавливается в false, что означает, что игрок больше не пытается прыгнуть.
            isAttemptingToJump = false; //и isAttemptingToJump устанавливается в false, что означает, что игрок больше не пытается прыгнуть.
            checkJumpMultiplier = true; //Наконец, устанавливается checkJumpMultiplier в true, что означает, что при следующем прыжке игрока будет использоваться множитель прыжка, который позволит ему прыгать выше, чем обычно.
        }
    }

    private void WallJump()
    {
        if (canWallJump) //Сначала проверяется, может ли игрок выполнить стенный прыжок с помощью переменной canWallJump.Если можно, то происходят следующие действия:
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);//Сначала скорость игрока по оси y устанавливается в 0.0f, чтобы избежать продолжения скольжения по стене.
            isWallSliding = false; //Затем isWallSliding устанавливается в false, что означает, что игрок больше не скользит по стене.
            amountOfJumpsLeft = amountOfJumps; //Затем количество доступных прыжков устанавливается на изначальное значение(amountOfJumps)
            amountOfJumpsLeft--; // а затем уменьшается на 1.
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y); //Затем вычисляется вектор силы forceToAdd, который будет добавлен к скорости игрока.Этот вектор зависит от wallJumpForce (силы прыжка), wallJumpDirection (направления прыжка), movementInputDirection (направления движения игрока) и facingDirection (направления, в котором смотрит игрок). 
            rb.AddForce(forceToAdd, ForceMode2D.Impulse); //Затем этот вектор силы добавляется к скорости игрока с помощью метода AddForce объекта Rigidbody2D.
            jumpTimer = 0;//Далее происходят такие же действия, как в обычном прыжке: jumpTimer устанавливается в 0
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;//Для более детального объяснения, переменная wallJumpTimer устанавливается на wallJumpTimerSet, что означает, что после выполнения стенного прыжка, игрок не сможет сразу же выполнить его снова, пока таймер wallJumpTimer не достигнет нуля. Это сделано для того, чтобы предотвратить неограниченное использование стенного прыжка.
            lastWallJumpDirection = -facingDirection;//Переменная lastWallJumpDirection устанавливается на противоположное направление от того, в котором смотрит игрок, чтобы после стенного прыжка он смог бы перевернуться и двигаться в противоположном направлении.
        }
    }


    //Функция ApplyMovement() используется для применения движения к игроку. Она основывается на значениях переменных, связанных с движением, таких как movementInputDirection, isWallSliding и canMove.
    private void ApplyMovement()
    {

        if (!isGrounded && !isWallSliding && movementInputDirection == 0 && !knockback)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove && !knockback)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }


        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    //Функция отвечает за разворот персонажа на противоположную сторону.
    private void Flip()
    {
        if (!isWallSliding && canFlip && !knockback) //происходит проверка на два условия: не происходит ли стена скольжения (isWallSliding) и разрешен ли разворот (canFlip).
        {
            facingDirection *= -1;//Если эти условия выполняются, то происходит изменение направления, в котором смотрит персонаж (facingDirection) путем умножения на -1. 
            isFacingRight = !isFacingRight; //Также меняется значение флага, который определяет, в какую сторону смотрит персонаж (isFacingRight).
            transform.Rotate(0.0f, 180.0f, 0.0f); //Затем происходит поворот персонажа на 180 градусов с помощью метода Rotate класса Transform. 
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
    /*Первая строка функции рисует проводящую сферу (wire sphere) в позиции, указанной в переменной groundCheck, с радиусом, указанным в переменной groundCheckRadius.
        Это может использоваться, например, для визуализации области, которая проверяется на наличие земли для персонажа.
        Вторая строка функции рисует линию (draw line) между двумя точками: позицией указанной в переменной wallCheck и точкой, 
        смещенной от нее на заданное расстояние по оси x (wallCheckDistance). Это может использоваться, например, для визуализации области, которая проверяется на наличие стены для персонажа.*/

    public bool GetDashStatus()
    {
        return isDashing;
    }
    
    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    private void CheckKnockback()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }
}



