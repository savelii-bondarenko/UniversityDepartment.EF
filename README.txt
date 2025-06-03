Для супер-користувача в БД
  Логін - sa 
  Пароль - StrongPassw0rd\!
  "DefaultConnection": "Server=localhost,1433;Database=ElectronicDepartmentDb;User=sa;Password=StrongPassw0rd!;TrustServerCertificate=True;"

Команди для міграцій
	dotnet ef migrations add InitialCreate --project ElectronicDepartment.DAL --startup-project ElectronicDepartment.PL
	dotnet ef database update --project ElectronicDepartment.DAL --startup-project ElectronicDepartment.PL




