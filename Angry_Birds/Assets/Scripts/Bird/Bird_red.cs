using UnityEngine;

public class Bird_red : MonoBehaviour {

    bool canUseAbility = true;   //是否能使用能力

    AudioSource audioSource;
    public AudioClip[] crySound;
    public AudioClip[] collisionSound;
    
	void Start () {
        audioSource = GetComponent<AudioSource>();
    }

    void Update () {
        if (GameManager.cursorInLevel && gameObject.name.EndsWith(GameManager.birdNum.ToString()) && Input.GetMouseButtonDown(0) && Birds.state==Birds.BirdState.Shot && GetComponent<LineRenderer>() == null && canUseAbility && GameManager.gameState != GameManager.GameState.End)
        {
            canUseAbility = false;
            int cryRand = Random.Range(0, 3);
            if (cryRand == 3) cryRand = 2;
            audioSource.PlayOneShot(crySound[cryRand]);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.name.Contains((GameManager.birdNum - 1).ToString()))
        {
            canUseAbility = false;
            if (collision.relativeVelocity.magnitude >= 4f)
            {
                int birdCollsionRand = Random.Range(0, 4);
                if (birdCollsionRand == 4) birdCollsionRand = 0;
                audioSource.PlayOneShot(collisionSound[birdCollsionRand]);
            }
        }
    }
}
