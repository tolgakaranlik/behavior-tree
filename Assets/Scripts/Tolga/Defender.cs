using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public Transform[] PatrolStations;
    public float Speed;

    public enum DefenderStates { Patrol, Attack }
    public DefenderStates DefenderCurrentState = DefenderStates.Patrol;
    public float FireSpeed = 1.5f;
    public GameObject Bullet;
    public float BulletSpeed = 0.1f;

    int currentPatrolStation = 0;
    GameObject target = null;
    public int Health = 25;

    int remaingngHealth;
    float lastShootTime = 0;
    Attacker attacker;

    private void Start()
    {
        remaingngHealth = Health;
    }

    void Update()
    {
        switch(DefenderCurrentState) { 
            case DefenderStates.Patrol:
                // bir sonraki devriye istasyonuna gitmek
                transform.LookAt(new Vector3(PatrolStations[currentPatrolStation].position.x, transform.position.y, PatrolStations[currentPatrolStation].position.z));

                if(Vector3.Distance(PatrolStations[currentPatrolStation].position, transform.position) < 0.25f)
                {
                    currentPatrolStation = (currentPatrolStation + 1) % PatrolStations.Length;
                }

                transform.position += transform.forward.normalized * Speed * Time.deltaTime;

                break;
            case DefenderStates.Attack:
                // saldırı modu
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));

                if(lastShootTime + FireSpeed < Time.time)
                {
                    // fire!
                    StartCoroutine(FireTo(target.transform.position + Vector3.up, Vector3.Distance(target.transform.position, transform.position) * BulletSpeed));

                    lastShootTime = Time.time;

                    if(attacker.Hit() <= 0)
                    {
                        // düşman ölmüştür
                        DefenderCurrentState = DefenderStates.Patrol;
                    }
                }

                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Debug.Log("Defender is switching to attack mode!");

            DefenderCurrentState = DefenderStates.Attack;
            target = other.gameObject;
            attacker = target.GetComponent<Attacker>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            target = null;
            DefenderCurrentState = DefenderStates.Patrol;
        }
    }

    public IEnumerator FireTo(Vector3 position, float duration)
    {
        // transform.position'da bir duman instantiate yap
        var bullet = Instantiate(Bullet, transform.position + Vector3.up, Quaternion.identity);
        bullet.transform.LookAt(position);
        bullet.transform.DOMove(position, duration).SetEase(Ease.Linear);

        yield return new WaitForSeconds(duration);

        // position'da bir duman instantiate yap
        Destroy(bullet);
    }

    public void Hit()
    {
        if(--remaingngHealth <= 0)
        {
            // duman instantiate yap
            // patlama sesi koy
            Destroy(gameObject);
        }
    }
}
