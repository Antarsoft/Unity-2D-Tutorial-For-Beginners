using UnityEngine;

public class ShootingItem : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.Translate(transform.right * transform.localScale.x * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            return;

        //Trigger the custom action on the other object IF IT EXISTS
        if (collision.GetComponent<ShootingAction>())
            collision.GetComponent<ShootingAction>().Action();
        //Destroy
        Destroy(gameObject);

    }
}
