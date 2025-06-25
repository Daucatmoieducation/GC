using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    public Transform player;  // Nhân vật của bạn
    public GameObject borderObject;  // Đối tượng viền, không thay đổi kích thước
    public float maxSize = 5f;  // Kích thước tối đa của hình tròn
    public float minSize = 1f;  // Kích thước tối thiểu của hình tròn
    public float maxDistance = 10f;  // Khoảng cách tối đa để thay đổi kích thước
    public float maxScale = 100f;  // Giới hạn kích thước tối đa cho localScale (có thể điều chỉnh theo ý muốn)

    private SpriteRenderer circleRenderer;  // SpriteRenderer cho đối tượng Circle
    private SpriteRenderer borderRenderer;  // SpriteRenderer cho đối tượng Border

    void Start()
    {
        circleRenderer = GetComponent<SpriteRenderer>();  // SpriteRenderer cho đối tượng Circle
        borderRenderer = borderObject.GetComponent<SpriteRenderer>();  // SpriteRenderer cho đối tượng Border
    }

    void Update()
    {
        // Lấy vị trí chuột trên màn hình và chuyển đổi thành vị trí thế giới
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;  // Đảm bảo Z luôn bằng 0 nếu bạn làm việc trong 2D

        // Di chuyển hình tròn (đối tượng chính) đến vị trí chuột
        transform.position = mouseWorldPosition;

        // Tính khoảng cách giữa nhân vật và hình tròn
        float distance = Vector3.Distance(player.position, transform.position);

        // Đảm bảo rằng khoảng cách không bị âm hoặc quá lớn
        distance = Mathf.Min(distance, maxDistance);  // Giới hạn khoảng cách tối đa để tránh giá trị vô hạn

        // Tính toán kích thước của hình tròn (đối tượng Circle) tùy theo khoảng cách
        float size = Mathf.Lerp(minSize, maxSize, Mathf.InverseLerp(0, maxDistance, distance));

        // Kiểm tra xem giá trị size có hợp lệ không
        if (!float.IsFinite(size) || size <= 0)
        {
            // Nếu size không hợp lệ (Infinity hoặc NaN) hoặc nhỏ hơn hoặc bằng 0, gán lại giá trị hợp lệ
            size = minSize;
        }

        // Giới hạn kích thước để tránh vô hạn hoặc quá lớn
        size = Mathf.Clamp(size, minSize, maxSize);  // Đảm bảo size không quá lớn hoặc quá nhỏ

        // Giới hạn thêm giá trị kích thước lớn nhất cho localScale
        size = Mathf.Min(size, maxScale);  // Đảm bảo size không vượt quá maxScale

        // Kiểm tra lại giá trị size trước khi áp dụng vào localScale để tránh Infinity
        if (float.IsInfinity(size) || float.IsNaN(size))
        {
            size = minSize;  // Nếu size vẫn là Infinity hoặc NaN, gán lại giá trị minSize
        }

        // Áp dụng kích thước mới cho hình tròn
        transform.localScale = new Vector3(size, size, 1);

        // Giữ kích thước viền (Border) cố định
        float borderSize = borderRenderer.bounds.size.x;  // Kích thước viền cố định
        borderObject.transform.localScale = new Vector3(borderSize, borderSize, 1);
    }

}
