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

---

##  2. `CameraTargetFollow`
Script ini berfungsi untuk mengontrol pergerakan kamera utama agar selalu mengikuti posisi objek player secara konsisten dengan memberikan *offset* ketinggian vertikal.

#### Parameter & Variabel
| Nama Variabel | Tipe Data | Akses | Deskripsi |
|---|---|---|---|
| `player` | `Transform` | `public` | Referensi komponen `Transform` milik objek Player yang akan diikuti |

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

### <ins> Penjelasan Kode </ins>
1. **Menggunakan `LateUpdate()`**: Dipanggil setiap frame setelah seluruh method `Update()` selesai dieksekusi. Hal ini menjamin posisi kamera diperbarui setelah posisi player dipastikan selesai berpindah, mencegah terjadinya efek *jittering* atau gerakan patah-patah pada kamera.
2. **Kalkulasi Posisi Kamera**:
   ```csharp
   transform.position = player.position + Vector3.up * 1.5f;
   ```
   Posisi kamera disesuaikan dengan posisi player ditambah offset `1.5` unit ke arah atas (`Vector3.up`).

---

##  3. `NPCSensor`
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
- rayDirection menunjukkan arah dari mata NPC menuju pemain, sedangkan rayDistance menyimpan jarak antara kedua titik tersebut.
  
```
if (Physics.Raycast(
    eyePosition,
    rayDirection.normalized,
    rayDistance,
    obstacleMask))
{
    return;
}

CanSeePlayer = true;
```
Physics.Raycast() menembakkan sinar dari posisi mata NPC menuju pemain untuk memeriksa apakah terdapat objek penghalang pada layer yang ditentukan oleh obstacleMask. Jika raycast mengenai penghalang, fungsi berhenti dan pemain tidak terdeteksi. Jika tidak ada penghalang, CanSeePlayer diatur menjadi true.

```
private void DetectSound()
{
    CanHearPlayer = false;

    if (player == null)
        return;

    PlayerController playerController =
        player.GetComponent<PlayerController>();

    if (playerController == null)
        return;

    if (playerController.IsCrouching)
        return;
```
Memeriksa apakah NPC dapat mendengar pemain. Status pendengaran diatur false terlebih dahulu, kemudian script mengambil komponen PlayerController dari objek pemain. Jika pemain tidak memiliki komponen tersebut, fungsi dihentikan. Jika pemain sedang crouch, NPC tidak mendeteksi suara pemain.

```
float distanceToPlayer =
    Vector3.Distance(
        transform.position,
        player.position
    );

if (distanceToPlayer <= hearingRadius)
{
    CanHearPlayer = true;
}
```
Jarak NPC dan pemain dihitung menggunakan Vector3.Distance(). Jika jaraknya kurang dari atau sama dengan hearingRadius dan pemain tidak crouch, CanHearPlayer diatur menjadi true.

```
private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(
        transform.position,
        viewRadius
    );

    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(
        transform.position,
        hearingRadius
    );

    if (player != null && CanSeePlayer)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            transform.position +
            Vector3.up * eyeHeight,
            player.position +
            Vector3.up * 0.5f
        );
    }
}
```
Menampilkan visualisasi sensor NPC pada Scene View Unity ketika GameObject dipilih.
- Lingkaran kuning menunjukkan radius penglihatan.
- Lingkaran cyan menunjukkan radius pendengaran.
- Terdapat juga garis batas field of view yang digambar menggunakan DirectionFromAngle() dan Gizmos.DrawLine(). Garis tersebut membantu menunjukkan arah batas kiri dan kanan penglihatan NPC.
- Jika pemain sedang terlihat, garis merah digambar dari posisi mata NPC menuju pemain.

