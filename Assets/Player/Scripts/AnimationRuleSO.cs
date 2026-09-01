using System.Collections.Generic;
using UnityEngine;

public enum EntityState { Walking, Idle }

[System.Serializable]
public class AnimationRule
{
    public EntityState state;
    public AnimationClip track;
    public float fade;

    [HideInInspector] public int track_hash;
}

[CreateAssetMenu(menuName = "Visuals/AnimationRuleSO")]
public class AnimationRuleSO : ScriptableObject
{
    public List<AnimationRule> rules = new List<AnimationRule>();

    private void OnValidate()
    {
        foreach (AnimationRule rule in rules)
        {
            if (rule.track == null) { continue; }
            rule.track_hash = Animator.StringToHash(rule.track.name);
        }
    }

    public Dictionary<EntityState, AnimationRule> generateDictionary()
    {
        var dict = new Dictionary<EntityState, AnimationRule>();

        foreach (AnimationRule rule in rules)
        {
            if (rule.track == null) { continue; }

            dict[rule.state] = rule;
        }

        return dict;
    }
}
