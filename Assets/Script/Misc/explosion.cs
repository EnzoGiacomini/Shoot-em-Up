using UnityEngine;

public class explosion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Só pra animação de exploção sumir dps de meio segundo
        Destroy(this.gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
