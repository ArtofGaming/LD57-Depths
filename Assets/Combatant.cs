using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    int maxHp;
    int currentHp;
    int maxMp;
    int currentMp;
    BattleManager battleManager;
    int roll;

    List<Skill> skills = new List<Skill>();

    List<Skill> usableSkills = new List<Skill>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        battleManager = GetComponent<BattleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    List<Skill> FindUsableSkill()
    {
        foreach (Skill skill in skills)
        {
            if (skill.mpCost < currentMp) 
            {
                usableSkills.Add(skill);
            }
        }
        return usableSkills;
    }
    void DecideMove()
    {
        if (skills.Count != 0)
        {
            if (FindUsableSkill().Count > 0)
            {
                if (Random.Range(1,6) == 6)
                {
                    UseSkill();
                }
            }
        }
        if (Random.Range(1,6) == 1)
        {
            if (Random.Range(1,6) == 1)
            {
                Flee();
            }
            //else 
        }
        //else if (Rna)
    }
    void Attack()
    {

    }
    void UseSkill()
    {

    }
    void Flee()
    {

    }
    void Die()
    {

    }
}
