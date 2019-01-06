using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    #region singleton
    public static GameManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one GameManager in the scene");
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    [SerializeField]
    private static int _numberOfPlayers = 4;
    public static int NumberOfPlayers
    {
        get { return _numberOfPlayers; }
        set { _numberOfPlayers = Mathf.Clamp(value, 2, 4); }
    }

    [SerializeField]
    private int _currentPlayerIndex;
    private int CurrentPlayerIndex
    {
        get { return _currentPlayerIndex; }
        set
        {
            if (value >= NumberOfPlayers)
            {
                _currentPlayerIndex = 0;
            }
            else
            {
                _currentPlayerIndex = value;
            }
        }
    }

    public float minRangeError;
    public float smoothness;

    private bool hasKilled = false;

    public List<Player> players = new List<Player>();
    public Player currentPlayer;

    public bool waitingForRoll = true;
    public DiceCube dice;

    public WinningUI winningUI;
    public GameObject gameOverUI;

    // Token prefabs
    public GameObject blueTokenPrefab;
    public GameObject greenTokenPrefab;
    public GameObject redTokenPrefab;
    public GameObject yellowTokenPrefab;

    // All spawnNodes as transforms
    public Transform[] blueSpawnNodesTransforms;
    public Transform[] greenSpawnNodesTransforms;
    public Transform[] redSpawnNodesTransforms;
    public Transform[] yellowSpawnNodesTransforms;

    // Death effects prefabs
    public GameObject blueDeathEffectPrefab;
    public GameObject greenDeathEffectPrefab;
    public GameObject redDeathEffectPrefab;
    public GameObject yellowDeathEffectPrefab;

    // Life effects prefabs
    public GameObject blueLifeEffectPrefab;
    public GameObject greenLifeEffectPrefab;
    public GameObject redLifeEffectPrefab;
    public GameObject yellowLifeEffectPrefab;

    private void Start()
    {
        SetupGame();
    }

    public void BeginTurn()
    {
        int interactablesNumber = 0;
        Node nextNode;
        foreach (Token token in currentPlayer.tokens)
        {
            if (token.tokenStatus == TokenStatus.LOCKED_IN_SPAWN && dice.value == 6)
            {
                token.originalSpawnNodeComponent.interactable = true;
                token.tokenTransform.GetChild(0).gameObject.SetActive(true);
                interactablesNumber++;
                continue;
            }

            if(token.tokenStatus == TokenStatus.FREE_TO_MOVE)
            {
                nextNode = token.GetParentNodeComponent();
                for (int i = 1; i <= dice.value; i++)
                {
                    nextNode = nextNode.GetNextNode(currentPlayer.playerType);
                    if(nextNode == null)
                    {
                        break;
                    }
                }
                if(nextNode != null)
                {
                    token.GetParentNodeComponent().interactable = true;
                    token.tokenTransform.GetChild(0).gameObject.SetActive(true);
                    interactablesNumber++;
                }
            }
        }

        if (interactablesNumber == 0)
        {
            currentPlayer = GetNextPlayer();
            waitingForRoll = true;
        }
        
        if(interactablesNumber == 1)
        {
            foreach (Token token in currentPlayer.tokens)
            {
                if(token.tokenStatus == TokenStatus.LOCKED_IN_SPAWN && dice.value == 6)
                {
                    StartCoroutine(PlayWithChosenToken(token));
                    break;
                }
                if(token.tokenStatus == TokenStatus.FREE_TO_MOVE)
                    if(token.GetParentNodeComponent().interactable == true)
                    {
                        StartCoroutine(PlayWithChosenToken(token));
                        break;
                    }
            }
        }

    }

    public IEnumerator PlayWithChosenToken(Token token)
    {
        ResetInteractables();
        // The chosen token can only be ready to be spawned (in which case the player rolled 6) 
        // or a free token that CAN move taking rolled number into account.

        if (token.tokenStatus == TokenStatus.LOCKED_IN_SPAWN)
        // The chosen token is not yet spawned. This spawns it. Same player will play the next turn.
        {
            token.originalSpawnNodeComponent.interactable = false;
            token.Spawn();
            token.GetParentNodeComponent().AddPlayer(token);
            waitingForRoll = true;
        }
        else
        // The chosen token is free to move in this case.
        {
            // Finding path minus the last step.
            Vector3 direction;
            Vector3 targetPosition;
            Node nextNode = token.GetParentNodeComponent();
            token.GetParentNodeComponent().RemovePlayer(token);
            for (int i = 0; i < dice.value - 1; i++)
            {
                nextNode = nextNode.GetNextNode(currentPlayer.playerType);
                //move towards it
                if (nextNode.IsEmpty())
                    targetPosition = nextNode.GetPosition();
                else
                    targetPosition = nextNode.GetUpPosition();
                while (true)
                {
                    token.tokenTransform.localScale = Vector3.Slerp(token.tokenTransform.localScale, token.originalScale, smoothness * Time.deltaTime);
                    direction = targetPosition - token.tokenTransform.position;
                    if (direction.magnitude <= minRangeError)
                        break;
                    direction = Vector3.Slerp(Vector3.zero, direction, smoothness * Time.deltaTime);
                    token.tokenTransform.Translate(direction);
                    yield return 0;
                }
            }

            // Last step is an edge case. 4 scenarios may take place
            // 1) Next node allows killing, is not empty and nothing to kill (friendly tokens)
            // 2) Next node doesn't allow killing and is not empty
            // 3) Next node allows killing, is not empty and there are token(s) to kill
            // 4) Next node is empty
            // 1 & 2 can be treated together 3 & 4 also
            // Last step code
            nextNode = nextNode.GetNextNode(currentPlayer.playerType);
            if ((nextNode.allowsKilling && !nextNode.IsEmpty() && nextNode.GetPlayerToKill(token) == null) ||
                !nextNode.allowsKilling && !nextNode.IsEmpty())
            {
                nextNode.AddPlayer(token);
            }
            else
            {
                // Move normally.
                // Kill any opponent token if possible.
                while (true)
                {
                    if (token.IsColliding())
                    {
                        // Kill opponent token(s)
                        Token opponentToken = nextNode.GetPlayerToKill(token);
                        while(opponentToken != null)
                        {
                            nextNode.RemovePlayer(opponentToken);
                            StartCoroutine(KillOpponent(opponentToken));
                            opponentToken = nextNode.GetPlayerToKill(token);
                            hasKilled = true;
                        }
                    }
                    direction = nextNode.GetPosition() - token.tokenTransform.position;
                    if (direction.magnitude <= minRangeError)
                        break;
                    direction = Vector3.Slerp(Vector3.zero, direction, smoothness * Time.deltaTime);
                    token.tokenTransform.Translate(direction);
                    yield return 0;
                }
                
                nextNode.AddPlayer(token);
            }

            // Did we win? Is the game over?
            if (token.GetParentNodeComponent().GetNextNode(currentPlayer.playerType) == null)
            {
                token.tokenStatus = TokenStatus.WON;
                if (currentPlayer.HasWon())
                {
                    if (GameIsOver())
                    {
                        EndGame();
                        yield break;
                    }
                    else
                    {
                        winningUI.AnimateWinnnerText(currentPlayer.playerType);
                    }

                }
            }

            // Prepare for the next turn.
            if (!(token.tokenStatus == TokenStatus.WON && !currentPlayer.HasWon() || dice.value == 6 || hasKilled))
            {
                currentPlayer = GetNextPlayer();
            }
            hasKilled = false;
            waitingForRoll = true;
        }
    }
    
    IEnumerator KillOpponent(Token opponentToken)
    {
        // Instanciate death effect
        GameObject deathEffect = null;
        switch (opponentToken.tokenType)
        {
            case PlayerType.BLUE:
                deathEffect = Instantiate(blueDeathEffectPrefab, opponentToken.tokenTransform.position, opponentToken.tokenTransform.rotation);
                break;
            case PlayerType.GREEN:
                deathEffect = Instantiate(greenDeathEffectPrefab, opponentToken.tokenTransform.position, opponentToken.tokenTransform.rotation);
                break;
            case PlayerType.RED:
                deathEffect = Instantiate(redDeathEffectPrefab, opponentToken.tokenTransform.position, opponentToken.tokenTransform.rotation);
                break;
            case PlayerType.YELLOW:
                deathEffect = Instantiate(yellowDeathEffectPrefab, opponentToken.tokenTransform.position, opponentToken.tokenTransform.rotation);
                break;
        }
        Destroy(deathEffect, 3f);
        opponentToken.tokenTransform.GetComponent<MeshRenderer>().enabled = false;
        opponentToken.Despawn();
        yield return new WaitForSeconds(1f);
        opponentToken.tokenTransform.localScale = opponentToken.originalScale;
        opponentToken.tokenTransform.GetComponent<MeshRenderer>().enabled = true;
        // Instanciate life effect
        GameObject lifeEffect = null;
        switch (opponentToken.tokenType)
        {
            case PlayerType.BLUE:
                lifeEffect = Instantiate(blueLifeEffectPrefab, opponentToken.tokenTransform.position, opponentToken.tokenTransform.rotation);
                break;
            case PlayerType.GREEN:
                lifeEffect = Instantiate(greenLifeEffectPrefab, opponentToken.tokenTransform.position, opponentToken.tokenTransform.rotation);
                break;
            case PlayerType.RED:
                lifeEffect = Instantiate(redLifeEffectPrefab, opponentToken.tokenTransform.position, opponentToken.tokenTransform.rotation);
                break;
            case PlayerType.YELLOW:
                lifeEffect = Instantiate(yellowLifeEffectPrefab, opponentToken.tokenTransform.position, opponentToken.tokenTransform.rotation);
                break;
        }
        Destroy(lifeEffect, 3f);
    }

    void ResetInteractables()
    {
        foreach (Token token in currentPlayer.tokens)
        {
            if(token.tokenStatus == TokenStatus.LOCKED_IN_SPAWN)
            {
                token.originalSpawnNodeComponent.interactable = false;
                token.tokenTransform.GetChild(0).gameObject.SetActive(false);
            }
            if(token.tokenStatus == TokenStatus.FREE_TO_MOVE)
            {
                token.GetParentNodeComponent().interactable = false;
                token.tokenTransform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    Player GetNextPlayer()
    {
        CurrentPlayerIndex++;
        while (players[CurrentPlayerIndex].HasWon())
            CurrentPlayerIndex++;
        return players[CurrentPlayerIndex];
    }

    void SetupGame()
    {
        switch (NumberOfPlayers)
        {
            case 2:
                players.Add(SetupPlayer(PlayerType.BLUE, blueTokenPrefab, blueSpawnNodesTransforms));
                players.Add(SetupPlayer(PlayerType.GREEN, greenTokenPrefab, greenSpawnNodesTransforms));
                break;
            case 3:
                players.Add(SetupPlayer(PlayerType.BLUE, blueTokenPrefab, blueSpawnNodesTransforms));
                players.Add(SetupPlayer(PlayerType.RED, redTokenPrefab, redSpawnNodesTransforms));
                players.Add(SetupPlayer(PlayerType.GREEN, greenTokenPrefab, greenSpawnNodesTransforms));
                break;
            case 4:
                players.Add(SetupPlayer(PlayerType.BLUE, blueTokenPrefab, blueSpawnNodesTransforms));
                players.Add(SetupPlayer(PlayerType.RED, redTokenPrefab, redSpawnNodesTransforms));
                players.Add(SetupPlayer(PlayerType.GREEN, greenTokenPrefab, greenSpawnNodesTransforms));
                players.Add(SetupPlayer(PlayerType.YELLOW, yellowTokenPrefab, yellowSpawnNodesTransforms));
                break;
        }
        CurrentPlayerIndex = Random.Range(0, NumberOfPlayers);
        currentPlayer = players[CurrentPlayerIndex];
    }

    Player SetupPlayer(PlayerType _playerType, GameObject tokenPrefab, Transform[] _spawnNodes)
    {
        Transform[] tokenTransforms = new Transform[4];
        for (int i = 0; i < 4; i++)
        {
            tokenTransforms[i] = Instantiate(tokenPrefab).transform;
        }
        return new Player(_playerType, _spawnNodes, tokenTransforms);
    }

    void EndGame()
    {
        Debug.Log("GAME OVER!");
        gameOverUI.SetActive(true);
    }

    public bool GameIsOver()
    {
        int winners = 0;
        foreach (Player player in players)
        {
            if (player.HasWon())
                winners++;
        }
        return winners == NumberOfPlayers - 1;
    }

}
