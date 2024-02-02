using UnityEngine;

public class EnemyAttack : StateMachineBehaviour
{
    [SerializeField] float showColliderTiming = 0.15f;

    private EnemyBase enemy;
    bool alreadyShowCollider;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<EnemyBase>();
        alreadyShowCollider = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(layerIndex);
        float elapsedTime = clip[0].clip.length * stateInfo.normalizedTime;
        if (!alreadyShowCollider && elapsedTime > showColliderTiming)
        {
            alreadyShowCollider = true;
            enemy.ShowAttackCollider();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.HideAttackCollider();
    }
}
