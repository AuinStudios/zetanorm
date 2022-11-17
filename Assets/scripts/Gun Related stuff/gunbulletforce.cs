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
        left
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
    // [SerializeField]	private Transform gundirection;
    // Start is called before the first frame update
    private void Start()
    {
        switch (typeofthing)
        {
            case Type.coin:
                rig.AddForce(dir() * gunproperty.coinforce, ForceMode2D.Impulse);
                StartCoroutine(deflectonearbyenemy());
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
    private IEnumerator deflectonearbyenemy()
    {
        foreach (Transform i in enemymanager.Instance.enemys)
        {
            if ((transform.position - i.position).magnitude < 13)
            {
                int t = 0;
                while (t < 100)
                {
                    var dir = transform.position - i.position;
                    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = (transform.position - i.position).magnitude > 5 ? Quaternion.AngleAxis(angle + 180 - (transform.position - i.position).magnitude, Vector3.forward) : Quaternion.AngleAxis(angle + 180, Vector3.forward);
                    t++;
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("bullet") && gameObject.transform.CompareTag("coin"))
        {
            collision.attachedRigidbody.velocity = new Vector2(0, 0);
            collision.transform.position = transform.position;
            collision.transform.rotation = transform.rotation;
            collision.attachedRigidbody.AddForce(transform.right * gunproperty.bulletspeed, ForceMode2D.Impulse);
            Destroy(gameObject);
        }
        else if (collision.transform.CompareTag("Untagged"))
        {
            Destroy(gameObject);
        }
    }
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
