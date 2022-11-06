using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Start()
    {
      float x = Random.Range(-5, 5);
      float z = Random.Range(-5, 5);
      transform.position = new Vector3(x, 3, z);
    }
}
