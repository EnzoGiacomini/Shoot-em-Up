using System.Collections;
using TMPro;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    //Stats
    public float speed_;
    public float hp_max;
    public float hp_;
    public float atk_dmg;
    public float atk_speed;
    public int points_carry;

    //PowerUp
    public bool HasShield;

    //Animation
    public GameObject pointPopUp_;
    public GameObject explosion_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp_ = hp_max;
        HasShield = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Inimigo pisca vermelho ao receber dano
    IEnumerator BlinkRed(GameObject boss)
    {

        boss.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(0.15f);

        boss.GetComponent<SpriteRenderer>().color = new Color(0.77f, 0.99f, 0.52f, 1);
    }
    //Player pisca vermelho ao receber dano
    IEnumerator BlinkRedP(GameObject player)
    {

        player.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(0.15f);

        player.GetComponent<SpriteRenderer>().color = Color.white;
    }
    //Void de dano
    public void TakeDamage(float dmg_)
    {
        //Se tiver shield nao toma dano.
        if (HasShield == true)
        {
            return;
        }
        else
        {
            //Recebe o dano
            hp_ -= dmg_;
            //Som de dano
            InventoryManager.Instance.StartSound();

            //Tira a vida do slider e pisca vermelho
            if (gameObject.tag == "boss")
            {
                InventoryManager.Instance.RefreshHUD();
                StartCoroutine(BlinkRed(gameObject));
            }

            if (gameObject.tag == "Player")
            {
                InventoryManager.Instance.RefreshHUD();
                StartCoroutine(BlinkRedP(gameObject));
            }
        }

        Death();
    }
    //Void de morte
    void Death()
    {
        if (hp_ <= 0)
        {
            if (gameObject.tag == "Enemy")
            {
                //PopUP de ponto
                EntityStats enemy_stats = this.gameObject.GetComponent<EntityStats>();

                GameObject point_Instance = Instantiate(pointPopUp_, gameObject.transform.position, Quaternion.identity);
                point_Instance.GetComponentInChildren<TMP_Text>().text = "+" + enemy_stats.points_carry.ToString();

                Destroy(point_Instance, 0.8f);

                //Adiciona os pontos
                InventoryManager.Instance.AddPoints(points_carry);
                //Roda probabilidade de powerup
                gameObject.GetComponent<SimpleEnemy>().PowerUpChance();

                Destroy(this.gameObject);
            }
            if (gameObject.tag == "Player")
            {
                //Para os laser se estiverem ativos
                InventoryManager.Instance.DisableLaserConti();
                //Para a musica de boss se estiver ativo
                SpawnManager.Instance.finalboss_.Stop();
                //Tela de gameover
                gameObject.GetComponent<PlayerScript>().GameOver();

                Destroy(this.gameObject);
            }
            if (gameObject.tag == "boss")
            {
                //Inicia a animação de morte
                gameObject.GetComponent<FinalBossScript>().Death();
                //Para os ataques e destroi os laser
                gameObject.GetComponent<FinalBossScript>().StopAllAttacks();
                gameObject.GetComponent<FinalBossScript>().DestroyActiveLasers();
            }
        }
    }

}
