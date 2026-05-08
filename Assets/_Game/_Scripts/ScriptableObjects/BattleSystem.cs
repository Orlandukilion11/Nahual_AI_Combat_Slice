using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    [Header("Fighter Stats")]
    public int playerHP = 100;
    public int enemyHP = 30;

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

        int damage = UnityEngine.Random.Range(5, 10);
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
        // This cleans up the AI's response just in case it accidentally adds a period or space
        aiDecision = aiDecision.Trim().Trim('.', '"');

        if (aiDecision == "Heal")
        {
            int healAmount = 25;
            enemyHP += healAmount;
            dialogueText.text = "The Nahual chose to HEAL! It recovered " + healAmount + " HP.";
        }
        else if (aiDecision == "Flee")
        {
            dialogueText.text = "The Nahual respects your strength and FLED the battle!";
            state = BattleState.WON; // Fleeing ends the battle
        }
        else if (aiDecision == "Claw" || aiDecision == "Bite" || aiDecision == "Tackle" || aiDecision == "SpiritStrike")
        {
            // For now, all 4 moves do random damage, but you can customize this later!
            int enemyDamage = UnityEngine.Random.Range(10, 20);

            // Let's give SpiritStrike a little extra punch
            if (aiDecision == "SpiritStrike") enemyDamage += 5;

            playerHP -= enemyDamage;
            dialogueText.text = $"The Nahual used {aiDecision.ToUpper()}! You took {enemyDamage} damage.";
        }
        else
        {
            // A fallback just in case the AI says something confusing
            int enemyDamage = 10;
            playerHP -= enemyDamage;
            dialogueText.text = "The Nahual thrashes wildly for " + enemyDamage + " damage!";
        }

        // We pass the decision to the final step so it knows if the battle ended
        StartCoroutine(FinishEnemyTurn(aiDecision));
    }

    // 3. We calculate if anyone died (or fled) and pass the turn back
    IEnumerator FinishEnemyTurn(string lastEnemyAction)
    {
        yield return new WaitForSeconds(3f);

        if (lastEnemyAction == "Flee")
        {
            // If it fled, we send the player back to the Overworld!
            UnityEngine.SceneManagement.SceneManager.LoadScene("OverworldScene");
            yield break; // This stops the Coroutine instantly
        }

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