using UnityEngine;

public class UISkillManager : MonoBehaviour
{
    public static UISkillManager instance = null;

    public GameObject skillRoot;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    public GameObject GetSkill()
    {
        return skillRoot;
    }
}