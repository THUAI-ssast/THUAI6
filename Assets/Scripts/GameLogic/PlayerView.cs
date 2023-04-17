using UnityEngine;
public class PlayerView : MonoBehaviour
{
    private LineRenderer _bulletLineRenderer;
    private Rigidbody2D _rb2d;

    void Awake()
    {
        _bulletLineRenderer = GetComponent<LineRenderer>();
        _rb2d = GetComponent<Rigidbody2D>();
    }

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
        _bulletLineRenderer.startColor = color;
        _bulletLineRenderer.endColor = color;
    }

    public void ShootBullet(Vector2 target)
    {
        _bulletLineRenderer.SetPosition(0, _rb2d.position);
        _bulletLineRenderer.SetPosition(1, target);
        _bulletLineRenderer.enabled = true;
        DelayedFunctionCaller.CallAfter(0.1f, () => _bulletLineRenderer.enabled = false);
    }
}
