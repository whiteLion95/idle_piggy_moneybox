using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingAtCamera : MonoBehaviour
{
    [SerializeField] private bool _updateLook;
    [SerializeField] private bool _reverse;

    private Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void OnEnable()
    {
        LookAtCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (_updateLook)
            LookAtCamera();
    }

    private void LookAtCamera()
    {
        int koeff = -1;

        if (_reverse)
            koeff = 1;

        transform.LookAt(transform.position + (koeff * _mainCam.transform.forward));
    }
}