using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public enum Type { Melee, Range}; //근접무기와 원거리무기 구분
    public Type type;
    public int damage; //공격력
    public float rate; //공격속도
    public BoxCollider meleeArea; //근딜범위
    public TrailRenderer trailEffect; //근접공격 이펙트
    public Transform arrowPos; //화살나가는위치
    public GameObject arrow; //화살
    public PlayerST playerST;

  

    public void Use()//무기 사용
    {
        if (type == Type.Melee) //근접무기일때 실행
        {
            StopCoroutine("Swing");  //현재 공격중일시 멈춤
            StartCoroutine("Swing"); //공격실행
        }
        else if (type == Type.Range)
        {
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {

        yield return new WaitForSeconds(0.1f); // 대기
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);

    }

    IEnumerator Shot()
    {

        //화살 발사
        playerST.isSootReady = true;
        yield return new WaitForSeconds(0.2f); //애니메이션과 화살나가는속도와 맞추기위함
        GameObject intantArrow = Instantiate(arrow, arrowPos.position, arrowPos.rotation);
        Rigidbody arrowRigid = intantArrow.GetComponent<Rigidbody>();
        arrowRigid.velocity = arrowPos.forward * playerST.bowPower * 150;
        Destroy(intantArrow, 1f);

        playerST.anim.SetBool("doShot", false);
        
        playerST.bowPower = 0;

        yield return new WaitForSeconds(0.25f);

        playerST.isFireReady = true;

        yield return null;
    }
}
