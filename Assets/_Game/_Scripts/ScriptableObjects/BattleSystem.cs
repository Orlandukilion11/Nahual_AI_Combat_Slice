using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    [Header("Fighter Stats")]
    public int playerHP = 100;
    public int enemyHP = 100;

    [Header("UI References")]
    public UnityEngine.UI.Text dialogueText;
    public LLMModels llmBrain; // The link to Groq AI

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        dialogueText.text = "A wild Nahual appears!";
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose your action: Attack, Heal, or Flee.";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN) return;

        int damage = UnityEngine.Random.Range(15, 25);
        enemyHP -= damage;
        dialogueText.text = "You dealt " + damage + " damage!";

        if (enemyHP <= 0)
        {
            state = BattleState.WON;
            dialogueText.text = "You defeated the Nahual!";
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyAITurn());
        }
    }

    // 1. The Nahual's turn starts here
    IEnumerator EnemyAITurn()
    {
        dialogueText.text = "The Nahual is thinking...";

        // Send the HP data to the LLMModels script and wait for the AI's answer
        yield return StartCoroutine(llmBrain.GetNahualDecision(enemyHP, playerHP, ExecuteAIAction));
    }

    // 2. The AI's answer arrives here
    void ExecuteAIAction(string aiDecision)
    {
        if (aiDecision == "Heal")
        {
            int healAmount = 25;
            enemyHP += healAmount;
            dialogueText.text = "The AI chose to HEAL! Nahual recovered " + healAmount + " HP.";
        }
        else // It chose to Attack
        {
            int enemyDamage = UnityEngine.Random.Range(10, 20);
            playerHP -= enemyDamage;
            dialogueText.text = "The AI chose to ATTACK! You took " + enemyDamage + " damage.";
        }

        // Move to the final step
        StartCoroutine(FinishEnemyTurn());
    }

    // 3. We calculate if anyone died and pass the turn back to the player
    IEnumerator FinishEnemyTurn()
    {
        yield return new WaitForSeconds(3f);

        if (playerHP <= 0)
        {
            state = BattleState.LOST;
            dialogueText.text = "You were defeated...";
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
}
