# 🛡️ CTI HUB - Cyber Threat Intelligence Platform

![.NET](https://img.shields.io/badge/.NET-8.0%2F9.0-512BD4?style=flat&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=flat&logo=docker)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-336791?style=flat&logo=postgresql)
![License](https://img.shields.io/badge/License-MIT-green)
![Status](https://img.shields.io/badge/Status-Under%20Development-orange)

> **Modern, ölçeklenebilir ve yapay zeka destekli yeni nesil siber tehdit istihbarat ve zafiyet yönetim platformu.**

---

## 📖 Proje Hakkında

**CTI Hub**, klasik port tarama araçlarının ötesine geçerek, hedef sistemleri analiz eden, üzerindeki servisleri ve versiyonları tespit eden, bu versiyonları küresel zafiyet veritabanlarıyla (NVD/CVE) eşleştiren ve yapay zeka destekli çözüm önerileri sunan bir **Karar Destek Sistemidir (Decision Support System).**

Bu proje, **Software Engineering** prensiplerine sadık kalınarak, **Clean Architecture (Onion)** mimarisi üzerine inşa edilmiştir ve **Microservices** prensiplerine (Asenkron İletişim, Dağıtık Yapı) uygun olarak geliştirilmektedir.

---

## 🏗️ Mimari ve Teknoloji Yığını

Proje, **Polyglot (Çok Dilli)** ve **Hibrit** bir yapıya sahiptir. Her iş için en uygun teknoloji seçilmiştir:

### 🔧 Backend Core (Yönetim Merkezi)
* **Dil:** C# (.NET 8/9)
* **Mimari:** Onion / Clean Architecture (Domain, Application, Infrastructure, WebApi)
* **Veritabanı:** PostgreSQL (Entity Framework Core - Code First)
* **Güvenlik:** JWT Authentication, BCrypt Hashing, Role-Based Access Control (RBAC)
* **Validasyon:** FluentValidation

### ⚡ İletişim ve İşleme (Yakında)
* **Message Broker:** RabbitMQ (Asenkron İletişim için)
* **Scanner Service:** Go / Golang (Yüksek performanslı Nmap taramaları için)
* **Analysis Service:** Python (Veri zenginleştirme, CVE sorgulama ve AI entegrasyonu için)

### 🐳 DevOps & Altyapı
* **Containerization:** Docker & Docker Compose
* **OS:** Ubuntu Linux (Servisler için), Windows (Geliştirme için)

---

## 🗺️ Geliştirme Yol Haritası (Roadmap)

Proje, kurumsal bir ürün yaşam döngüsü simüle edilerek fazlar halinde geliştirilmektedir.

### ✅ Faz 1-3: Temel Altyapı (Tamamlandı)
- [x] Docker ortamının kurulması ve izole edilmesi.
- [x] PostgreSQL veritabanı entegrasyonu.
- [x] Clean Architecture katmanlarının oluşturulması.
- [x] Repository Pattern ve DTO yapısının kurulması.

### 🔄 Faz 4: Güvenlik ve Kimlik (Aktif Geliştirme)
- [x] Kullanıcı Kayıt/Giriş (JWT Token).
- [x] Şifreleme (BCrypt Hashing).
- [ ] Gelişmiş Validasyon Kuralları.
- [ ] Yetkilendirme (Authorization) Middleware.

### 🔜 Faz 5: Asenkron İletişim 
- [ ] RabbitMQ entegrasyonu.
- [ ] "Scan Request" kuyruk yapısının kurulması.
- [ ] Producer/Consumer mimarisinin kodlanması.

### 🔜 Faz 6: Tarama Motoru (Scanner)
- [ ] Go veya C# ile Worker Service yazılması.
- [ ] Nmap entegrasyonu (-sV Version Detection).
- [ ] Tarama sonuçlarının veritabanına işlenmesi.

### 🔮 Faz 7: Zeka ve Analiz (Enterprise Vision)
- [ ] Python Analiz Servisi entegrasyonu.
- [ ] **NVD/Vulners API:** Tespit edilen versiyonlar için CVE (Zafiyet) sorgusu.
- [ ] **AI Entegrasyonu (LLM):** Zafiyetler için çözüm reçetesi üreten yapay zeka modülü.

---

## 🚀 Kurulum (Nasıl Çalıştırılır?)

Proje tamamen Dockerize edilmiştir. Tek komutla ayağa kaldırılabilir.

### Gereksinimler
* Docker Desktop
* .NET 8/9 SDK (Geliştirme için)

### Adımlar

1.  **Repoyu Klonlayın:**
    ```bash
    git clone [https://github.com/feyzanurdandal/cti-hub.git](https://github.com/feyzanurdandal/cti-hub.git)
    cd cti-hub
    ```

2.  **Docker ile Başlatın:**
    ```bash
    docker-compose up -d --build
    ```

3.  **Erişim:**
    * **API (Swagger):** `http://localhost:5075/swagger` (Port değişebilir)
    * **Veritabanı:** `localhost:5432`

---

## 🤝 Katkıda Bulunma

Bu proje açık kaynaklıdır. Pull Request'ler ve Issue'lar memnuniyetle karşılanır. Özellikle "Scanner" ve "AI Analysis" modülleri için fikirlerinizi bekliyorum.

---

## 👤 İletişim

**Feyza Nur Dandal** - *Software Engineering Student @ DPÜ*

* 💼 [LinkedIn](https://www.linkedin.com/in/feyza-nur-dandal-7a290720b/)
* 💻 [GitHub](https://github.com/feyzanurdandal)

---