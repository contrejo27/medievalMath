using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wizardBarrierBehavior : MonoBehaviour {

    Animator m_Animator;
    string animationName = "wizardBarrierPop";

    void Start() {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat("Direction", 1.0f);
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            Toggle();
        }
        if (Input.GetKey(KeyCode.Alpha1)) {
            Rise();
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            Sink();
        }
    }


    private void Toggle() {
        /// <summary>
        /// Summons/returns the rock underground
        /// </summary>

        m_Animator.SetFloat("Direction", (-1.0f) * m_Animator.GetFloat("Direction"));
        m_Animator.Play(animationName);
    }

    private void Rise() {
        /// <summary>
        /// Summons the rock from underground
        /// </summary>
        m_Animator.SetFloat("Direction", 1.0f);
        m_Animator.Play(animationName);
    }

    private void Sink() {
        /// <summary>
        /// Returns the rock underground
        /// </summary>
        m_Animator.SetFloat("Direction", -1.0f);
        m_Animator.Play(animationName);
    }
}
