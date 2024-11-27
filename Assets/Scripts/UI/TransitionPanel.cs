using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public IEnumerator StartTransitionPanelAnimation(string sceneName) {
        if (TransitionPanel.instance != null) {
            TransitionPanel.instance.FadeOutTransition();
        }

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneName);
    }
}
