using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdate : MonoBehaviour {

	public Text text;
	public PlayerController player;
	public int score;
	public int edge;

	private void Start() {
		text = GetComponent<Text>();
		score = player.score;
		edge = player.edge + 3;
		StringBuilder sb = new StringBuilder(20);
		sb.Append("Score: ");
		sb.Append(score);
		sb.AppendLine();
		sb.Append("Edge: ");
		sb.Append(edge);
		text.text = sb.ToString();
	}

	private void Update() {
		if (player && (score != player.score || edge != player.edge)) {
			score = player.score;
			edge = player.edge + 3;
			StringBuilder sb = new StringBuilder(20);
			sb.Append("Score: ");
			sb.Append(score);
			sb.AppendLine();
			sb.Append("Edge: ");
			sb.Append(edge);
			text.text = sb.ToString();
		}
	}
}