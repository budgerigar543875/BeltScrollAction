using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleText : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    [SerializeField] float scaleReductionSpeed;

    public event Action UpdateCompleted;

    void Start()
    {
        transform.localScale = new Vector3(20f, 20f, 0f);
    }

    void Update()
    {
        if (transform.localScale.x > 1f)
        {
            float speed = scaleReductionSpeed * Time.deltaTime;
            transform.localScale -= new Vector3(speed, speed, 1f);
            if (transform.localScale.x < 1f)
            {
                transform.localScale = Vector3.one;
            }
        }
        else
        {
            if (transform.rotation.z <= 0f)
            {
                transform.eulerAngles = Vector3.zero;
                if(UpdateCompleted != null)
                {
                    UpdateCompleted();
                    UpdateCompleted = null;
                }
                return;
            }
        }
        Vector3 rotate = transform.eulerAngles;
        rotate.z -= rotateSpeed * Time.deltaTime;
        transform.eulerAngles = rotate;
    }
}
