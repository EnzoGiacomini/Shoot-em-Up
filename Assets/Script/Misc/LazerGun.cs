using UnityEngine;

public class LazerGun : MonoBehaviour
{
    public int projectile_dmg;
    float cooldown_dmg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cooldown_dmg = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown_dmg > 0)
        {
            cooldown_dmg -= Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Causa dano mas apenas 1 segundo após pra nao ficar causando dano super rapidamente
        if (collision.gameObject.tag == "Player" && cooldown_dmg <= 0)
        {
            collision.gameObject.GetComponent<EntityStats>().TakeDamage(projectile_dmg);
            cooldown_dmg = 1;
        }
    }
}