```
private Vector3 DirectionFromAngle(float angle)
{
    float finalAngle =
        transform.eulerAngles.y + angle;

    return new Vector3(
        Mathf.Sin(finalAngle * Mathf.Deg2Rad),
        0f,
        Mathf.Cos(finalAngle * Mathf.Deg2Rad)
    );
}
```
Mengubah nilai sudut menjadi vector arah 3D yang digunakan untuk menggambar batas field of view. transform.eulerAngles.y digunakan sebagai arah rotasi horizontal NPC, kemudian ditambahkan dengan sudut yang diberikan. Mathf.Sin() dan Mathf.Cos() digunakan untuk menghitung komponen X dan Z dari vector arah. Mathf.Deg2Rad mengubah satuan derajat menjadi radian karena fungsi trigonometri Unity menggunakan radian.

---

## 4. `NPCBrain`
Script ini merupakan otak pengendali utama kecerdasan buatan (AI) NPC Guard. Script ini menangani sensor, pengambilan keputusan, serta eksekusi aksi pergerakan NPC.

#### Parameter & Variabel
| Nama Variabel | Tipe Data | Akses | Deskripsi |
|---|---|---|---|
| `sensor` | `NPCSensor` | `private` | Referensi komponen sensor penglihatan dan pendengaran NPC |
| `agent` | `NavMeshAgent` | `private` | Referensi komponen NavMeshAgent untuk navigasi patfinding |
| `patrolPoints` | `Transform[]` | `private` | Array titik-titik lokasi *waypoint* patroli |
| `waypointTolerance` | `float` | `private` | Jarak toleransi pencapaian titik *waypoint* (`0.7f`) |
| `patrolSpeed` | `float` | `private` | Kecepatan gerak NPC saat kondisi patroli (`2f`) |
| `chaseSpeed` | `float` | `private` | Kecepatan gerak NPC saat mengejar player (`4f`) |
| `searchDuration` | `float` | `private` | Durasi waktu pencarian saat kondisi *Search* (`4f`) |
| `searchTolerance` | `float` | `private` | Toleransi jarak pencapaian titik pencarian terakhir (`0.8f`) |
| `waitTimeAtWaypoint` | `float` | `private` | **(Challenge 1)** Waktu jeda tunggu di tiap titik *waypoint* (`2f`) |
| `searchRotationSpeed` | `float` | `private` | **(Challenge 2)** Kecepatan rotasi geleng kepala saat *Search* (`90f`) |
| `alertExclamation` | `GameObject` | `private` | **(Challenge 3)** Objek UI tanda seru (`!`) saat mengejar (*Chase*) |
| `alertQuestion` | `GameObject` | `private` | **(Challenge 3)** Objek UI tanda tanya (`?`) saat mencari (*Search*) |
| `currentState` | `NPCState` | `private` | Status FSM NPC saat ini (`Patrol`, `Chase`, `Search`) |

### <ins> 4.1 Kode Lengkap </ins>

<details>
<summary><b>Klik untuk melihat kode lengkap NPCBrain.cs</b></summary>

