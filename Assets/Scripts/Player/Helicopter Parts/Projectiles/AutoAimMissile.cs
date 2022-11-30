using UnityEngine;

public class AutoAimMissile : Projectile
{
     internal GameObject _target; //���� ���� ������

    private void Update()
    {
        //��������� ����� ���� ���� ���, ���� ����, �� ��������� � ����
        if(_target == null)
        {
            transform.Translate(0, speed * SpeedModificator * Time.deltaTime, 0);
        }
        else
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _target.transform.position, speed * SpeedModificator * Time.deltaTime);
            transform.up = _target.transform.position - transform.position;
        }
    }
    //��� ��������������� ������ � ������ ���������� � ������� ���� �����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IHealth collisionHealth = collision.transform.GetComponent<IHealth>();
        collisionHealth.GetDamage((damage * DamageMultiplier) * DamageModificator);
        Destroy(this.gameObject);
    }

    //����� ���������� ���������� ���������� � ���������� ���� ��� ������
    public void SetAimTarget(GameObject target)
    {
        _target = target;
    }
}
