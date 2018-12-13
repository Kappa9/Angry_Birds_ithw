using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelEdge : MonoBehaviour {

    public GameObject Cam;  //摄像机

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")       //与相机碰撞之后使相机速度变为0
        {
            Cam.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
