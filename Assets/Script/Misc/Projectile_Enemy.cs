using UnityEngine;

public class Projectile_Enemy : MonoBehaviour
{
    public GameObject impac_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Da dano e animação
        if (collision.gameObject.tag == "Player")
        {
            GameObject impac_instance = Instantiate(impac_, collision.gameObject.transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<EntityStats>().TakeDamage(5);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "wall")
        {
            Destroy(this.gameObject);
        }
    }

}
