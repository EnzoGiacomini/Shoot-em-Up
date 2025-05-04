using UnityEngine;

public class PowerUp : MonoBehaviour
{
    GameObject player_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_ = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }
    //Acha o player e vai ao encontro dele
   void Movement()
    {
        transform.position = Vector2.MoveTowards(gameObject.transform.position ,player_.transform.position, 3.5f * Time.deltaTime);
    }
}
