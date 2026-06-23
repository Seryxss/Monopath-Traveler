[System.Serializable]
public class ActionIntent
{
    public ScriptableSkill ChosenSkill;
    public CharacterBase Target;
    public int BoostAmount;

    // Fungsi untuk mereset rencana ke default (misal: Attack biasa, 0 Boost)
    public void ResetIntent()
    {
        ChosenSkill = null;
        Target = null;
        BoostAmount = 0;
    }
}