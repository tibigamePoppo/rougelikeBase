using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffect : EffectEmitBase
{
    [SerializeField] GameObject _prefab;
    private const int SPEED = 3000;
    public override void Emit()
    {
        GameObject projectile = Instantiate(_prefab, transform.position, Quaternion.identity, transform);
        projectile.transform.LookAt(transform.position + transform.forward); //Sets the projectiles rotation to look at the point clicked
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * SPEED);
    }
}
