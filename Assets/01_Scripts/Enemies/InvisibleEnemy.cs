
public class InvisibleEnemy : Enemy
{
    protected bool _revealed = false;
    
    public override void MakeDamage(float damage)
    {
        base.MakeDamage(damage);
        
        EnemyInvisibleConfig invisibleConfig = config as EnemyInvisibleConfig;
            
        if (!invisibleConfig) return;
        _revealed = true;
        
        if (!animator) return;
        animator.SetLibrary(invisibleConfig.revealedSpriteLibraryAsset);
    }
    
    public override bool IsTargetable()
    {
        return _revealed && base.IsTargetable();
    }
}
