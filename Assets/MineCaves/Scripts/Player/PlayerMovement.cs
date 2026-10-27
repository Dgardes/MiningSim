using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public enum PlayerState
{
    Idle,
    Walk,
    Jump,
    InAir,
    CrawlIdle,
    CrawlWalk
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;
    private PlayerStats stats;
    private PlayerController playerController;
    private FloatingJoystick joystick;

    [Header("Crawl Settings")]
    [SerializeField] private float crawlSpeedMultiplier = 0.5f;
    
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private CapsuleDirection2D originalDirection;

    private Vector2 crawlColliderSize = new Vector2(1.4f, 0.8f);
    private Vector2 crawlColliderOffset = new Vector2(0f, -0.4f);

    [Header("Fall Tracker")]
    private float maxFallSpeedThisJump = 0f;
    private bool wasGroundedLastFrame;

    [Header("State Machine")]
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    public event Action<PlayerState> OnStateChanged;

    private Vector2 movementInput;
    private bool isGrounded;
    private bool jumpIsReloading;
    private float jumpCooldownTime = 0.15f;
    private bool joystickIsActive;

    private LayerMask jumpableLayers;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        stats = GetComponent<PlayerStats>();
        jumpableLayers = LayerMask.GetMask("Ground", "MiningTilemap", "MinableBlocks");

        if (playerCollider != null)
        {
            originalColliderSize = playerCollider.size;
            originalColliderOffset = playerCollider.offset;
            originalDirection = playerCollider.direction;
        }

        playerController = new PlayerController();
        
        var moveObj = GameObject.Find("Move");
        if (moveObj != null) joystick = moveObj.GetComponent<FloatingJoystick>();

        playerController.Enable();
    }

    private void OnEnable()
    {
        playerController.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerController.Player.Move.canceled += ctx => movementInput = Vector2.zero;
        playerController.Player.Jump.performed += ctx => TryJump();
    }

    private void OnDisable()
    {
        playerController.Player.Disable();
    }

    private void Update()
    {
        CheckGround();
        TrackFallSpeed();
        UpdateStateMachine();
    }

    private void FixedUpdate()
    {
        HandleJoystickInput();
        ApplyMovement();
    }

    private void HandleJoystickInput()
    {
        if (joystick == null) return;

        if (joystick.Direction.y >= 0.7f)
        {
            TryJump();
        }

        if (joystick.Direction.magnitude > 0.05f)
        {
            joystickIsActive = true;
            movementInput = new Vector2(joystick.Horizontal, 0);
        }
        else if (joystickIsActive)
        {
            movementInput = Vector2.zero;
            joystickIsActive = false;
        }
    }

    private void ApplyMovement()
    {
        float currentSpeed = stats.MoveSpeed;

        if (IsCrawlingState(CurrentState))
        {
            currentSpeed *= crawlSpeedMultiplier;
        }

        rb.linearVelocity = new Vector2(movementInput.x * currentSpeed, rb.linearVelocityY);
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, jumpableLayers);
        isGrounded = hit.collider != null;

        if (isGrounded && !wasGroundedLastFrame)
        {
            OnLanded(hit);
        }

        wasGroundedLastFrame = isGrounded;
    }

    private void TrackFallSpeed()
    {
        if (!isGrounded && rb.linearVelocityY < maxFallSpeedThisJump)
        {
            maxFallSpeedThisJump = rb.linearVelocityY;
        }
    }

    private void OnLanded(RaycastHit2D groundHit)
    {
        if (maxFallSpeedThisJump < stats.SafeFallSpeed)
        {
            float excessSpeed = Mathf.Abs(maxFallSpeedThisJump - stats.SafeFallSpeed);
            float damageToApply = excessSpeed * stats.FallDamageMultiplier;

            /*
            if (groundHit.collider.CompareTag("Water") || groundHit.collider.CompareTag("Bouncer"))
            {
                damageToApply = 0;
            }
            */
            if (damageToApply > 0)
            {
                stats.TakeDamage(damageToApply);
            }
        }

        maxFallSpeedThisJump = 0f;
    }

    private void TryJump()
    {
        if (isGrounded && !jumpIsReloading && !IsCrawlingState(CurrentState))
        {
            rb.AddForce(new Vector2(0f, stats.JumpForce), ForceMode2D.Impulse);
            SetState(PlayerState.Jump);
            StartCoroutine(JumpReload());
        }
    }

    private IEnumerator JumpReload()
    {
        jumpIsReloading = true;
        yield return new WaitForSeconds(jumpCooldownTime);
        jumpIsReloading = false;
    }

    private void UpdateStateMachine()
    {
        if (!isGrounded)
        {
            if (rb.linearVelocityY < 0)
            {
                SetState(PlayerState.InAir);
            }
            return;
        }

        if (IsCrawlingState(CurrentState))
        {
            if (Mathf.Abs(movementInput.x) > 0.05f)
            {
                SetState(PlayerState.CrawlWalk);
            }
            else
            {
                SetState(PlayerState.CrawlIdle);
            }
            return;
        }

        if (CurrentState == PlayerState.Jump || CurrentState == PlayerState.InAir)
        {
            SetState(Mathf.Abs(movementInput.x) > 0.05f ? PlayerState.Walk : PlayerState.Idle);
            return;
        }

        if (Mathf.Abs(movementInput.x) > 0.05f)
        {
            SetState(PlayerState.Walk);
        }
        else
        {
            SetState(PlayerState.Idle);
        }
    }

    public void SetState(PlayerState newState)
    {
        if (CurrentState == newState) return;

        bool wasCrawling = IsCrawlingState(CurrentState);
        bool willCrawl = IsCrawlingState(newState);

        CurrentState = newState;

        if (playerCollider != null && wasCrawling != willCrawl)
        {
            if (willCrawl)
            {
                playerCollider.direction = CapsuleDirection2D.Horizontal;
                playerCollider.size = crawlColliderSize;
                playerCollider.offset = crawlColliderOffset;
            }
            else
            {
                playerCollider.direction = originalDirection;
                playerCollider.size = originalColliderSize;
                playerCollider.offset = originalColliderOffset;
            }
        }

        OnStateChanged?.Invoke(CurrentState);
    }

    public void ToggleCrawl()
    {
        if (IsCrawlingState(CurrentState))
        {
            SetState(isGrounded ? (Mathf.Abs(movementInput.x) > 0.05f ? PlayerState.Walk : PlayerState.Idle) : PlayerState.InAir);
        }
        else if (isGrounded)
        {
            SetState(Mathf.Abs(movementInput.x) > 0.05f ? PlayerState.CrawlWalk : PlayerState.CrawlIdle);
        }
    }

    private bool IsCrawlingState(PlayerState state)
    {
        return state == PlayerState.CrawlIdle || state == PlayerState.CrawlWalk;
    }
}