```csharp
using UnityEngine;
using UnityEngine.AI;

public class NPCBrain : MonoBehaviour
{
    public enum NPCState
    {
        Patrol,
        Chase,
        Search
    }

    [Header("References")]
    [SerializeField] private NPCSensor sensor;
    [SerializeField] private NavMeshAgent agent;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waypointTolerance = 0.7f;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Search Settings")]
    [SerializeField] private float searchDuration = 4f;
    [SerializeField] private float searchTolerance = 0.8f;

    [Header("Challenge1")]
    [SerializeField] private float waitTimeAtWaypoint = 2f;
    private float waitTimer;
    private bool isWaiting;

    [Header("Search Rotation")]
    [SerializeField] private float searchRotationSpeed = 90f;
    private float lookRight = true;
    private float lookTimer = 0f;

    [Header("Challenge 3")]
    [SerializeField] private GameObject alertExclamation;
    [SerializeField] private GameObject alertQuestion;

    [Header("Debug")]
    [SerializeField] private NPCState currentState;
    private NPCState previousState;
    private int patrolIndex = 0;

    private Vector3 lastKnownPosition;
    private bool hasLastKnownPosition;
    private float searchTimer;

    private void Start()
    {
        currentState = NPCState.Patrol;
        previousState = currentState;
        GoToCurrentPatrolPoint();
    }

    private void Update()
    {
        UpdateMemory();
        MakeDecision();
        ExecuteCurrentState();
        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        if (alertExclamation != null)
            alertExclamation.SetActive(currentState == NPCState.Chase);

        if (alertQuestion != null)
            alertQuestion.SetActive(currentState == NPCState.Search);
    }

    private void UpdateMemory()
    {
        if (sensor.CanSeePlayer || sensor.CanHearPlayer)
        {
            lastKnownPosition = sensor.Player.position;
            hasLastKnownPosition = true;
        }
    }

    private void MakeDecision()
    {
        if (sensor.CanSeePlayer)
        {
            ChangeState(NPCState.Chase);
            return;
        }

        if (sensor.CanHearPlayer)
        {
            searchTimer = searchDuration;
            ChangeState(NPCState.Search);
            return;
        }

        if (currentState == NPCState.Chase && hasLastKnownPosition)
        {
            searchTimer = searchDuration;
            ChangeState(NPCState.Search);
            return;
        }

        if (currentState == NPCState.Search && searchTimer <= 0f)
        {
            hasLastKnownPosition = false;
            ChangeState(NPCState.Patrol);
        }
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case NPCState.Patrol: Patrol(); break;
            case NPCState.Chase:  Chase(); break;
            case NPCState.Search: Search(); break;
        }
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                patrolIndex++;
                if (patrolIndex >= patrolPoints.Length) patrolIndex = 0;
                GoToCurrentPatrolPoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            isWaiting = true;
            waitTimer = waitTimeAtWaypoint;
            agent.ResetPath();
        }
    }

    private void GoToCurrentPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    private void Chase()
    {
        agent.speed = chaseSpeed;
        if (sensor.Player == null) return;
        agent.SetDestination(sensor.Player.position);
    }

    private void Search()
    {
        agent.speed = patrolSpeed;
        agent.SetDestination(lastKnownPosition);

        if (!agent.pathPending && agent.remainingDistance <= searchTolerance)
        {
            agent.ResetPath();
            lookTimer += Time.deltaTime;
            float direction = lookRight ? 1f : -1f;
            transform.Rotate(0f, direction * searchRotationSpeed * Time.deltaTime, 0f);

            if (lookTimer >= 1f)
            {
                lookRight = !lookRight;
                lookTimer = 0f;
            }

            searchTimer -= Time.deltaTime;
        }
    }

    private void ChangeState(NPCState newState)
    {
        if (currentState == newState) return;
        previousState = currentState;
        currentState = newState;
        Debug.Log(gameObject.name + ": " + previousState + " -> " + currentState);

        if (currentState == NPCState.Patrol)
        {
            GoToCurrentPatrolPoint();
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch (currentState)
        {
            case NPCState.Patrol: Gizmos.color = Color.green; break;
            case NPCState.Chase:  Gizmos.color = Color.red; break;
            case NPCState.Search: Gizmos.color = Color.blue; break;
        }
        Gizmos.DrawWireSphere(transform.position, 0.8f);

        if (hasLastKnownPosition)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(lastKnownPosition, 0.3f);
            Gizmos.DrawLine(transform.position, lastKnownPosition);
        }
    }
}
```
</details>

### <ins> 4.2 Overview State NPC </ins>
NPC Guard memiliki 3 status utama (`NPCState`):
- **Patrol**: NPC mengitari rute titik patroli (*waypoint*) secara berurutan.
- **Chase**: NPC mengejar player secara aktif ketika player terlihat oleh sensor penglihatan.
- **Search**: NPC bergerak menuju dan memeriksa lokasi terakhir player terdeteksi (*last known position*).

