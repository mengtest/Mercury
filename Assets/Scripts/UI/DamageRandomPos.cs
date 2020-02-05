using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class DamageRandomPos : MonoBehaviour
{
    public Animator animator;
    public List<string> parmaName = new List<string> {"offsetX"};
    public float min = -1;
    public float max = 1;

    private void OnEnable()
    {
        foreach (var pn in parmaName)
        {
            var r = Random.Range(min, max);
            Debug.Log(r);
            animator.SetFloat(pn, r);
        }
    }
}