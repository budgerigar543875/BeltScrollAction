using UnityEngine;

public class PlayerSpecialAttack : StateMachineBehaviour
{
    [SerializeField] float showColliderTiming = 0.3f;

    private Player player;
    bool alreadyShowCollider;
    private Vector3 moveVector;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        player = obj.GetComponent<Player>();
        alreadyShowCollider = false;
        int dir = obj.transform.localScale.x > 0 ? 1 : -1;
        moveVector = new Vector3(dir, 0f, 0f);
        player.SetSpecialAttack(true);
        player.SetInvincible(true);
        // ‘ŠŽè‚ª‚«”ò‚Ô‚æ‚¤‚É‚·‚é
        animator.SetInteger("comboCount", Player.COMBO_COUNT_STAN);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(layerIndex);
        float elapsedTime = clip[0].clip.length * stateInfo.normalizedTime;
        if(elapsedTime > showColliderTiming)
        {
            if(!alreadyShowCollider)
            {
                alreadyShowCollider = true;
                player.ShowSpecialAttackCollider();
                player.PlaySpecialAttackAudio();
            }
            player.Move(moveVector);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.HideSpecialAttackCollider();
        player.SetInvincible(false);
        player.SetSpecialAttack(false);
        player.StopAudio();
        animator.SetInteger("comboCount", 0);
    }
}
