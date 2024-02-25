using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(_cam.transform.forward + transform.position, Vector3.up);
    }

    private void OnEnable()
    {
        transform.LookAt(_cam.transform.forward + transform.position, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
