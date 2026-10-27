using UnityEngine;

public class TargetHighlighter : MonoBehaviour
{
    [SerializeField] Frame framePrefab;
    
    private Vector3 currentHighlightedPosition;
    private bool isHighlighting = false;
    private Frame currentFrame;

    public void highliteTarget(Vector3 position)
    { 
        if(isHighlighting && currentHighlightedPosition == position)
        { return; }
        else if (isHighlighting && currentHighlightedPosition != position)
        {
            currentFrame.animateJump(position);
        }
        else if (!isHighlighting)
        {
            currentFrame = Instantiate(framePrefab, position, Quaternion.identity);
            currentFrame.animateAppearence();
            isHighlighting = true;
            
        }
    }

    public void removeHighlight()
    {
        if(currentFrame == null) { return; }
        currentFrame.animateDisappearance();
        currentFrame = null;
    }

    public void highlight(bool _isHighlighting)
    { 
        isHighlighting = _isHighlighting;
    }

}
