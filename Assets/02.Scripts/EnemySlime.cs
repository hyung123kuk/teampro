using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySlime : MonoBehaviour
{

    public int maxHealth; //�ִ� ü��
    public int curHealth; //���� ü��
    public Transform target; //��ǥ
    public BoxCollider meleeArea; //���� ���ݹ���
    public bool isChase; //�������� ����
    public bool isAttack; //���� ������
  
    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat; //�ǰݽ� �����ϰ�
    NavMeshAgent nav; //����
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim=GetComponent<Animator>();

        Invoke("ChaseStart", 0.5f);
    }

    void ChaseStart() //������
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        if(nav.enabled) //�������̸�
        nav.SetDestination(target.position); //Ÿ������
        nav.isStopped = !isChase;
        transform.LookAt(target); //�÷��̾� �ٶ󺸱�
    }

    void FreezeVelocity() //�̵�����
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void Targerting()//Ÿ����
    {
        float targetRadius = 1.5f; 
        float targetRange = 1.0f; 

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position,
            targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));  //����ĳ��Ʈ

        if(rayHits.Length>0 && !isAttack) //����ĳ��Ʈ�� �÷��̾ �����ٸ� && ���� �������� �ƴ϶��
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack() //������ �ϰ� �������ϰ� �ٽ� ������ ����
    {
        isChase = false;
        isAttack = true;
        
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.7f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.9f);
        meleeArea.enabled = false;
        
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    void FixedUpdate()
    {
        Targerting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)  //�ǰ�
    {
       if(other.tag == "Melee")
        {
            Weapons weapon = other.GetComponent<Weapons>();
            curHealth -= weapon.damage;
            
            StartCoroutine(OnDamage());
            
        }
       else if(other.tag=="Arrow")
        {
            Arrow arrow = other.GetComponent<Arrow>();
            curHealth -= arrow.damage;
            
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage() 
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth>0)
        {
            mat.color = Color.white;

        }
        else
        {
            mat.color = Color.black;
            isChase = false; //�׾����� ��������
            anim.SetTrigger("doDie");
            Destroy(gameObject, 0.5f);
        }
    }
}