```
           ┌──────────┐  [Player Terlihat]  ┌──────────┐
           │  PATROL  │────────────────────►│  CHASE   │
           └──────────┘                     └──────────┘
                 ▲                                │
                 │                                │ [Player Hilang /
                 │       ┌──────────┐             │  Terdengar]
                 └───────│  SEARCH  │◄────────────┘
     [Search Selesai]    └──────────┘
```

### <ins> 4.3 Inisialisasi & Loop Utama </ins>
Method `Start()` menginisialisasi state awal ke `Patrol` dan mengarahkan NPC ke titik patroli pertama.  
Method `Update()` memanggil 4 fungsi utama setiap *frame* secara berurutan:

```csharp
private void Start()
{
    currentState = NPCState.Patrol;
    previousState = currentState;
    GoToCurrentPatrolPoint();
}

private void Update()
{
    UpdateMemory();      // 1. Memperbarui memori posisi player
    MakeDecision();      // 2. Evaluasi kondisi & transisi state
    ExecuteCurrentState(); // 3. Eksekusi perilaku sesuai state terkini
    UpdateIndicators();  // 4. Update tampilan UI indikator (! / ?)
}
```

### <ins> 4.4 Sistem Sensor & Memori </ins>
NPC memperbarui koordinat posisi terakhir player (`lastKnownPosition`) berdasarkan dua sensor:
1. **Penglihatan (`sensor.CanSeePlayer`)**: Menyimpan posisi player saat terlihat.
2. **Pendengaran (`sensor.CanHearPlayer` - Challenge 4)**: Menyimpan posisi suara player saat terdengar.

```csharp
private void UpdateMemory()
{
    if (sensor.CanSeePlayer)
    {
        lastKnownPosition = sensor.Player.position;
        hasLastKnownPosition = true;
    }

    if (sensor.CanHearPlayer) // Challenge 4
    {
        lastKnownPosition = sensor.Player.position;
        hasLastKnownPosition = true;
    }
}
```

### <ins> 4.5 Logika Pengambilan Keputusan </ins>
Proses pengambilan keputusan dievaluasi berdasarkan urutan prioritas:
1. **Prioritas 1 (Chase)**: Jika player terlihat (`sensor.CanSeePlayer`), ubah state ke `Chase`.
2. **Prioritas 2 (Search via Suara - Challenge 4)**: Jika player terdengar (`sensor.CanHearPlayer`), setel `searchTimer = searchDuration` dan ubah state ke `Search`.
3. **Prioritas 3 (Search via Kehilangan Penglihatan)**: Jika sebelumnya `Chase` lalu player menghilang dan ada `hasLastKnownPosition`, ubah state ke `Search`.
4. **Prioritas 4 (Patrol)**: Jika sedang `Search` dan `searchTimer <= 0f`, reset `hasLastKnownPosition` dan kembali ke `Patrol`.

```csharp
private void MakeDecision()
{
    if (sensor.CanSeePlayer)
    {
        ChangeState(NPCState.Chase);
        return;
    }

    if (sensor.CanHearPlayer) // Challenge 4
    {
        searchTimer = searchDuration;
        ChangeState(NPCState.Search);
        return;
    }

    if (currentState == NPCState.Chase && hasLastKnownPosition)
    {
        searchTimer = searchDuration;
        ChangeState(NPCState.Search);
        return;
    }

    if (currentState == NPCState.Search && searchTimer <= 0f)
    {
        hasLastKnownPosition = false;
        ChangeState(NPCState.Patrol);
    }
}
```

Fungsi `ChangeState()` menangani perubahan state, mencetak log ke console, dan memanggil `GoToCurrentPatrolPoint()` saat kembali ke `Patrol`.

### <ins> 4.6 Eksekusi Perilaku State & Challenge </ins>

