using UnityEngine;

public enum UpperBodyState
{
    Idle,
    Walk,
    Jump,
    InAir,
    CrawlIdle,
    CrawlWalk,
    Mining,
    CrawlMining,
}

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private MiningLogic miningLogic;
    [SerializeField] private Rigidbody2D rb;

    private Animator animator;

    private static readonly int LegsStateHash = Animator.StringToHash("LegsState");
    private static readonly int TorsoStateHash = Animator.StringToHash("TorsoState");

    private bool isCurrentlyMining;
    private PlayerState currentMovementState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (movement == null)       movement = GetComponentInParent<PlayerMovement>();
        if (miningLogic == null)    miningLogic = GetComponentInParent<MiningLogic>();
        if (rb == null)             rb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (movement != null) movement.OnStateChanged += HandleMovementStateChanged;
        if (miningLogic != null) miningLogic.OnMiningStateChanged += HandleMiningStateChanged;
    }

    private void OnDisable()
    {
        if (movement != null) movement.OnStateChanged -= HandleMovementStateChanged;
        if (miningLogic != null) miningLogic.OnMiningStateChanged -= HandleMiningStateChanged;
    }

    public void HandleMovementStateChanged(PlayerState newState)
    {
        currentMovementState = newState;
        animator.SetInteger(LegsStateHash, (int)newState);
        UpdateTorsoState();
    }

    public void HandleMiningStateChanged(bool isMining)
    {
        isCurrentlyMining = isMining;
        UpdateTorsoState();
    }

    private void UpdateTorsoState()
    {
        if (isCurrentlyMining)
        {
            if (currentMovementState == PlayerState.CrawlIdle || currentMovementState == PlayerState.CrawlWalk)
            {
                animator.SetInteger(TorsoStateHash, (int)UpperBodyState.CrawlMining);
            }
            else
            {
                animator.SetInteger(TorsoStateHash, (int)UpperBodyState.Mining);
            }
            return;
        }

        switch (currentMovementState)
        {
            case PlayerState.Idle:
                animator.SetInteger(TorsoStateHash, (int)UpperBodyState.Idle);
                break;
            case PlayerState.Walk:
                animator.SetInteger(TorsoStateHash, (int)UpperBodyState.Walk);
                break;
            case PlayerState.Jump:
                animator.SetInteger(TorsoStateHash, (int)UpperBodyState.Jump);
                break;
            case PlayerState.InAir:
                animator.SetInteger(TorsoStateHash, (int)UpperBodyState.InAir);
                break;
            case PlayerState.CrawlIdle:
                animator.SetInteger(TorsoStateHash, (int)UpperBodyState.CrawlIdle);
                break;
            case PlayerState.CrawlWalk:
                animator.SetInteger(TorsoStateHash, (int)UpperBodyState.CrawlWalk);
                break;
        }
    }
    private void LateUpdate()
    {
        if (!isCurrentlyMining && rb != null)
        {
            if (rb.linearVelocityX > 0.05f)
            {
                SetFacing(1f);
            }
            else if (rb.linearVelocityX < -0.05f)
            {
                SetFacing(-1f);
            }
        }
    }

    public void SetFacing(float directionX)
    {
        float sign = Mathf.Sign(directionX);
        transform.localScale = new Vector3(sign, 1f, 1f);
    }
}