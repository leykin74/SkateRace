using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fin_serp_fx : MonoBehaviour
{

  [SerializeField] ParticleSystem FinParticle = null;
    // Start is called before the first frame update
    void Start()
    {
        FinParticle.Stop();
       // FinParticle.play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
         FinParticle.Play();
    }
}
