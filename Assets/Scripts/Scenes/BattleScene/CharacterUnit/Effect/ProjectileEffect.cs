using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProjectileEffect : EffectEmitBase
{
    [SerializeField] GameObject _impactParticle;
    [SerializeField] GameObject _projectileParticle;
    [SerializeField] GameObject _muzzleParticle;
    private GameObject projectile;
    private GameObject muzzle;
    private GameObject impact;
    public override void Emit(Vector3 vector3)
    {
        projectile = Instantiate(_projectileParticle, transform.position, Quaternion.identity, transform);
        muzzle = Instantiate(_muzzleParticle, transform.position, Quaternion.identity, transform);
        Destroy(muzzle, 1.5f);
        projectile.transform.DOMove(vector3, 0.2f).OnComplete(Impact);
    }

    private void Impact()
    {
        impact = Instantiate(_impactParticle, projectile.transform.position, Quaternion.identity, transform);
        Destroy(projectile);
        Destroy(impact,3.5f);
    }
}
