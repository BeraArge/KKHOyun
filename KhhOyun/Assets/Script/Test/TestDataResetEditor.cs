#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class TestDataResetEditor
{
    [MenuItem("Tools/Test Verilerini Sýfýrla")]
    public static void ResetAllData()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "Test Verilerini Sýfýrla",
            "Giriþ oturumu, kullanýcý bilgileri, Beni Hatýrla ve aþama ilerlemesi silinecek.",
            "Evet, Sýfýrla",
            "Vazgeç"
        );

        if (!confirmed)
            return;

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("[TestDataResetEditor] Tüm PlayerPrefs verileri silindi.");

        EditorUtility.DisplayDialog(
            "Tamamlandý",
            "Yerel veriler silindi. Play modunu yeniden baþlat.",
            "Tamam"
        );
    }
}
#endif