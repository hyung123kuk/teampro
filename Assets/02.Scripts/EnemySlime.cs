using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySlime : MonoBehaviour
{

    public int maxHealth; //최대 체력
    public int curHealth; //현재 체력
    public Transform target; //목표
    public BoxCollider meleeArea; //몬스터 공격범위
    public bool isChase; //추적중인 상태
    public bool isAttack; //현재 공격중
  
    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat; //피격시 색깔변하게
    NavMeshAgent nav; //추적
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

    void ChaseStart() //추적중
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        if(nav.enabled) //추적중이면
        nav.SetDestination(target.position); //타겟추적
        nav.isStopped = !isChase;
        transform.LookAt(target); //플레이어 바라보기
    }

    void FreezeVelocity() //이동보정
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void Targerting()//타겟팅
    {
        float targetRadius = 1.5f; 
        float targetRange = 1.0f; 

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position,
            targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));  //레이캐스트

        if(rayHits.Length>0 && !isAttack) //레이캐스트에 플레이어가 잡혔다면 && 현재 공격중이 아니라면
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack() //정지를 하고 공격을하고 다시 추적을 개시
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

    void OnTriggerEnter(Collider other)  //피격
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
            isChase = false; //죽었으니 추적중지
            anim.SetTrigger("doDie");
            Destroy(gameObject, 0.5f);
        }
    }
}
