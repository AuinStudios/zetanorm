// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this gunbulletforce do?
/// </summary>
public sealed class gunbulletforce : MonoBehaviour
{
    public enum Direction
    {
        up,
        down,
        right,
        left,
    }
    public enum Type
    {
        coin,
        bullet,
        stickycoin,
    }
    [SerializeField] private gunscriptableobject gunproperty;
    [SerializeField] private Rigidbody2D rig;
    [SerializeField] private Direction direct = Direction.right;
    [SerializeField] private Type typeofthing = Type.bullet;
    private Transform target;
    // Start is called before the first frame update
    private void Start()
    {
        switch (typeofthing)
        {
            case Type.coin:
                rig.AddForce(new Vector3(transform.right.x * gunproperty.coinforce / 4f, dir().y * gunproperty.coinforce, 0), ForceMode2D.Impulse);
                StartCoroutine(getarget());
                break;
            case Type.bullet:
                rig.AddForce(transform.right * gunproperty.bulletspeed, ForceMode2D.Impulse);

                break;
            case Type.stickycoin:
                break;
            default:
                break;
        }

    }
    private Transform getcloseistenemy()
    {
        target = null;
        float lastdistance = Mathf.Infinity;
        float dSqrToTarget = 0.0f;
        foreach (Transform i in enemymanager.Instance.enemys)
        {
            Vector3 distance = i.position - transform.position;
            dSqrToTarget = distance.sqrMagnitude;
            if ( dSqrToTarget < lastdistance)
            {
                lastdistance = dSqrToTarget;
                target = i.transform;

            }
        }
        return target;
    }
    private IEnumerator getarget()
    {
        target = getcloseistenemy();
        if (target == null)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Random.Range(0, 360));
        }
        yield return new WaitUntil(() => target != null);
        // make inf loop for the realtime looking direction untll the coin breaks
        //  Rigidbody2D temp = target.GetComponent<Rigidbody2D>();
        while (0 < Mathf.Infinity)
        {
            var dir = target.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            yield return new WaitForFixedUpdate();
        }
    }
    //Vector3 PredictPosition(GameObject target)
    //{
    //    Vector3 velocity = target.GetComponent<Rigidbody2D>().velocity;
    //    float time = Vector3.Distance(transform.position, target.transform.position) / (gunproperty.bulletspeed);
    //    Vector3 coef = velocity * time;
    //    Vector3 newTarget = target.transform.position + coef;
    //    return newTarget;
    //}
    Vector2 PredictPosition(Rigidbody2D targetRigid)
    {
        Vector3 pos = targetRigid.position;
        Vector3 dir = targetRigid.velocity;

        float dist = (pos - transform.position).magnitude;

        return pos + (dist / gunproperty.bulletspeed) * dir;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("bullet") && gameObject.transform.CompareTag("coin"))
        {
          StopAllCoroutines();
            collision.attachedRigidbody.velocity = new Vector2(0, 0);
            collision.transform.position = transform.position;
            collision.transform.rotation = transform.rotation;
            
            Destroy(gameObject);
        }
        else if (collision.transform.CompareTag("coin"))
        {
            target = getcloseistenemy();
           // StartCoroutine(movetowards());
             float gunspeed = gunproperty.bulletspeed / gunproperty.bulletspeedrag;
            float totalspeed = gunspeed * (target.position - transform.position).magnitude;
            LeanTween.move(gameObject, PredictPosition(target.GetComponent<Rigidbody2D>()), totalspeed).setEaseLinear();
            
        }
        else if (collision.transform == target)
        {
            Debug.Log("hit");
        }
        else
        {
            Destroy(gameObject);
        }
    }
   // private IEnumerator movetowards()
   // {
   //    
   //     float time = 0f;
   //     while (target != null)
   //     {
   //         time += gunspeed * Time.deltaTime;
   //      transform.position =   Vector2.Lerp(transform.position, PredictPosition(target.GetComponent<Rigidbody2D>()), time / gunspeed);
   //         yield return new WaitForFixedUpdate();
   //     }
   //     
   //     
   // }
    private Vector3 dir()
    {
        Vector3 direction = Vector3.zero;
        switch (direct)
        {
            case Direction.up:
                direction = Vector3.up;
                break;
            case Direction.down:
                direction = Vector3.down;
                break;
            case Direction.right:
                direction = Vector3.right;
                break;
            case Direction.left:
                direction = Vector3.left;
                break;
            default:
                break;
        }
        return direction;
    }
}
