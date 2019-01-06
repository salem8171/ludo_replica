using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token {

    public Transform tokenTransform;
    public PlayerType tokenType;
    public Transform originalSpawnNode;
    public SpawnNode originalSpawnNodeComponent;
    public Transform parentNode;
    public Vector3 originalScale;
    public TokenComponent tokenComponent;

    public TokenStatus tokenStatus;

    public void SetParentNode(Transform _parentNode)
    {
        parentNode = _parentNode;
    }

    public Node GetParentNodeComponent()
    {
        if (tokenStatus == TokenStatus.LOCKED_IN_SPAWN)
            return null;
        return parentNode.GetComponent<Node>();
    }

    public Token(PlayerType playerType, Transform spawnNode, Transform _tokenTransform)
    {
        tokenType = playerType;
        originalSpawnNode = spawnNode;
        originalSpawnNodeComponent = originalSpawnNode.GetComponent<SpawnNode>();
        originalSpawnNodeComponent.token = this;

        tokenTransform = _tokenTransform;
        tokenTransform.SetPositionAndRotation(originalSpawnNodeComponent.GetPosition(), Quaternion.identity);
        tokenStatus = TokenStatus.LOCKED_IN_SPAWN;

        originalScale = tokenTransform.localScale;

        tokenComponent = tokenTransform.GetComponent<TokenComponent>();
        tokenComponent.tokenInstance = this;

    }

    public void Despawn()
    {
        tokenTransform.SetPositionAndRotation(originalSpawnNodeComponent.GetPosition(), Quaternion.identity);
        SetParentNode(originalSpawnNode);
        tokenStatus = TokenStatus.LOCKED_IN_SPAWN;
    }

    public void Spawn()
    {
        SetParentNode(originalSpawnNodeComponent.nextNode);
        tokenStatus = TokenStatus.FREE_TO_MOVE;
    }

    public bool IsColliding()
    {
        return tokenTransform.GetComponent<TokenComponent>().isColliding;
    }

    public void Interact()
    {
        if(tokenStatus == TokenStatus.LOCKED_IN_SPAWN)
        {
            if(originalSpawnNodeComponent.interactable == true)
            {
                originalSpawnNodeComponent.Interact();
                return;
            }
        }

        if (tokenStatus == TokenStatus.FREE_TO_MOVE)
        {
            if(GetParentNodeComponent().interactable == true)
            {
                GetParentNodeComponent().Interact();
            }
        }
    }

    public void Highlight()
    {
        if (tokenStatus == TokenStatus.FREE_TO_MOVE)
        {
            if (GetParentNodeComponent().interactable == true)
            {
                GetParentNodeComponent().Highlight();
            }
        }
    }

    public void Unhighlight()
    {
        if (tokenStatus == TokenStatus.FREE_TO_MOVE)
        {
            if (GetParentNodeComponent().interactable == true)
            {
                GetParentNodeComponent().Unhighlight();
            }
        }
    }

}

public enum PlayerType {BLUE, GREEN, RED, YELLOW}
public enum TokenStatus {LOCKED_IN_SPAWN, FREE_TO_MOVE, WON}
