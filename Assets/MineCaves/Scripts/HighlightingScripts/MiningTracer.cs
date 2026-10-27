using UnityEngine;

public class MiningTracer : MonoBehaviour
{

    [SerializeField] private FloatingJoystick miningJoystick;
    [SerializeField] private GameObject player;
    //[SerializeField] private Sprite dot; 
    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;

    private LayerMask minable;
    private float lineLength = 2.5f;

    private void Awake()
    {
        minable = LayerMask.GetMask("MiningTilemap", "MinableBlocks");
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (miningJoystick.Direction.magnitude > 0.05f)
        {
            spriteRenderer.enabled = true;
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, player.transform.position);

            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, miningJoystick.Direction.normalized, lineLength, minable);
            if (hit.collider != null)
            {   
                lineRenderer.SetPosition(1, hit.point);
                spriteRenderer.transform.position = hit.point;
            }
            else
            {
                lineRenderer.SetPosition(1, player.transform.position + (Vector3)(miningJoystick.Direction.normalized * lineLength));
                spriteRenderer.transform.position = player.transform.position + (Vector3)(miningJoystick.Direction.normalized * lineLength);
            }
            
        }
        else
        {
            spriteRenderer.enabled = false;
            lineRenderer.enabled = false;
        }
    }

}
