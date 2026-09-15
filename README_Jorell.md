# Laporan Praktikum 2 - Game Cerdas

**Kelompok 14:** kata Jorell We <3 Unity

---

## Anggota Kelompok

| No | Nama | NRP |
|---|---|---|
| 1. | Jeihan Shawmy Prasetya | 5025241132 |
| 2. | Alfianz Risqia Ilahi Loven Kary | 50252411164 |
| 3. | Jorell Ramos Sinaga | 5025241202 |

---

##  1. `CameraTargetFollow`
Script ini berfungsi untuk mengontrol pergerakan kamera utama agar selalu mengikuti posisi objek player secara konsisten dengan memberikan *offset* ketinggian vertikal.

### <ins> Kode </ins>
```csharp
using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        transform.position =
            player.position +
            Vector3.up * 1.5f;
    }
}
```

---

### <ins> Penjelasan Kode </ins>
1. **Menggunakan `LateUpdate()`**: Dipanggil setiap frame setelah seluruh method `Update()` selesai dieksekusi. Hal ini menjamin posisi kamera diperbarui setelah posisi player dipastikan selesai berpindah, mencegah terjadinya efek *jittering* atau gerakan patah-patah pada kamera.
2. **Kalkulasi Posisi Kamera**:
   ```csharp
   transform.position = player.position + Vector3.up * 1.5f;
   ```
   Posisi kamera disesuaikan dengan posisi player ditambah offset `1.5` unit ke arah atas (`Vector3.up`).
