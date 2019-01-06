using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollDiceButton : MonoBehaviour {

    public Animator animator;
    public bool isInteractive;
    public PlayerType playerType;
    public DiceCube diceCube;
    private GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
    }

    private void Update()
    {
        isInteractive = gm.currentPlayer.playerType == playerType && gm.waitingForRoll == true && !diceCube.isRolling;
        animator.SetBool("isInteractive", isInteractive);
    }

    private void OnMouseDown()
    {
        if(isInteractive == true)
        {
            transform.position = transform.position - Vector3.up * 0.3f;
        }
    }

    private void OnMouseUp()
    {
        if(isInteractive == true)
        {
            transform.position = transform.position + Vector3.up * 0.3f;
            diceCube.StartCoroutine(diceCube.Roll());
        }
    }

}
