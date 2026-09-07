using System.Collections.Generic;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject character;

    [Header("Animations")]
    public EntityState state;
    [SerializeField] private AnimationRuleSO ruleSO;

    private Dictionary<EntityState, AnimationRule> rulesDictionary;

    private Animator animator;
    private int currentAnimation;

    private void Awake()
    {
        animator = character.GetComponent<Animator>();
        rulesDictionary = ruleSO.generateDictionary();
    }

    public virtual void Update()
    {
        // Animations section
        var currentRule = rulesDictionary[state];
        if (currentRule == null)  return;

        ChangeAnimation(currentRule.track_hash, currentRule.fade);
    }

    public void ChangeAnimation(int animation_hash, float fade = 0f)
    {
        if (currentAnimation != animation_hash)
        {
            currentAnimation = animation_hash;
            animator.CrossFade(animation_hash, fade);
        }
    }

    public void FlipCharacter(int direction)
    {
        if (direction == 0) return;
        transform.localScale = new Vector3(direction, 1, 1);
    }
}
