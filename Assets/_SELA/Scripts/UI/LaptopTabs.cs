using UnityEngine;

// Tab di dalam laptop (CV, Lowongan, Portofolio, Interview, dll.).
// Hubungkan tiap tombol tab: OnClick -> LaptopTabs.Show(nomor halaman).
public class LaptopTabs : MonoBehaviour
{
    public GameObject[] pages;

    void OnEnable() { Show(0); }

    public void Show(int index)
    {
        for (int i = 0; i < pages.Length; i++) pages[i].SetActive(i == index);
    }
}
