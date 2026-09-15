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

    rotY += mouseX * mouseSensitivity;
    rotX -= mouseY * mouseSensitivity;

    rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

    target.rotation = Quaternion.Euler(rotX, rotY, 0f);
    }
}
```
LateUpdate() dijalankan setiap frame dan digunakan untuk memperbarui rotasi kamera. Input.GetMouseButton(1) memeriksa apakah tombol mouse kanan sedang ditekan. Jika iya, pergerakan mouse dibaca dengan Mouse X untuk pergerakan horizontal dan Mouse Y untuk pergerakan vertikal. rotY bertambah atau berkurang berdasarkan gerakan mouse horizontal, sehingga kamera dapat berputar ke kiri dan kanan, sedangkan rotX berubah berdasarkan gerakan mouse vertikal, sehingga kamera dapat melihat ke atas dan bawah. Mathf.Clamp() digunakan untuk membatasi nilai rotasi rotX agar berada dalam rentang tertentu (mencegah kamera berputar terlalu jauh ke atas atau ke bawah). Quaternion.Euler() mengubah sudut rotasi Euler pada sumbu X, Y, dan Z menjadi Quaternion yang digunakan Unity untuk merepresentasikan rotasi objek sehingga orientasi target diperbarui sesuai input mouse.

##  2. `NPCSensor`
```
public class NPCSensor : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Vision Settings")]
    [SerializeField] private float viewRadius = 12f;

    [Header("Hearing Settings")]
    [SerializeField] private float hearingRadius = 5f;

    public bool CanHearPlayer { get; private set; }

    [Range(0f, 360f)]
    [SerializeField] private float viewAngle = 90f;

    [SerializeField] private LayerMask obstacleMask;

    [Header("Eye Settings")]
    [SerializeField] private float eyeHeight = 1.2f;

    public bool CanSeePlayer { get; private set; }

    public Transform Player => player;
```
Definisi variabel untuk mengatur sensor NPC:
- player: Referensi objek pemain yang akan dideteksi.
- viewRadius: Jarak maksimum penglihatan NPC, yaitu 12 unit.
- hearingRadius: Jarak maksimum pendengaran, yaitu 5 unit.
- viewAngle: Sudut field of view sebesar 90°.
- obstacleMask: Menentukan layer objek yang dianggap sebagai penghalang penglihatan.
- eyeHeight: Ketinggian posisi mata NPC untuk melakukan pengecekan garis pandang.
- CanSeePlayer dan CanHearPlayer: Menyimpan status apakah NPC dapat melihat atau mendengar pemain.

```
private void Update()
{
    DetectPlayer();
    DetectSound();
}
```
DetectPlayer() untuk memeriksa penglihatan dan DetectSound() untuk memeriksa pendengaran. Fungsi Update() ini dipanggil setiap frame selama permainan berjalan.

```
private void DetectPlayer()
{
    CanSeePlayer = false;

    if (player == null)
        return;
```
Setiap kali fungsi dijalankan, CanSeePlayer diatur menjadi false terlebih dahulu. Jika referensi pemain tidak tersedia, fungsi langsung dihentikan untuk mencegah error.

```
Vector3 directionToPlayer =
    player.position - transform.position;

float distanceToPlayer =
    directionToPlayer.magnitude;

if (distanceToPlayer > viewRadius)
    return;
```
Menghitung jarak antara NPC dan pemain menggunakan selisih posisi kedua objek. magnitude menghasilkan panjang vector, yang digunakan sebagai jarak antara NPC dan pemain. Jika jarak melebihi viewRadius, pemain dianggap berada di luar jangkauan penglihatan sehingga proses deteksi dihentikan.

```
Vector3 normalizedDirection =
    directionToPlayer.normalized;

float angleToPlayer =
    Vector3.Angle(
        transform.forward,
        normalizedDirection
    );

if (angleToPlayer > viewAngle / 2f)
    return;
```
Memeriksa apakah pemain berada dalam sudut pandang NPC. transform.forward menunjukkan arah depan NPC, sedangkan normalizedDirection menunjukkan arah dari NPC menuju pemain. Vector3.Angle() menghitung sudut antara kedua arah tersebut. Karena viewAngle adalah 90°, sudut maksimum dari arah tengah ke masing-masing sisi adalah 45°. Jika sudut pemain melebihi 45°, pemain berada di luar field of view dan tidak terdeteksi.

```
Vector3 eyePosition =
    transform.position +
    Vector3.up * eyeHeight;

Vector3 targetPosition =
    player.position +
    Vector3.up * 0.5f;

Vector3 rayDirection =
    targetPosition - eyePosition;

float rayDistance =
    rayDirection.magnitude;
```
Menentukan posisi mata NPC dan posisi target pada pemain. 
- eyePosition: Posisi NPC ditambah ketinggian mata 1.2 unit.
- targetPosition: Posisi pemain ditambah 0.5 unit, sehingga pengecekan diarahkan ke bagian tubuh pemain, bukan hanya ke titik kaki.
rayDirection menunjukkan arah dari mata NPC menuju pemain, sedangkan rayDistance menyimpan jarak antara kedua titik tersebut.

##  3. `PlayerController`
```

```
