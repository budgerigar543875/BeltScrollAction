using UnityEngine;

public class PlayerNomalAttack : StateMachineBehaviour
{
    [SerializeField] float showColliderTiming = 0.15f;

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
            player.PlaySlashAudio();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.HideNomalAttackCollider();
        player.UpdateHitCount();
        player.StopAudio();
    }
}
