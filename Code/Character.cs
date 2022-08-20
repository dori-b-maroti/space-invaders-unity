/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using UnityEngine;

public class Character : Actor
{
    public Projectile projectile;
    public float shootCooldown;

    protected float lastShootTime;

    public virtual void Shoot()
    {
        Instantiate(projectile, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
    }

    public virtual void Die() { }
}