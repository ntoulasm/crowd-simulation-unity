using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
    
    private Animator animator = null;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Start() {
        
    }

    public void Walk() {
        animator.SetBool("Walking", true);
    }

    public void StopWalk() {
        animator.SetBool("Walking", false);
    }

    public void SetSpeed(float speed) {
        animator.SetFloat("Speed", speed);
    }

    public void SetOffset(float offset) {
        animator.SetFloat("Offset", offset);
    }

}
