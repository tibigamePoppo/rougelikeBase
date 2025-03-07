using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProjectileEffect : EffectEmitBase
{
    [SerializeField] GameObject _impactParticle;
    [SerializeField] GameObject _projectileParticle;
    [SerializeField] GameObject _muzzleParticle;
    [SerializeField] private float _hightRatio = 1;
    private GameObject projectile;
    private GameObject muzzle;
    private GameObject impact;
    public override void Emit(Vector3 vector3)
    {
        projectile = Instantiate(_projectileParticle, transform.position, Quaternion.identity, transform);
        muzzle = Instantiate(_muzzleParticle, transform.position, Quaternion.identity, transform);
        Destroy(muzzle, 1.5f);
        float hight = Vector3.Distance(transform.position, vector3);
        projectile.transform.DOJump(vector3, hight * _hightRatio / 3, 1, 0.2f).OnComplete(Impact);
    }

    private void Impact()
    {
        impact = Instantiate(_impactParticle, projectile.transform.position, Quaternion.identity, transform);
        Destroy(projectile);
        Destroy(impact,3.5f);
    }
}
