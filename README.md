# Trabajo Práctico Integrador
## Desarrollo de Software
### Backend

## Introducción
Se desea desarrollar una plataforma de comercio electrónico (E-commerce). 
En esta primera etapa el objetivo es construir el módulo de Órdenes, permitiendo la gestión completa de éstas.

## Características de la Solución

- Lenguaje: C# 12.0
- Plataforma: .NET 8

## Integrantes
-@FeeerJ

## Instrucciones para el correcto funcionamiento del proyecto
### Herramientas Necesarias
- .NET 8 SDK
- SQL Server/ localdB (Incluido en Visual Studio)
- Visual Studio Community 2022 (+ASP.NET CORE)
- Git

### Pasos a seguir
1. Clonar el repositorio:
   Tendras que usar la siguiente serie de comandos en tu terminal.
   ```bash
   git clone https://github.com/FeeerJ/TPI_PT1-DSW2025
   cd TPI_TP1-DSW2025
2. Configurar la base de datos: 
   Ten en cuenta que la cadena de conexion utilizada sea la correcta segun tu entorno preferido. La utilizada por defecto es (localdb)\\mssqllocaldb
   ```bash
    "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Dsw2025Tpi;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
3. Aplicar las migraciones y poblar la base de datos:
   Se espera la aplicacion automatica de las migraciones y la carga de los datos iniciales desde un archivo JSON al iniciar el programa.
   ```bash
   // ...
   using (var scope = app.Services.CreateScope())
   {
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Dsw2025TpiContext>();
        context.Database.Migrate(); // Aplica las migraciones pendientes
        context.Seedwork<Customer>("sources/Customers.json"); // Usa el método de extensión para seedear los clientes
    }
    // ...
    }
4. Ejecutar el proyecto
   Se puede hacer simplemente tocando F5, tambien podemos usar la linea de comandos como sigue:
   ```bash
    dotnet run --project Dsw2025Tpi.Api/Dsw2025Tpi.Api.csproj


 
 
# EndPoints Disponibles

## Products

| Método | Ruta               | Descripción                        |
|--------|--------------------|----------------------------------|
| GET    | `/api/products`     | Obtiene todos los productos disponibles |
| GET    | `/api/products/{id}`| Obtiene un producto por su ID    |
| POST   | `/api/products`     | Agrega un nuevo producto          |
| PUT    | `/api/products/{id}`| Actualiza los datos de un producto|
| PATCH  | `/api/products/{id}`| Desactiva un producto             |

## Orders

| Método | Ruta           | Descripción                      |
|--------|----------------|--------------------------------|
| POST   | `/api/orders`  | Crea una nueva orden, validando stock |
| GET    | `/api/orders`     | Obtiene todas las ordenes  |
| GET    | `/api/orders/{id}`| Obtiene una orden por su ID    |
| PUT    | `/api/order/{id}/status`| Actualiza el estado de una orden|
