using UnityEngine;

public class ArcherController : MonoBehaviour
{
    [Header("Impostazioni Arciere")]
    public float tempoTraIColpi = 1.0f; // 1 secondo tra una freccia e l'altra
    public float velocitaFreccia = 5.0f;

    [Header("Riferimenti")]
    public Transform puntoDiLancio; // punto di spawn per la freccia

    [Header("Impostazioni Radar")]
    public float raggioAzione = 5f;
    public LayerMask layerNemici;

    private Transform bersaglioAttuale;

    private Animator animator;

    private float prossimoColpoDisponibile = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2? enemyPosition = CheckEnemy();

        

        if (enemyPosition.HasValue) 
        {   

            Vector3 scaleArcher = transform.localScale;

            if (enemyPosition.Value.x < transform.position.x) {
                scaleArcher.x = -Mathf.Abs(scaleArcher.x);
            } else {
                scaleArcher.x = Mathf.Abs(scaleArcher.x);
            }   

            transform.localScale = scaleArcher;

            if (Time.time >= prossimoColpoDisponibile)
            {
                animator.SetTrigger("shoot");

                prossimoColpoDisponibile = Time.time + tempoTraIColpi;
            } 
        } else 
        {
            animator.ResetTrigger("shoot");
        }
    }

    public void ScoccaFrecciaReale() 
    {
        Vector2? enemyPosition = CheckEnemy();

        if (enemyPosition.HasValue) 
        {
            ArrowManager.Instance.SparaFreccia(puntoDiLancio.position, enemyPosition.Value, velocitaFreccia);
        }
    }

    private Vector2? CheckEnemy() 
    {
        if(bersaglioAttuale != null) {
            
            float distanza = Vector2.Distance(transform.position, bersaglioAttuale.position);

            if (distanza <= raggioAzione) {
                return bersaglioAttuale.position;
            } else {
                bersaglioAttuale = null;
            }
        }

        // se siamo arrivati qui vuol dire che non c'è un bersaglio e che se ne deve trovare un altro
        Collider2D[] nemiciNelRaggio = Physics2D.OverlapCircleAll(transform.position, raggioAzione, layerNemici);

        if(nemiciNelRaggio.Length > 0) {
            bersaglioAttuale = nemiciNelRaggio[0].transform;
            return bersaglioAttuale.position;
        }

        return null;
    }

    // Disegna area arcere
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raggioAzione);
    }
}