using UnityEngine;

public class Player {

    public string name;
    public Token[] tokens;
    public PlayerType playerType;

    public Player(PlayerType _playerType, Transform[] spawnNodes, Transform[] tokenTransforms)
    {
        playerType = _playerType;

        switch (playerType)
        {
            case PlayerType.BLUE:
                name = "Blue";
                break;
            case PlayerType.GREEN:
                name = "Green";
                break;
            case PlayerType.RED:
                name = "Red";
                break;
            case PlayerType.YELLOW:
                name = "Yellow";
                break;
        }

        tokens = new Token[4];

        for (int i = 0; i < 4; i++)
        {
            tokens[i] = new Token(playerType, spawnNodes[i], tokenTransforms[i]);
        }
    }
	
    public bool HasWon()
    {
        foreach (Token token in tokens)
        {
            if (token.tokenStatus != TokenStatus.WON)
                return false;
        }
        return true;
    }

}
