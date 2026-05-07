using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LLMModels : MonoBehaviour
{
    [Header("API Settings")]
    public string apiKey; 
    private string apiUrl = "htt://api.groq.com/openai/v1/chat/completions";

    // We use an Action callback to send the AI's final answer back to the BattleSystem
    public IEnumerator GetNahualDecision(int enemyHP, int playerHP, System.Action<string> callback)
    {
        // 1. The Prompt: We give the AI the current state of the battle
        string prompt = $"You are a Nahual monster in a turn-based RPG. Your HP is {enemyHP}. The player's HP is {playerHP}. You can choose one action: 'Attack' or 'Heal'. Respond with exactly one word: either Attack or Heal.";

        // 2. The JSON Payload
        string jsonData = $"{{\"model\": \"gpt-3.5-turbo\", \"messages\": [{{\"role\": \"user\", \"content\": \"{prompt}\"}}], \"temperature\": 0.7}}";

        // 3. The Web Request
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Required Headers for the API
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            UnityEngine.Debug.Log("Sending battle data to AI...");

            // 4. Send the request and pause the game until we get an answer
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                UnityEngine.Debug.LogError("AI Connection Failed: " + request.error);
                callback("Attack"); // Default to an attack if the Wi-Fi drops
            }
            else
            {
                // 5. Extract the AI's answer
                string rawResponse = request.downloadHandler.text;

                // We will add a JSON parser here later, but for now, let's just see if the raw response arrives!
                UnityEngine.Debug.Log("AI Responded: " + rawResponse);

                // Check if the AI's response contains the word "Heal"
                if (rawResponse.Contains("Heal"))
                {
                    callback("Heal");
                }
                else
                {
                    callback("Attack");
                }
            }
        }
    }
}
