using UnityEngine;

public class BulletDisplay : MonoBehaviour
{
    public SpriteRenderer[] bulletIcons;       // Mỗi viên đạn là một SpriteRenderer
    public Sprite fullBulletSprite;            // Asset 1 - viên đạn còn
    public Sprite emptyBulletSprite;           // Asset 2 - viên đạn trống

    private int currentBulletCount;

    public void SetBulletCount(int count)
    {

        currentBulletCount = count;
        UpdateBulletSprites();
    }

    public void Reload(int max)
    {
        currentBulletCount = max;
        UpdateBulletSprites();
    }

    private void UpdateBulletSprites()
    {

        for (int i = 0; i < bulletIcons.Length; i++)
        {
            if (bulletIcons[i] == null)
            {
                continue;
            }

            if (i < currentBulletCount)
            {
                bulletIcons[i].sprite = fullBulletSprite;
            }
            else
            {
                bulletIcons[i].sprite = emptyBulletSprite;
            }
        }
    }

}
