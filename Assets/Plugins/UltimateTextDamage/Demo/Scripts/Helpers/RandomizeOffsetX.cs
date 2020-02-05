using UnityEngine;

namespace Guirao.UltimateTextDamage
{
    public class RandomizeOffsetX : MonoBehaviour
    {
        public string offsetParamName = "offsetX";
        public Animator animator;

        private void OnEnable()
        {
            animator.SetFloat(offsetParamName, Random.value);
        }
    }
}