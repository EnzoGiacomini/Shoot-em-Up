using System.Collections;
using UnityEngine;

public class Projectile_Player : MonoBehaviour
{
    //Stats bullet
    EntityStats player_stats;
    public bool canExplode = false;
    float explosion_area = 4;
    public GameObject explosion_enemy;
    public GameObject impact_boss;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_stats = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "boss")
        {
            //Se a explosao estiver ativo, da dano em area
            if (canExplode == true)
            {
                GameObject[] enemys_explosion = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject e_e in enemys_explosion)
                {
                    float dist_ = Vector2.Distance(e_e.transform.position, collision.gameObject.transform.position);

                    if (dist_ <= explosion_area)
                    {
                        e_e.gameObject.GetComponent<EntityStats>().TakeDamage(player_stats.atk_dmg);
                        Instantiate(explosion_enemy, e_e.transform.position, Quaternion.identity);
                    }
                }

                Destroy(this.gameObject);
            }
            //se nao causa o dano e cria a animao de impacto
            else if (canExplode == false)
            {
                collision.gameObject.GetComponent<EntityStats>().TakeDamage(player_stats.atk_dmg);

                if (collision.gameObject.tag == "Enemy")
                {
                    Instantiate(explosion_enemy, collision.transform.position, Quaternion.identity);
                }
                if (collision.gameObject.tag == "boss")
                {
                    Instantiate(impact_boss, collision.ClosestPoint(transform.position), Quaternion.identity);
                }

                Destroy(this.gameObject);
            }
        }
    }

}
