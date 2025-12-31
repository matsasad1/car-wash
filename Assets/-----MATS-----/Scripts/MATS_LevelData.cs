using System.Collections;
using UnityEngine;


//[CreateAssetMenu(menuName = "CarWash/Level")]
public class MATS_LevelData : MonoBehaviour
{

    public int levelNumber;
    public MATS_LevelTask[] tasks;



    [Header("Movement")]
    [SerializeField] private float minY = 1.64f;
    [SerializeField] private float maxY = 1.65f;
    [SerializeField] private float moveSpeed = 1.5f;

    public IEnumerator MoveAvatar(Transform obj)
    {


        MATS_Debug.Log("Moving the avatar   xxxxx");
        Transform t = obj.transform;
        float midY = (minY + maxY) / 2f;
        float amplitude = (maxY - minY) / 2f;
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime * moveSpeed;
            float newY = midY + Mathf.Sin(time) * amplitude;
            Vector3 pos = t.position;
            pos.y = newY;
            t.position = pos;

            yield return null;
        }
    }
}