using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

    public Quaternion rotation;
    public float smoothness;

    private void Awake()
    {
        rotation = transform.rotation;
    }

    void Update () {

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothness * Time.deltaTime);

	}

    public void SetRotation(Quaternion _rotation)
    {
        rotation = _rotation;
    }
}
