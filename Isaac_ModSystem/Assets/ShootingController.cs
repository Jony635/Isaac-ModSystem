using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public void Shoot()
    {
        PlayerController.Instance.OnShootEvent();
    }
}
