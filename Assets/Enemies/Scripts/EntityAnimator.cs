using System.Collections;
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
    private SpriteRenderer renderer;
    private int currentAnimation;

    private void Awake()
    {
        animator = character.GetComponent<Animator>();
        rulesDictionary = ruleSO.generateDictionary();
        renderer = character.GetComponent<SpriteRenderer>();
    }

    public virtual void Update()
    {
        // Animations section
        var currentRule = rulesDictionary[state];
        if (currentRule == null)  return;

        animator.speed = GameManager.instance.timeScale;
        if (GameManager.instance.timeScale != 0) ChangeAnimation(currentRule.track_hash, currentRule.fade);
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
        if (direction == 0 || GameManager.instance.timeScale == 0) return;
        transform.localScale = new Vector3(direction, 1, 1);
    }

    public void OnDamageTaken(float duration = 0.1f) => StartCoroutine(DamageEffects(duration));

    private IEnumerator DamageEffects(float duration = 0.1f)
    {
        renderer.material.SetFloat("_FlashAmount", 1) ;
        yield return new WaitForSeconds(duration);
        renderer.material.SetFloat("_FlashAmount", 0);
    }

    private void OnDisable() => renderer.material.SetFloat("_FlashAmount", 0);
}
