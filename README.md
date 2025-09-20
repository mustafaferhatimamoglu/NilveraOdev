# Nilvera .NET Core Web API Assessment

Windows makinede Docker(linux) için ayarlanmıştır

 appsettings.json 

    ~~~json
    {
      "ConnectionStrings": {
        "SqlServer": "Server=localhost;Database=master;User Id=sa;Password=<YourStrong!Passw0rd>123;TrustServerCertificate=True;"
      }
    }
    ~~~

kullanacağınız mssql bağlantı bilgilerini güncelleyin
Database/Scripts/Customers.sql ilgili db üzerinde çalıştırın 
Test olmayan programı derleyin

dotnet test Tests/NilveraOdev.Tests/NilveraOdev.Tests.csproj -v minimal 
testleri çalıştırmak için cmd komutu

profiler ile doğrulayabilirsiniz
