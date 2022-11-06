using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Start()
    {
      float x = Random.Range(-6, 6);
      float z = Random.Range(-6, 6);
      transform.position = new Vector3(x, 3, z);
    }
}
