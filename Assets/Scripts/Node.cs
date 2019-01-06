using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public bool allowsKilling = true;
    public bool interactable = false;

    public Node nextNode;

    // Used to keep track of any player on top of the node
    public List<Token> bluePlayers = new List<Token>();
    public List<Token> greenPlayers = new List<Token>();
    public List<Token> redPlayers = new List<Token>();
    public List<Token> yellowPlayers = new List<Token>();
    public List<Token> players = new List<Token>();

    // Used to determine the player position on top of the node
    public Vector3 offset = new Vector3(0, 2, 0);
    
    // Used only if the node is an entrance for a specific player type
    public Node nextToEntranceNode;
    public bool isEntrance = false;
    public PlayerType affectedPlayerType;

    public GameObject posGOPrefab;
    public GameObject posGO;
    public float smoothness = .2f;
    public float[] scaleMultipliers = { 0.5f, 0.5f, 0.5f, 0.4f, 0.4f, 0.3f, 0.3f, 0.3f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };

    public Material defaultMaterial;
    public Material highLightMaterial;

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
        defaultMaterial = GetComponent<MeshRenderer>().material;
    }

    private void OnMouseDown()
    {
        Interact();
    }

    public void Interact()
    {
        if (interactable == true)
        {
            Unhighlight();
            Token chosenToken = null;
            switch (gm.currentPlayer.playerType)
            {
                case PlayerType.BLUE:
                    chosenToken = bluePlayers[0];
                    break;
                case PlayerType.GREEN:
                    chosenToken = greenPlayers[0];
                    break;
                case PlayerType.RED:
                    chosenToken = redPlayers[0];
                    break;
                case PlayerType.YELLOW:
                    chosenToken = yellowPlayers[0];
                    break;
            }
            gm.StartCoroutine(gm.PlayWithChosenToken(chosenToken));
        }
    }

    public Node GetNextNode(PlayerType playerType)
    {
        if(isEntrance)
        {
            if (playerType == affectedPlayerType)
                return nextToEntranceNode;
        }
        return nextNode;
    }

    public void AddPlayer(Token _token)
    {
        _token.SetParentNode(transform);
        players.Add(_token);
        if (players.Count == 1)
            posGO = Instantiate(posGOPrefab, GetPosition(), transform.rotation);
        switch (_token.tokenType)
        {
            case PlayerType.BLUE:
                bluePlayers.Add(_token);
                break;
            case PlayerType.GREEN:
                greenPlayers.Add(_token);
                break;
            case PlayerType.RED:
                redPlayers.Add(_token);
                break;
            case PlayerType.YELLOW:
                yellowPlayers.Add(_token);
                break;
        }
    }

    public void RemovePlayer(Token _token)
    {
        players.Remove(_token);
        if (players.Count == 0)
            Destroy(posGO);
        switch (_token.tokenType)
        {
            case PlayerType.BLUE:
                bluePlayers.Remove(_token);
                break;
            case PlayerType.GREEN:
                greenPlayers.Remove(_token);
                break;
            case PlayerType.RED:
                redPlayers.Remove(_token);
                break;
            case PlayerType.YELLOW:
                yellowPlayers.Remove(_token);
                break;
        }
    }

    public Token GetPlayerToKill(Token killer)
    {
        if (killer.tokenType != PlayerType.BLUE && bluePlayers.Count > 0)
            return bluePlayers[0];
        if (killer.tokenType != PlayerType.GREEN && greenPlayers.Count > 0)
            return greenPlayers[0];
        if (killer.tokenType != PlayerType.RED && redPlayers.Count > 0)
            return redPlayers[0];
        if (killer.tokenType != PlayerType.YELLOW && yellowPlayers.Count > 0)
            return yellowPlayers[0];
        return null;
    }

    public Vector3 GetPosition()
    {
        return transform.position + offset;
    }

    public Vector3 GetUpPosition()
    {
        return transform.position + 2 * offset;
    }

    public bool IsEmpty()
    {
        return (bluePlayers.Count + greenPlayers.Count + redPlayers.Count + yellowPlayers.Count) == 0;
    }

    public void Update()
    {
        int count = players.Count;
        if (count == 0)
            return;
        Vector3 direction;
        if(count == 1)
        {
            players[0].tokenTransform.localScale = Vector3.Slerp(players[0].tokenTransform.localScale, players[0].originalScale, smoothness);
            direction = Vector3.Slerp(Vector3.zero, GetPosition() - players[0].tokenTransform.position, smoothness);
            players[0].tokenTransform.Translate(direction);
            return;
        }
        Transform aux = posGO.transform.GetChild(count - 2);
        for (int i = 0; i < count; i++)
        {
            players[i].tokenTransform.localScale = Vector3.Slerp(players[i].tokenTransform.localScale, players[i].originalScale * scaleMultipliers[count - 2], smoothness);
            direction = Vector3.Slerp(Vector3.zero, aux.GetChild(i).position - players[i].tokenTransform.position, smoothness);
            players[i].tokenTransform.Translate(direction);
        }
    }

    public void Highlight()
    {
        if(interactable)
        {
            Node auxNode = this;
            for (int i = 0; i <= gm.dice.value; i++)
            {
                auxNode.GetComponent<MeshRenderer>().material = auxNode.highLightMaterial;
                if (auxNode.isEntrance && gm.currentPlayer.playerType == auxNode.affectedPlayerType)
                    auxNode = auxNode.nextToEntranceNode;
                else
                    auxNode = auxNode.nextNode;
            }
        }
    }

    public void Unhighlight()
    {
        if (interactable)
        {
            Node auxNode = this;
            for (int i = 0; i <= gm.dice.value; i++)
            {
                auxNode.GetComponent<MeshRenderer>().material = auxNode.defaultMaterial;
                if (auxNode.isEntrance && gm.currentPlayer.playerType == auxNode.affectedPlayerType)
                    auxNode = auxNode.nextToEntranceNode;
                else
                    auxNode = auxNode.nextNode;
            }
        }
    }

    private void OnMouseOver()
    {
        Highlight();
    }

    private void OnMouseExit()
    {
        Unhighlight();
    }
}
