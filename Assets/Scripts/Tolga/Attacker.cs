using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    public enum AttackerStates { MoveToTarget, Attack, Idle };
    public AttackerStates AttackerCurrentState = AttackerStates.MoveToTarget;
    public GameObject Target;
    public GameObject Defender = null;

    public float Speed = 3f;
    public float FireSpeed = 2f;
    public int Health = 10;
    public GameObject Bullet;
    public float BulletSpeed = 0.015f;

    int remaingngHealth;
    float lastShootTime = 0;
    Defender defender;

    private void Start()
    {
        remaingngHealth = Health;
    }

    void Update()
    {
        switch(AttackerCurrentState)
        {
            case AttackerStates.MoveToTarget:
                transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));

                transform.position += transform.forward.normalized * Speed * Time.deltaTime;
                break;
            case AttackerStates.Attack:
                transform.LookAt(new Vector3(Defender.transform.position.x, transform.position.y, Defender.transform.position.z));

                if (lastShootTime + FireSpeed < Time.time)
                {
                    // fire!
                    StartCoroutine(FireTo(Defender.transform.position + Vector3.up, Vector3.Distance(Defender.transform.position, transform.position) * BulletSpeed));
                    lastShootTime = Time.time;

                    defender.Hit();

                    if(Defender == null)
                    {
                        // düşman ölmüştür
                        AttackerCurrentState = AttackerStates.MoveToTarget;
                    }
                }

                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(AttackerCurrentState)
        {
            case AttackerStates.MoveToTarget:
                if(other.tag == "Target" || other.tag == "Player")
                {
                    Debug.Log("Attacker is switching to attack mode");
                    transform.LookAt(new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z));
                    AttackerCurrentState = AttackerStates.Attack;
                }

                if(other.tag == "Player")
                {
                    Defender = other.gameObject;
                    defender = Defender.GetComponent<Defender>();
                }

                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch(AttackerCurrentState)
        {
            case AttackerStates.Attack:
                if(other.tag == "Player")
                {
                    AttackerCurrentState = AttackerStates.MoveToTarget;
                } else if(other.tag == "Target")
                {
                    if (Target == null)
                    {
                        AttackerCurrentState = AttackerStates.Idle;
                    } else
                    {
                        AttackerCurrentState = AttackerStates.MoveToTarget;
                    }
                }

                break;
        }
    }

    public IEnumerator FireTo(Vector3 position, float duration)
    {
        // transform.position'da bir duman instantiate yap
        var bullet = Instantiate(Bullet, transform.position, Quaternion.identity);
        bullet.transform.LookAt(position);
        bullet.transform.DOMove(position, duration).SetEase(Ease.Linear);

        yield return new WaitForSeconds(duration);

        // position'da bir duman instantiate yap
        Destroy(bullet);
    }

    public int Hit()
    {
        if (--remaingngHealth <= 0)
        {
            // duman instantiate yap
            // patlama sesi koy
            gameObject.transform.localScale = Vector3.zero;
            AttackerCurrentState = AttackerStates.Idle;
        }

        return remaingngHealth;
    }
}
