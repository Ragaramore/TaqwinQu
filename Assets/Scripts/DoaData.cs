using UnityEngine;

[CreateAssetMenu(fileName = "DataDoaBaru", menuName = "Taqwinqu/Buat Data Doa")]
public class DoaData : ScriptableObject
{
    public string namaDoa;
    [TextArea(3, 5)] public string teksLatin;
    [TextArea(3, 5)] public string teksArti;
    public Sprite gambarArab;
}
