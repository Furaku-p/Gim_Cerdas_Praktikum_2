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

#### Parameter & Variabel
| Nama Variabel | Tipe Data | Akses | Deskripsi |
|---|---|---|---|
| `player` | `Transform` | `public` | Referensi komponen `Transform` milik objek Player yang akan diikuti |

---

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

---

### <ins> Demo / Tampilan Hasil </ins>
*(Tambahkan Screenshot atau GIF animasi kamera mengikuti player di sini)*  
`![Demo Camera Follow](docs/images/camera_follow_demo.gif)`

---

## 2. `NPCBrain`
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

---

### <ins> 2.1 Overview State NPC </ins>
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

---

### <ins> 2.2 Inisialisasi & Loop Utama </ins>
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

---

### <ins> 2.3 Sistem Sensor & Memori </ins>
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

---

### <ins> 2.4 Logika Pengambilan Keputusan </ins>
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

---

### <ins> 2.5 Eksekusi Perilaku State & Challenge </ins>

#### 2.5.1 State Patrol & Jeda Waypoint (Challenge 1)
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

#### 2.5.2 State Chase
Pada state `Chase`, NPC meningkatkan kecepatannya ke `chaseSpeed` (`4f`) dan secara terus-menerus memperbarui tujuan navigasi (`SetDestination`) ke posisi player terkini.

```csharp
private void Chase()
{
    agent.speed = chaseSpeed;
    if (sensor.Player == null) return;
    agent.SetDestination(sensor.Player.position);
}
```

#### 2.5.3 State Search & Rotasi Melihat Kiri-Kanan (Challenge 2)
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

---

### <ins> 2.6 Indikator Alert (Challenge 3) </ins>
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

---

### <ins> 2.7 Gizmos </ins>
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

### <ins> 2.8 Kode Lengkap </ins>

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

---

### <ins> Demo / Tampilan Hasil </ins>
*(Tambahkan Screenshot atau GIF animasi pergerakan FSM NPC Guard di sini)*  
`![Demo NPC FSM](docs/images/npc_guard_demo.gif)`
