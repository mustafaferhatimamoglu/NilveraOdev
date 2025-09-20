# Nilvera .NET Core Web API Assessment

Bu proje, Nilvera teknik deÄŸerlendirme dokÃ¼manÄ±nda yer alan gereksinimlere uygun olarak hazÄ±rlanmÄ±ÅŸ Ã¶rnek bir .NET 8 Web API uygulamasÄ±dÄ±r. Sistem; CQRS, MediatR, Dapper ve SQL Server saklÄ± yordamlarÄ± kullanarak mÃ¼ÅŸteri (Customer) yÃ¶netimi iÃ§in CRUD uÃ§ noktalarÄ± saÄŸlar. JSON serileÅŸtirme / deserileÅŸtirme iÅŸlemi mÃ¼ÅŸteri iletiÅŸim bilgilerini veritabanÄ±nda NVARCHAR(MAX) bir sÃ¼tunda saklamak iÃ§in kullanÄ±lmÄ±ÅŸtÄ±r.

## YapÄ±landÄ±rma

- VarsayÄ±lan baÄŸlantÄ± dizesi appsettings.json dosyasÄ±nda SqlServer anahtarÄ± altÄ±nda yer alÄ±r:

    ~~~json
    {
      "ConnectionStrings": {
        "SqlServer": "Server=localhost;Database=master;User Id=sa;Password=<YourStrong!Passw0rd>123;TrustServerCertificate=True;"
      }
    }
    ~~~

- KullanÄ±cÄ± adÄ± sa, parola ise <YourStrong!Passw0rd>123 olarak tanÄ±mlanmÄ±ÅŸtÄ±r. FarklÄ± bir ortamda Ã§alÄ±ÅŸtÄ±rmak isterseniz baÄŸlantÄ± dizesini gÃ¼ncellemeniz yeterlidir.

## VeritabanÄ± kurulumu

1. SQL Server Ã¼zerinde master adÄ±nda bir veritabanÄ± oluÅŸturun (veya appsettings dosyasÄ±ndaki isimle eÅŸleÅŸen bir veritabanÄ± kullanÄ±n).
2. Database/Scripts/Customers.sql betiÄŸini Ã§alÄ±ÅŸtÄ±rarak Customers tablosunu ve gerekli saklÄ± yordamlarÄ± (usp_Customers_*) oluÅŸturun.

## UygulamayÄ± Ã§alÄ±ÅŸtÄ±rma

1. Gerekli NuGet paketlerini indirmek iÃ§in dotnet restore komutunu Ã§alÄ±ÅŸtÄ±rÄ±n.
2. UygulamayÄ± baÅŸlatmak iÃ§in proje kÃ¶k dizininde aÅŸaÄŸÄ±daki komutu kullanÄ±n:

    ~~~bash
    dotnet run
    ~~~

3. GeliÅŸtirme ortamÄ±nda Swagger arayÃ¼zÃ¼ne https://localhost:5001/swagger (veya terminal Ã§Ä±ktÄ±sÄ±nda belirtilen port) Ã¼zerinden eriÅŸebilirsiniz.

## Ã–rnek istekler

### MÃ¼ÅŸteri oluÅŸturma

    POST /api/customers
    Content-Type: application/json

    {
      "firstName": "Ada",
      "lastName": "Lovelace",
      "contactInfo": {
        "email": "ada@example.com",
        "phoneNumber": "+90 555 444 3322",
        "address": {
          "street": "Baker Street 221B",
          "city": "Istanbul",
          "country": "Turkey",
          "postalCode": "34000"
        }
      }
    }

### MÃ¼ÅŸteri listeleme

    GET /api/customers

API, saklÄ± yordamlar Ã¼zerinden tÃ¼m CRUD iÅŸlemlerini gerÃ§ekleÅŸtirir ve veritabanÄ±nda tutulan JSON verisini otomatik olarak modele deserileÅŸtirerek istemciye dÃ¶ndÃ¼rÃ¼r.

## Mimari Notlar

- CQRS & MediatR: Komutlar ve sorgular Features/Customers dizininde MediatR istekleri olarak organize edilmiÅŸtir.
- Dapper: TÃ¼m veritabanÄ± iÅŸlemleri saklÄ± yordamlar aracÄ±lÄ±ÄŸÄ±yla Dapper kullanÄ±larak yapÄ±lÄ±r.
- JSON Serialize/Deserialize: Infrastructure/Database/Mappers/CustomerMapper sÄ±nÄ±fÄ±, CustomerContactInfo nesnesini JSON'a serileÅŸtirmek ve tersine Ã§evirmekten sorumludur.
- BaÄŸÄ±mlÄ±lÄ±k enjeksiyonu: Program.cs iÃ§inde MediatR ve Ã¶zel ISqlConnectionFactory servisi projeye eklenmiÅŸtir.

Gereksinimlerde belirtilen tÃ¼m teknolojiler kullanÄ±lmakta olup proje Docker ile paketlenmeye hazÄ±rdÄ±r (gerekirse Dockerfile gÃ¼ncellenebilir). Ek geliÅŸtirme veya iyileÅŸtirme ihtiyaÃ§larÄ± iÃ§in lÃ¼tfen bilgi verin.

## Testler

- `dotnet test` komutu, appsettings dosyalarındaki `SqlServer` bağlantı dizesini kullanarak gerçek SQL Server üzerinde testi yürütür.
- Saklı yordam senaryoları önce `Database/Scripts/Customers.sql` betiğini çalıştırır ve gereken objeleri günceller.
- Testler geçici kayıtlar oluşturur; başarıyla tamamlandığında ilgili kayıtları `usp_Customers_Delete` ile temizler. İsteğe göre testleri durdurup SQL Server Management Studio üzerinden ara sonuçları inceleyebilirsiniz.
