using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour {

    public bool doesExplode;

    public void DoOnDestroy()
    {
        if (doesExplode) Explode();
        Destroy(gameObject);
    }
	
    public void Explode()
    {
        A_Source.clip = boom;
        A_Source.Play();
        var expl = Instantiate(explosion, transform.position, Quaternion.identity);
        //get all the colliders within a 10 radius
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 13);
        Destroy(expl, 3);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].gameObject.tag == "Enemy")
            {
                //hitColliders [i].gameObject.GetComponent<EnemyBehavior> ().bomb = true;
                EnemyBehavior eb = hitColliders[i].gameObject.GetComponent<EnemyBehavior>();

                eb.TakeDamage(2);
                if (eb.hitPoints - 2 <= 0)
                {
                    eb.hasBombDeath = true;
                    hitColliders[i].gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    hitColliders[i].gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    float upForceModifier = 7f;
                    hitColliders[i].gameObject.GetComponent<Rigidbody>().AddExplosionForce(5000, this.transform.position, 15, upForceModifier);
                }

            }
            i += 1;
        }
    }

}
