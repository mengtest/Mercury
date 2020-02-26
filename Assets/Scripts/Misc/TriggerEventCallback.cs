using System;
using UnityEngine;

namespace Mercury
{
    public class TriggerEventCallback : MonoBehaviour
    {
        public event Action<Collider2D> OnTriggerEnterEvent;
        public event Action<Collider2D> OnTriggerStayEvent;
        public event Action<Collider2D> OnTriggerExitEvent;

        private void Awake()
        {
            if (!GetComponent<Collider2D>())
            {
                throw new InvalidOperationException();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) { OnTriggerEnterEvent?.Invoke(other); }

        private void OnTriggerStay2D(Collider2D other) { OnTriggerStayEvent?.Invoke(other); }

        private void OnTriggerExit2D(Collider2D other) { OnTriggerExitEvent?.Invoke(other); }
    }
}