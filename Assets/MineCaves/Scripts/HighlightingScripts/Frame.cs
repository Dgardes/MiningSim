using UnityEngine;
using DG.Tweening;

public class Frame :MonoBehaviour
{
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void animateAppearence()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(this.transform.DOScale(1.5f, 0.01f));
        sequence.Join(this.sr.DOFade(1.0f, 0.01f));
        sequence.Append(this.transform.DOScale(1.0f, 0.5f))
        .OnComplete(() => this.transform.DOScale(1.1f, 0.4f).SetLoops(-1, LoopType.Yoyo)); 
    }

    public void animateDisappearance()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(this.sr.DOFade(0.0f, 0.5f));
        sequence.Append(this.transform.DOScale(1.5f, 0.01f))
        .OnComplete(() => Destroy(this.gameObject));
    }

    public void animateJump(Vector3 newPos)
    {
        this.transform.DOMove(newPos, 0.5f).SetEase(Ease.OutExpo);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
