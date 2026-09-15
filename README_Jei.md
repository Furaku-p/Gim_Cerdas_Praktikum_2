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

##  1. `CameraOrbit`
```
public class CameraOrbit : MonoBehaviour
{
    public Transform target;

    public float mouseSensitivity = 3f;
    public float clampAngle = 70f;

    private float rotX;
    private float rotY;
```
Definisi variabel yang digunakan untuk mengatur rotasi kamera:
- target: Referensi Transform objek yang akan diputar.
- mouseSensitivity: Menentukan sensitivitas pergerakan mouse terhadap rotasi kamera.
- clampAngle: Menentukan batas rotasi vertikal kamera.
- rotX dan rotY: Menyimpan sudut rotasi pada sumbu X dan Y.

```
private void Start()
{
    rotX = target.eulerAngles.x;
    rotY = target.eulerAngles.y;
}
```
Fungsi Start() dijalankan ketika permainan dimulai. Pada bagian ini, rotasi awal target diambil menggunakan target.eulerAngles, kemudian disimpan ke rotX dan rotY sehingga rotasi kamera dapat dimulai dari orientasi awal objek, bukan langsung dari sudut 0°.
```
private void LateUpdate()
{
    if (Input.GetMouseButton(1))
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
```

##  2. `NPCSensor`

##  3. `PlayerController`
