using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Projectile laserPrefab;
    private Projectile laser;

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            MoveLeft();
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            MoveRight();
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))) {
            Shoot();
        }
    }
    public void MoveLeft()
    {
        Vector3 position = transform.position;
        position.x -= speed * Time.deltaTime;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);
        transform.position = position;
    }
    public void MoveRight()
    {
        Vector3 position = transform.position;
        position.x += speed * Time.deltaTime;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);
        transform.position = position;
    }
    public void Shoot()
    {
        if (laser == null)
        {
            laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invader")) {
            GameManager.Instance.OnPlayerKilled(this);
        }
    }

}
