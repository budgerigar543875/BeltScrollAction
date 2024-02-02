using UnityEngine;

public class PlayerJumpAttack : StateMachineBehaviour
{
    [SerializeField] float showColliderTiming = 0.15f;
   
    private Player player;
    bool alreadyShowCollider;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.gameObject.GetComponent<Player>();
        alreadyShowCollider = false;
        // ‘ŠŽè‚ª‚«”ò‚Ô‚æ‚¤‚É‚·‚é
        animator.SetInteger("comboCount", Player.COMBO_COUNT_STAN);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(layerIndex);
        float elapsedTime = clip[0].clip.length * stateInfo.normalizedTime;
        if(!alreadyShowCollider && elapsedTime > showColliderTiming)
        {
            alreadyShowCollider = true;
            player.ShowJumpAttackCollider();
            player.PlaySlashAudio();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.HideJumpAttackCollider();
        animator.SetInteger("comboCount", 0);
        player.StopAudio();
    }
}
