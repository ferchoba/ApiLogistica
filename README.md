# Logistica

Logistica es una plataforma para la agregar, validar y orquestar diferentes formatos de manifiestos de entrega.

## Arquitectura Base

El sistema está construido siguiendo los principios de **Clean Architecture**, asegurando un bajo acoplamiento y alta cohesión mediante la separación estricta en capas (API, Application, Domain, Infrastructure).

## Prerrequisitos

Para ejecutar este proyecto, necesitas tener instalado el siguiente entorno:

* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (o superior)
* Tu IDE de preferencia (Visual Studio 2022, JetBrains Rider, o VS Code)

## Instrucciones de Ejecución

Sigue estos pasos desde tu terminal para compilar y ejecutar el proyecto localmente. Asegúrate de estar ubicado en el directorio raíz de la solución (donde se encuentra `Logistica.sln`):

1. **Restaurar las dependencias del proyecto:**
   dotnet restore

2. **Compilar la solución:**
   dotnet build --no-restore

3. **Ejecutar el proyecto API:**
   dotnet run --project src/Logistica.API/Logistica.API.csproj

   *(El puerto por defecto se mostrará en la consola una vez la aplicación inicie, típicamente `https://localhost:5001` o `http://localhost:5000`)*.

## Uso de la API (Swagger)

La API cuenta con una interfaz interactiva de documentación y pruebas proveída por Swagger. Una vez que la aplicación esté en ejecución, puedes acceder a ella navegando a:

`https://localhost:<puerto>/swagger` o `http://localhost:<puerto>/swagger`

Desde la interfaz de Swagger podrás interactuar con los endpoints disponibles, enviar cargas de prueba (CSV, JSON, TXT, XML) y validar la respuesta del motor de procesamiento y orquestación
