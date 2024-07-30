using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)));
    }
}
