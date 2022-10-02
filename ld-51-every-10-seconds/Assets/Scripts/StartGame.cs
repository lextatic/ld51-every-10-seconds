using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
	public static string Nickname = "Guest";

	public TMP_InputField InputField;

	public void StartGameNow()
	{
		Nickname = InputField.text;
		SceneManager.LoadScene("MainScene");
	}
}
