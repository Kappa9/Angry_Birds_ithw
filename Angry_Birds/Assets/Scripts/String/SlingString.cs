using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingString : MonoBehaviour {

    public Transform anchor,middleAnchor;
    public static Vector3 middleAnchorStartPos, delta, birdPos;
    public float radius;
    bool posUpdated=false;

	void Start () {
        middleAnchorStartPos = middleAnchor.position;
        birdPos = middleAnchor.position;
    }


    void FixedUpdate()
    {
        if (Birds.state == Birds.BirdState.Grabbed && GameManager.gameState != GameManager.GameState.End)
        {
            posUpdated = true;
            UpdatePosition();
        }
        else if(posUpdated)
        {
            transform.localPosition = anchor.localPosition;
            transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
            transform.localScale = Vector3.one;
            birdPos = middleAnchor.position;
            posUpdated = false;
        }
	}

    void UpdatePosition()
    {
        delta = GameManager.GetWorldMousePos() - middleAnchor.position;
        Vector3 deltaAnchor = GameManager.GetWorldMousePos() - anchor.position;
        if (delta.x * delta.x + delta.y * delta.y > 2.25)
        {
            delta = delta * radius / ((Vector2)delta).magnitude;
            deltaAnchor = deltaAnchor * radius / ((Vector2)delta).magnitude;
        }
        //endPos = 2 * anchor.position + delta;
        Vector3 endPos = middleAnchor.position+ anchor.position + delta;
        Vector3 endOfSlingPos = middleAnchor.position + delta;
        birdPos = middleAnchor.position + delta*0.9f;

        transform.position = new Vector3((endPos.x) / 2f,(endPos.y) / 2f, transform.position.z);
        transform.rotation = Quaternion.Euler(180f, 0f,Vector2.SignedAngle(delta, Vector2.right));
        float scalex = ((Vector2)anchor.position - (Vector2)(endOfSlingPos)).magnitude;
        float scaley = 2 / scalex;
        if (scaley > 1) scaley = 1;
        transform.localScale = new Vector3(scalex*13, scaley, 1);
    }
}
