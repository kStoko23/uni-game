using System.Collections;
using UnityEngine;

public class Mole : MonoBehaviour {  
  [SerializeField] private Sprite mole;  
  [SerializeField] private Sprite bomb;    
  
  [SerializeField] private GameManager gameManager;
      
  private Vector2 startPosition = new Vector2(0f, -2.56f);
  private Vector2 endPosition = Vector2.zero;
  
  private float showDuration = 0.5f;
  private float duration = 1f;

  private SpriteRenderer spriteRenderer;  
  private BoxCollider2D boxCollider2D;
  private Vector2 boxOffset;
  private Vector2 boxSize;
  private Vector2 boxOffsetHidden;
  private Vector2 boxSizeHidden;

  private bool hittable = true;
  public enum MoleType { Standard, Bomb };
  private MoleType moleType;  
  private float bombRate = 0f;  
  private int moleIndex = 0;

  private IEnumerator ShowHide(Vector2 start, Vector2 end) {
    transform.localPosition = start;

    float elapsed = 0f;
    while (elapsed < showDuration) {
      transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
      boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
      boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
      
      elapsed += Time.deltaTime;
      yield return null;
    }

    transform.localPosition = end;
    boxCollider2D.offset = boxOffset;
    boxCollider2D.size = boxSize;

    yield return new WaitForSeconds(duration);

    elapsed = 0f;
    while (elapsed < showDuration) {
      transform.localPosition = Vector2.Lerp(end, start, elapsed / showDuration);
      boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
      boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
      
      elapsed += Time.deltaTime;
      yield return null;
    }
    
    transform.localPosition = start;
    boxCollider2D.offset = boxOffsetHidden;
    boxCollider2D.size = boxSizeHidden;
            
    if (hittable) {
      hittable = false;      
      gameManager.Missed(moleIndex, moleType != MoleType.Bomb);
    }
  }

  public void Hide() {   
    transform.localPosition = startPosition;
    boxCollider2D.offset = boxOffsetHidden;
    boxCollider2D.size = boxSizeHidden;
  }

  private IEnumerator QuickHide() {
    yield return new WaitForSeconds(0.25f);
    
    if (!hittable) {
      Hide();
    }
  }

  private void OnMouseDown() {
    if (hittable) {
      switch (moleType) {
        case MoleType.Standard:          
          gameManager.AddScore(moleIndex);            
          StopAllCoroutines();
          StartCoroutine(QuickHide());         
          hittable = false;
          break;        
        case MoleType.Bomb:         
          gameManager.GameOver(1);
          break;
        default:
          break;
      }
    }
  }

  private void CreateNext() {
    float random = Random.Range(0f, 1f);
    if (random < bombRate) {      
      moleType = MoleType.Bomb;     
      spriteRenderer.sprite = bomb;      
      spriteRenderer.sortingOrder = 0;
    } else {            
        moleType = MoleType.Standard;
        spriteRenderer.sprite = mole;             
    }    
    hittable = true;
  }

  private void SetLevel(int level) {    
    bombRate = Mathf.Min(level * 0.025f, 0.25f);   
           
    float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.01f, 1f);
    float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.01f, 2f);
    duration = Random.Range(durationMin, durationMax);
  }

  private void Awake() {   
    spriteRenderer = GetComponent<SpriteRenderer>();    
    boxCollider2D = GetComponent<BoxCollider2D>();
    
    boxOffset = boxCollider2D.offset;
    boxSize = boxCollider2D.size;
    boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
    boxSizeHidden = new Vector2(boxSize.x, 0f);
  }

  public void Activate(int level) {
    SetLevel(level);
    CreateNext();
    StartCoroutine(ShowHide(startPosition, endPosition));
  }
      
  public void SetIndex(int index) {
    moleIndex = index;
  }
      
  public void StopGame() {
    hittable = false;
    StopAllCoroutines();
  }

  public void ResumeGame()
    {
    hittable = true;
    StartCoroutine(ShowHide(startPosition, endPosition));
  }
}
