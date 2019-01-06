using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour {

    List<Token> tokens = new List<Token>();
    public RollDiceButton[] buttons;
    GameManager gm;
    public Vector3 offset;
    public float smoothness = 20;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    public bool isDynamic = true;

    public Camera cam;

    /*public float maxZoom;
    public float minZoom;
    public float zoomLimiter;*/

	// Use this for initialization
	void Start () {
        gm = GameManager.instance;
        foreach (Player player in gm.players)
        {
            foreach (Token token in player.tokens)
            {
                tokens.Add(token);
            }
        }
	}

    // Update is called once per frame
    private void LateUpdate()
    {
        if (isDynamic == false)
            return;

        List<Transform> targets = new List<Transform>();

        foreach (Token token in tokens)
        {
            if (token.tokenStatus == TokenStatus.FREE_TO_MOVE)
            {
                targets.Add(token.tokenTransform);
            }
        }

        foreach (RollDiceButton button in buttons)
        {
            if (button.isInteractive || gm.currentPlayer.playerType == button.playerType)
                targets.Add(button.transform);
        }

        if (targets.Count == 0)
            return;

        Vector3 center;
        //float maxDistance;

        if (targets.Count == 1)
        {
            center = new Vector3(targets[0].transform.position.x, 0, targets[0].transform.position.z);
            //maxDistance = 0;
        }

        else
        {
            var bounds = new Bounds(targets[0].transform.position, Vector3.zero);
            foreach (Transform target in targets)
            {
                bounds.Encapsulate(target.position);
            }
            
            center = new Vector3(bounds.center.x, 0, bounds.center.z);
            //maxDistance = bounds.size.x;
        }

        Vector3 newPosition = center + offset;
        newPosition = new Vector3(Mathf.Clamp(newPosition.x, minX, maxX), newPosition.y, Mathf.Clamp(newPosition.z, minZ, maxZ));
        transform.position = Vector3.Slerp(transform.position, newPosition, smoothness * Time.deltaTime);

        //float newZoom = Mathf.Lerp(minZoom, maxZoom, maxDistance / zoomLimiter);
        //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);

    }
}
