using static Player;

public class Turn
{
    public AttackTarget attack { get; set; }

    public AttackType attack_type { get; set; }

    public AttackTarget defend1 { get; set; }

    public AttackTarget defend2 { get; set; }

    public bool ProcessedByEnemy { get; set; }
}
