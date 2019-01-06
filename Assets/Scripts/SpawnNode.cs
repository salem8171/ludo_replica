using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNode : MonoBehaviour {

    public Transform nextNode;
    public Token token;
    public bool interactable = false;

    public float smoothness = 10f;

    private void Update()
    {
        if (token == null)
            return;

        if(token.tokenStatus == TokenStatus.LOCKED_IN_SPAWN)
        {
            token.tokenTransform.position = Vector3.Slerp(token.tokenTransform.position, GetPosition(), smoothness * Time.deltaTime);
        }
    }

    public Vector3 offset = new Vector3(0, 3, 0);

    public Vector3 GetPosition()
    {
        return transform.position + offset;
    }

    private void OnMouseDown()
    {
        Interact();
    }

    public void Interact()
    {
        if (interactable == true)
        {
            GameManager.instance.StartCoroutine(GameManager.instance.PlayWithChosenToken(token));
            interactable = false;
        }
    }

}
