using UnityEngine;

public class Missile : Projectile
{
    //��� ��������������� ������ � ������ ���������� � ������� ���� �����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IEnemy enemy = collision.transform.GetComponent<IEnemy>();
        enemy.GetDamage((damage * DamageMultiplier) * DamageModificator);
        Destroy(this.gameObject);
    }

    //�������� �����
    private void Update()
    {
        transform.Translate(0, speed * SpeedModificator * Time.deltaTime, 0);
    }
}
