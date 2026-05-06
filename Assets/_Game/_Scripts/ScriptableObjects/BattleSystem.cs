using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for UI manipulation

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    [Header("Fighter Stats")]
    public int playerHP = 100;
    public int enemyHP = 100;

    [Header("UI References")]
    public Text dialogueText; // We will link this in the editor

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

        // Wait 2 seconds so the player can read
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose your action: Attack, Heal, or Flee.";
    }

    // Hook this to a UI Button
    public void OnAttackButton()
    {
        // Prevent clicking if it's not the player's turn
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
            // Trigger the AI's turn
            StartCoroutine(EnemyAITurn());
        }
    }

    IEnumerator EnemyAITurn()
    {
        dialogueText.text = "The Nahual is thinking...";

        // Tomorrow, we replace this 2-second wait with a real API call to Claude/OpenAI
        yield return new WaitForSeconds(2f);

        int enemyDamage = UnityEngine.Random.Range(10, 20);
        playerHP -= enemyDamage;
        dialogueText.text = "The Nahual attacks for " + enemyDamage + " damage!";

        yield return new WaitForSeconds(2f);

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
