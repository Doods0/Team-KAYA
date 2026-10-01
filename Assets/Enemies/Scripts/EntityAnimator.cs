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
    private MaterialPropertyBlock flashingBlock;
    [Header("Particles")]
    [SerializeField] private ParticleController damageParticle;
    [SerializeField] private string damageParticleId;
    [SerializeField] private NumberParticleController damageNumberParticle;
    [SerializeField] private string damageNumberParticleId;
    

    private Dictionary<EntityState, AnimationRule> rulesDictionary;

    private Animator animator;
    private SpriteRenderer renderer;
    private int currentAnimation;

    private void Awake()
    {
        animator = character.GetComponent<Animator>();
        rulesDictionary = ruleSO.generateDictionary();
        renderer = character.GetComponent<SpriteRenderer>();
        flashingBlock = GameUtils.instance.flashingMaterial;
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

    public void OnDamageTaken(int damageAmount, float duration = 0.1f) 
    {
        if (gameObject.activeSelf) StartCoroutine(DamageEffects(damageAmount, duration));
    } 

    private IEnumerator DamageEffects(int damageAmount, float duration = 0.1f)
    {
        renderer.SetPropertyBlock(flashingBlock);

        if (damageParticle != null)
        {
            IPoolable particle = GameManager.instance.GetFromPool(damageParticleId) ?? Instantiate(damageParticle);
            if (particle is ParticleController controller)
                controller.Initialize(transform.position, damageParticleId);
        }

        if (damageNumberParticle != null)
        {
            IPoolable particle = GameManager.instance.GetFromPool(damageNumberParticleId) ?? Instantiate(damageNumberParticle);
            if (particle is NumberParticleController controller)
                controller.Initialize(transform.position, damageNumberParticleId, damageAmount);
        }

        yield return new WaitForSeconds(duration);

        renderer.SetPropertyBlock(null);
    }

    private void OnDisable()
    {
        renderer.SetPropertyBlock(null);
    }
}
