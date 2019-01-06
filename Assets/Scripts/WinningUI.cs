using UnityEngine;
using UnityEngine.UI;

public class WinningUI : MonoBehaviour {

    public Text winningPlayerText;
    public Animator animator;
    public ScreenFader fader;

    public void AnimateWinnnerText(PlayerType playerType)
    {
        switch (playerType)
        {
            case PlayerType.BLUE:
                winningPlayerText.text = "BLUE HAS WON!";
                animator.SetTrigger("BluePlayerHasWon");
                break;
            case PlayerType.GREEN:
                winningPlayerText.text = "GREEN HAS WON!";
                animator.SetTrigger("GreenPlayerHasWon");
                break;
            case PlayerType.RED:
                winningPlayerText.text = "RED HAS WON!";
                animator.SetTrigger("RedPlayerHasWon");
                break;
            case PlayerType.YELLOW:
                winningPlayerText.text = "YELLOW HAS WON!";
                animator.SetTrigger("YellowPlayerHasWon");
                break;
        }
    }

    public void Credits()
    {
        fader.FadeTo("CreditsScene");
    }
	
}
