using UnityEngine;

public class TransitionPanel : MonoBehaviour
{
    public static TransitionPanel instance;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        instance = this;
    }

    public void FadeOutTransition()
    {
        animator.Play("FadeOut");
    }
}
