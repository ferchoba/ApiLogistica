# Logistica 馃摝

Logistica es una plataforma para la agregar, validar y orquestar diferentes formatos de manifiestos de entrega.

## Arquitectura Base

El sistema está construido siguiendo los principios de **Clean Architecture**, asegurando un bajo acoplamiento y alta cohesión mediante la separación estricta en capas (API, Application, Domain, Infrastructure).

## Prerrequisitos

Para ejecutar este proyecto, necesitas tener instalado el siguiente entorno:

* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (o superior)
* Tu IDE de preferencia (Visual Studio 2022, JetBrains Rider, o VS Code)

## Instrucciones de Ejecuci贸n

Sigue estos pasos desde tu terminal para compilar y ejecutar el proyecto localmente. Aseg煤rate de estar ubicado en el directorio ra铆z de la soluci贸n (donde se encuentra `Logistica.sln`):

1. **Restaurar las dependencias del proyecto:**
   dotnet restore

2. **Compilar la soluci贸n:**
   dotnet build --no-restore

3. **Ejecutar el proyecto API:**
   dotnet run --project src/Logistica.API/Logistica.API.csproj

   *(El puerto por defecto se mostrar谩 en la consola una vez la aplicaci贸n inicie, t铆picamente `https://localhost:5001` o `http://localhost:5000`)*.

## Uso de la API (Swagger)

La API cuenta con una interfaz interactiva de documentaci贸n y pruebas prove铆da por Swagger. Una vez que la aplicaci贸n est茅 en ejecuci贸n, puedes acceder a ella navegando a:

馃憠 `https://localhost:<puerto>/swagger` o `http://localhost:<puerto>/swagger`

Desde la interfaz de Swagger podr谩s interactuar con los endpoints disponibles, enviar cargas de prueba (CSV, JSON, TXT, XML) y validar la respuesta del motor de procesamiento y orquestaci贸n.
