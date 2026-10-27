using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class MiningLogic : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private FloatingJoystick miningJoystick;

    [SerializeField] private ProgressBarProcessor progressBarProcessor;

    private Transform playerTransform; 
    private LayerMask minable;
    private PlayerStats playerStats;

    private BlockLogic currentMiningBlock;
    private float holdingTime = 0;
    private float requiredHoldTime = 1.5f;

    private TargetHighlighter th;

    private bool isMining = false;

    public event Action<bool> OnMiningStateChanged;

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
        playerStats = GetComponent<PlayerStats>();
        minable = LayerMask.GetMask("MiningTilemap", "MinableBlocks");
        th = GetComponent<TargetHighlighter>();
    }

    private void FixedUpdate()
    {
        if (miningJoystick.Direction.magnitude > 0.05f)
        {
            float currentMiningRange = playerStats != null ? playerStats.MiningRange : 2.5f;

            RaycastHit2D Hit = Physics2D.Raycast(playerTransform.position, miningJoystick.Direction.normalized, currentMiningRange, minable);
            if (Hit.collider != null)
            { 
                if (Hit.collider.CompareTag("MinableBlock"))
                { 
                    BlockLogic thisBlock = Hit.collider.GetComponent<BlockLogic>();
                    th.highliteTarget(getCreationPos(tilemap.WorldToCell(Hit.point + miningJoystick.Direction * 0.01f)));
                    SetMining(true);
                    mineBlock(thisBlock);
                }
                else if (Hit.collider.CompareTag("MinableTilemap"))
                { 
                    checkTile(Hit);
                }
            }
            else 
            { 
                ResetMiningState();
            }
        }
        else 
        { 
            ResetMiningState();
        }
    }

    private void ResetMiningState()
    {
        SetMining(false);

        currentMiningBlock = null; 
        holdingTime = 0; 
        if (th != null)
        {
            th.removeHighlight(); 
            th.highlight(false); 
        }
        if (progressBarProcessor != null)
        {
            progressBarProcessor.hideProgressBar();
        }
    }

    private void checkTile(RaycastHit2D Hit)
    {
        // Vector3Int cellPosition = tilemap.WorldToCell(Hit.point + miningJoystick.Direction * 0.01f);
        // BlockPrefabProcessor.instance.ActivateTile(cellPosition, TileLayer.Solid);
    }

    private void mineBlock(BlockLogic thisBlock)
    {
        if (currentMiningBlock != null && currentMiningBlock.getBlockID() == thisBlock.getBlockID())
        {
            holdingTime += Time.fixedDeltaTime;
            
            requiredHoldTime = CalculateRequiredTime(thisBlock);

            progressBarProcessor.updateProgressBar(holdingTime, requiredHoldTime, thisBlock);
            
            if (holdingTime >= requiredHoldTime)
            {
                // int damage = 5;
                // thisBlock.takeDamageFromPLayer(damage, playerStats);
                holdingTime = 0;
            }
        }
        else
        {
            currentMiningBlock = thisBlock;
            holdingTime = 0;
            requiredHoldTime = CalculateRequiredTime(thisBlock);
        }
    }

    private float CalculateRequiredTime(BlockLogic block)
    {
        if (block == null || block.getBlockData() == null) return 1.5f;
        float tBase = 1.5f;
        float efficiency = playerStats != null ? playerStats.MiningEfficiency : 0.0f;
        float finalTime = tBase / (1.0f + efficiency);
        return Mathf.Max(0.1f, finalTime);
    }

    private Vector3 getCreationPos(Vector3Int pos)
    { 
        return tilemap.GetCellCenterLocal(pos);
    }

    private void SetMining(bool state)
    {
        if (isMining != state)
        {
            isMining = state;
            OnMiningStateChanged?.Invoke(isMining);
        }
    }

    private void OnDrawGizmos()
    {
        if (miningJoystick != null && playerTransform != null)
        {
            Gizmos.color = Color.red;
            float range = playerStats != null ? playerStats.MiningRange : 2.5f;
            Gizmos.DrawRay(playerTransform.position, miningJoystick.Direction.normalized * range);
        }
    }
}