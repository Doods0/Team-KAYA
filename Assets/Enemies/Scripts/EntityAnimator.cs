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
    [Header("Particles")]
    [SerializeField] private GameObject damageParticle;
    [SerializeField] private string damageParticleId;
    [SerializeField] private GameObject damageNumberParticle;
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
        renderer.material.SetFloat("_FlashAmount", 1);

        if (damageParticle != null)
        {
            GameObject particle = GameManager.instance.GetFromPool(damageParticleId);
            if (particle == null) particle = Instantiate(damageParticle);
            else particle.SetActive(true);
            particle.transform.position = transform.position;
            particle.transform.rotation = Quaternion.identity;
            ParticleDriver driver = particle.GetComponent<ParticleDriver>();
            driver.id = damageParticleId;
        }

        if (damageNumberParticle != null)
        {
            GameObject particle = GameManager.instance.GetFromPool(damageNumberParticleId);
            if (particle == null) particle = Instantiate(damageNumberParticle);
            else particle.SetActive(true);
            particle.transform.position = transform.position;
            particle.transform.rotation = Quaternion.identity;
            DamageNumberDriver driver = particle.GetComponent<DamageNumberDriver>();
            driver.id = damageNumberParticleId;
            driver.Setup(damageAmount);
        }

        yield return new WaitForSeconds(duration);
        renderer.material.SetFloat("_FlashAmount", 0);
    }

    private void OnDisable() => renderer.material.SetFloat("_FlashAmount", 0);
}
