using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using NRKernal;

public class CharacterController : MonoBehaviour
{
    public float walkSpeed = 0.005f;
    private Animator animator;

    [SerializeField] GameObject playerHead;

    public Transform floor;
    [SerializeField] Transform environment;

    private Vector3 prePos;

    private bool isWaving = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        prePos = transform.position;
    }


    void Update()
    {
        if (NRInput.GetButtonDown(ControllerButton.APP))
            environment.transform.position -= floor.up * 0.1f;

        float x = NRInput.GetTouch().x;
        float y = NRInput.GetTouch().y;

        if (Utility.Distance2D_GT(x, y, 0.3f) && !isWaving)
        {
            animator.SetBool("isWalking", true);

            Vector3 vec = (transform.position - playerHead.transform.position).normalized;
            vec.y *= 0;

            float ang = Vector3.Angle(Vector3.forward, vec);
            if (playerHead.transform.position.x > 0) ang *= -1;

            transform.position = new Vector3(transform.position.x, floor.position.y, transform.position.z) + Quaternion.Euler(0, ang, 0) * new Vector3(x, 0, y) * walkSpeed;

            Vector3 diff = transform.position - prePos;
            transform.rotation = Quaternion.LookRotation(diff);

            prePos = transform.position;
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public IEnumerator Wave()
    {
        animator.SetTrigger("isWaving");
        isWaving = true;
        yield return new WaitForSeconds(4f);
        isWaving = false;
    }

}
