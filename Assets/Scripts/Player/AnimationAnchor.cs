using UnityEngine;

public class AnimationAnchor : MonoBehaviour
{
    public Player player;
    public Head head;
    private Animator anchorAnimator;

    public void BackToIdle()
    {
        player.BackToIdle();
    }
}
