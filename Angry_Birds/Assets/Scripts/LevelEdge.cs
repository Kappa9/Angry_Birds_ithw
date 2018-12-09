using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelEdge : MonoBehaviour {

    public GameObject Cam;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
        {
            Cam.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
