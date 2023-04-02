using UnityEngine;
public class PlayerView : MonoBehaviour
{
    public void SetColor(Team team)
    {
        Color color = Color.white;
        switch (team)
        {
            case Team.Blue:
                color = Color.blue;
                break;
            case Team.Red:
                color = Color.red;
                break;
        }
        GetComponent<Renderer>().material.color = color;
    }
}
