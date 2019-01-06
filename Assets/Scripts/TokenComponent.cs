using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenComponent : MonoBehaviour {

    public bool isColliding = false;
    public bool isInteractedWith = false;
    public Token tokenInstance;

    private void Start()
    {
        if (tokenInstance == null)
            return;
    }

    private void Update()
    {
        if (tokenInstance == null)
            return;
    }

    private void OnMouseDown()
    {
        tokenInstance.Interact();
    }

    private void OnMouseOver()
    {
        tokenInstance.Highlight();
    }

    private void OnMouseExit()
    {
        tokenInstance.Unhighlight();
    }

    void OnTriggerEnter(Collider other)
    {
        isColliding = true;
    }

    void OnTriggerExit(Collider other)
    {
        isColliding = false;
    }

}
