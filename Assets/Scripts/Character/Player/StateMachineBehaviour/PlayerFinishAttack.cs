using UnityEngine;

public class PlayerFinishAttack : StateMachineBehaviour
{
    [SerializeField] float showColliderTiming;

    private Player player;
    bool alreadyShowCollider;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.gameObject.GetComponent<Player>();
        alreadyShowCollider = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(layerIndex);
        float elapsedTime = clip[0].clip.length * stateInfo.normalizedTime;
        if (!alreadyShowCollider && elapsedTime > showColliderTiming)
        {
            alreadyShowCollider = true;
            player.ShowNomalAttackCollider();
            player.ShowFireball();
            player.PlayFireballAudio();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.HideNomalAttackCollider();
        player.HideFireball();
        player.StopAudio();
    }
}
