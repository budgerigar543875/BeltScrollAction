using UnityEngine;

public class BossAttack : StateMachineBehaviour
{
    [SerializeField] float showColliderTiming = 0.2f;

    private Boss boss;
    bool alreadyWeaponDirectionChange;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.gameObject.GetComponent<Boss>();
        boss.ShowAttackCollider();
        alreadyWeaponDirectionChange = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(layerIndex);
        float elapsedTime = clip[0].clip.length * stateInfo.normalizedTime;
        if (!alreadyWeaponDirectionChange && elapsedTime > showColliderTiming)
        {
            alreadyWeaponDirectionChange = true;
            boss.WeaponDirectionChange();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.HideAttackCollider();
    }
}
