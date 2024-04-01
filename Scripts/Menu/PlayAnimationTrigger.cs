using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
namespace RPG.Animations
{
    public class PlayAnimationTrigger : MonoBehaviour
    {
        [SerializeField] private string trigger;
        [SerializeField] private string value;
        [SerializeField] private float delay;
        [SerializeField] private int range;
        private Animator anim;
        private void Awake()
        {
            anim = GetComponent<Animator>();
        }
        private void Start()
        {
            StartCoroutine(PlayAnimation());
        }

        private IEnumerator PlayAnimation()
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                anim.SetInteger(value, (int)Random.Range(0, range));
                anim.SetTrigger(trigger);
            }
        }
        private void Hit()
        { }
        private void Land()
        { }
    }
}