using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("A wild Nahual attacked! Loading Battle Scene...");

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SceneManager.LoadScene("BattleScene");
        }
    }
}