#### 2.6.1 State Patrol & Jeda Waypoint (Challenge 1)
Pada state `Patrol`, NPC bergerak menuju titik patroli menggunakan `patrolSpeed`. Ketika mencapai titik target (jarak sisa $\le$ `waypointTolerance`), NPC akan berhenti sejenak selama `waitTimeAtWaypoint` (2 detik) sebelum beralih ke titik berikutnya.

```csharp
private void Patrol()
{
    agent.speed = patrolSpeed;
    if (patrolPoints == null || patrolPoints.Length == 0) return;

    if (isWaiting) // Challenge 1
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            isWaiting = false;
            patrolIndex++;
            if (patrolIndex >= patrolPoints.Length) patrolIndex = 0;
            GoToCurrentPatrolPoint();
        }
        return;
    }

    if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
    {
        isWaiting = true;
        waitTimer = waitTimeAtWaypoint;
        agent.ResetPath();
    }
}
```

#### 4.6.2 State Chase
Pada state `Chase`, NPC meningkatkan kecepatannya ke `chaseSpeed` (`4f`) dan secara terus-menerus memperbarui tujuan navigasi (`SetDestination`) ke posisi player terkini.

```csharp
private void Chase()
{
    agent.speed = chaseSpeed;
    if (sensor.Player == null) return;
    agent.SetDestination(sensor.Player.position);
}
```

#### 4.6.3 State Search & Rotasi Melihat Kiri-Kanan (Challenge 2)
Pada state `Search`, NPC bergerak menuju `lastKnownPosition`. Setelah tiba di lokasi tersebut, NPC menghentikan navigasi dan mulai nelihat kiri-kanan menggunakan `searchRotationSpeed` (`90f`) hingga durasi `searchTimer` habis.

```csharp
private void Search()
{
    agent.speed = patrolSpeed;
    agent.SetDestination(lastKnownPosition);

    if (!agent.pathPending && agent.remainingDistance <= searchTolerance)
    {
        agent.ResetPath();

        // Challenge 2: Rotasi geleng kepala kiri & kanan
        lookTimer += Time.deltaTime;
        float direction = lookRight ? 1f : -1f;
        transform.Rotate(0f, direction * searchRotationSpeed * Time.deltaTime, 0f);

        if (lookTimer >= 1f)
        {
            lookRight = !lookRight;
            lookTimer = 0f;
        }

        searchTimer -= Time.deltaTime;
    }
}
```

### <ins> 4.7 Indikator Alert (Challenge 3) </ins>
Fungsi `UpdateIndicators()` mengatur aktif/tidaknya ikon indikator visual di atas kepala NPC:
- `alertExclamation` (`!`): Aktif hanya saat state **`Chase`**.
- `alertQuestion` (`?`): Aktif hanya saat state **`Search`**.

```csharp
private void UpdateIndicators()
{
    if (alertExclamation != null)
        alertExclamation.SetActive(currentState == NPCState.Chase);

    if (alertQuestion != null)
        alertQuestion.SetActive(currentState == NPCState.Search);
}
```

### <ins> 4.8 Gizmos </ins>
Method `OnDrawGizmosSelected()` menggambar indikator lingkaran berwarna pada Scene View Unity sesuai state aktif:
- **Hijau**: State `Patrol`
- **Merah**: State `Chase`
- **Biru**: State `Search`
- **Magenta**: Objek bola & garis dari NPC menuju `lastKnownPosition`

```csharp
private void OnDrawGizmosSelected()
{
    switch (currentState)
    {
        case NPCState.Patrol: Gizmos.color = Color.green; break;
        case NPCState.Chase:  Gizmos.color = Color.red; break;
        case NPCState.Search: Gizmos.color = Color.blue; break;
    }
    Gizmos.DrawWireSphere(transform.position, 0.8f);

    if (hasLastKnownPosition)
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(lastKnownPosition, 0.3f);
        Gizmos.DrawLine(transform.position, lastKnownPosition);
    }
}
```

