using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LLMModels : MonoBehaviour
{
    [Header("API Settings")]
    public string apiKey; 
    private string apiUrl = "https://api.groq.com/openai/v1/chat/completions";

    // We use an Action callback to send the AI's final answer back to the BattleSystem
    public IEnumerator GetNahualDecision(int enemyHP, int playerHP, System.Action<string> callback)
    {
        // 1. The Prompt: We give the AI the current state of the battle
        string hpStatus = (enemyHP < 35) ? "CRITICALLY LOW" : "HEALTHY";
        string prompt = "You are a wild beast in an RPG. Your HP is " + hpStatus + ". Choose ONE action: Claw, Bite, Tackle, SpiritStrike, Heal, Flee. RULE: If your HP is CRITICALLY LOW, you MUST answer with Heal or Flee. If HEALTHY, choose an attack. Respond with EXACTLY ONE WORD.";
        // 2. The JSON Payload
        string jsonData = "{\"model\": \"llama-3.1-8b-instant\", \"messages\": [{\"role\": \"user\", \"content\": \"" + prompt + "\"}], \"temperature\": 0.0}";

        // 3. The Web Request
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Required Headers for the API
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            UnityEngine.Debug.Log("Sending JSON: " + jsonData); // Let's see exactly what we are sending

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                UnityEngine.Debug.LogError("AI Connection Failed: " + request.error);
                // THIS IS THE SMOKING GUN: Read Groq's rejection letter
                UnityEngine.Debug.LogError("Groq's Exact Error: " + request.downloadHandler.text);

                callback("Claw");
            }
            else
            {
                // 1. Read the giant JSON from Groq and force it to lowercase so we don't miss "claw" vs "Claw"
                string rawResponse = request.downloadHandler.text.ToLower();
                UnityEngine.Debug.Log("Raw AI JSON: " + rawResponse);

                string chosenMove = "Claw"; // Our safe default

                // 2. Pluck the exact decision out of the JSON
                if (rawResponse.Contains("flee")) chosenMove = "Flee";
                else if (rawResponse.Contains("heal")) chosenMove = "Heal";
                else if (rawResponse.Contains("claw")) chosenMove = "Claw";
                else if (rawResponse.Contains("bite")) chosenMove = "Bite";
                else if (rawResponse.Contains("tackle")) chosenMove = "Tackle";
                else if (rawResponse.Contains("spirit")) chosenMove = "SpiritStrike";

                // 3. Print exactly what we are handing to the BattleSystem so you can see it!
                UnityEngine.Debug.Log("TRANSLATED MOVE: " + chosenMove);

                callback(chosenMove);
            }
        }
        
    }
}
