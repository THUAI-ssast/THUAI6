using UnityEngine;
public class PlayerView : MonoBehaviour
{
    public void SetColor(Team team)
    {
        Color color = Color.white;
        switch (team)
        {
            case Team.Blue:
                color = new Color(88 / 255.0f, 184 / 255.0f, 221 / 255.0f);
                break;
            case Team.Red:
                color = Color.red;
                break;
        }
        GetComponent<Renderer>().material.color = color;
    }
}