---

##  5. `PlayerController`
```
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTarget;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchSpeed = 2f;

    private bool isCrouching;
    public bool IsCrouching => isCrouching;
```
Mendefinisikan variabel untuk mengatur pergerakan, rotasi, referensi kamera, dan kondisi crouch pemain.
- moveSpeed: Kecepatan gerak normal pemain dengan nilai awal 5.
- rotationSpeed: Kecepatan perubahan rotasi pemain agar dapat menghadap arah gerak.
- cameraTarget: Referensi Transform kamera yang digunakan untuk menentukan arah gerak relatif terhadap kamera.
- crouchSpeed: Kecepatan gerak ketika pemain crouch, yaitu 2.
- isCrouching: Menyimpan status apakah pemain sedang crouch.
- IsCrouching: Property untuk membaca status crouch dari script lain.

```
private void Update()
{
    isCrouching = Input.GetKey(KeyCode.LeftControl);

    MovePlayer();
}
```
Update() dijalankan setiap frame. Fungsi ini memeriksa apakah tombol CTRL kiri sedang ditekan menggunakan Input.GetKey(). Jika tombol ditekan, isCrouching bernilai true dan pemain menggunakan kecepatan crouch. Setelah itu, MovePlayer() dipanggil untuk memproses pergerakan.

```
private void MovePlayer()
{
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");

    float currentSpeed =
        isCrouching
        ? crouchSpeed
        : moveSpeed;
```
Input.GetAxisRaw() membaca input keyboard pada axis horizontal dan vertical, umumnya menggunakan tombol A/D dan W/S. Operator ternary menentukan kecepatan berdasarkan kondisi crouch:
- Jika crouch, menggunakan crouchSpeed.
- Jika tidak crouch, menggunakan moveSpeed.

```
Vector3 inputDirection =
    new Vector3(horizontal, 0f, vertical).normalized;

if (inputDirection == Vector3.zero)
    return;
```
Input keyboard diubah menjadi vector arah 3D. Komponen Y bernilai 0 agar gerakan dilakukan pada bidang horizontal. .normalized digunakan untuk menjaga panjang vector tetap 1, sedangkan return menghentikan fungsi jika tidak ada input gerak.

```
Vector3 camForward = cameraTarget.forward;
Vector3 camRight = cameraTarget.right;

camForward.y = 0f;
camRight.y = 0f;

camForward.Normalize();
camRight.Normalize();

Vector3 movement =
    camForward * inputDirection.z +
    camRight * inputDirection.x;

movement.Normalize();
```
Mengambil arah depan dan kanan kamera sebagai acuan gerak pemain. Komponen Y diatur menjadi 0 agar rotasi vertikal kamera tidak memengaruhi pergerakan karakter. Setelah itu, vector dinormalisasi untuk menjaga panjangnya tetap 1. Arah depan dan kanan kamera digabungkan dengan input pemain untuk menghasilkan arah gerak akhir. Dengan cara ini, pemain bergerak relatif terhadap arah kamera.

```
transform.position +=
    movement *
    currentSpeed *
    Time.deltaTime;
```
Posisi pemain diperbarui berdasarkan arah gerak, kecepatan, dan waktu. Time.deltaTime digunakan agar pergerakan lebih konsisten pada frame rate yang berbeda.

```
Quaternion targetRotation =
    Quaternion.LookRotation(movement);

transform.rotation =
    Quaternion.Slerp(
        transform.rotation,
        targetRotation,
        rotationSpeed * Time.deltaTime
    );
```
Quaternion.LookRotation() menghasilkan rotasi yang menghadap arah gerak pemain. Kemudian, Quaternion.Slerp() digunakan untuk mengubah rotasi secara halus dari rotasi saat ini menuju rotasi tujuan. Hal ini membuat karakter tidak langsung berputar secara kaku ketika arah geraknya berubah.
