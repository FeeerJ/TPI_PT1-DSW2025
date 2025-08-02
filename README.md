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
   dotnet ef migrations add InitialCreate
   dotnet ef database update
2. Configurar la base de datos: 
   Ten en cuenta que la cadena de conexion utilizada sea la correcta segun tu entorno preferido. La utilizada por defecto es (localdb)\\mssqllocaldb
3. Aplicar las migraciones y poblar la base de datos:
   Se espera la aplicacion automatica de las migraciones y la carga de los datos iniciales desde un archivo JSON al iniciar el programa.

