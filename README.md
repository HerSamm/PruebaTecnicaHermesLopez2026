# PruebaTecnicaHermesLopez2026

Desarrollo de Prueba Técnica .NET - Período 2026.

API REST para gestionar usuarios, direcciones y monedas, con conversión de divisas.

## Tecnologías

- .NET 9 (Minimal API)
- Entity Framework Core 9 + SQLite
- FluentValidation
- Swagger

## Cómo ejecutar

Requisito: SDK de .NET 9.

```
dotnet run --project PruebaTecnicaHermes.Api
```

Swagger: http://localhost:5012/swagger

También se puede abrir `PruebaTecnicaHermes.sln` en Visual Studio y ejecutar con F5.

## Base de datos

Al iniciar, la API aplica las migraciones y crea el archivo `pruebatecnicahermes.db` (SQLite). No hace falta ningún
paso extra. Para empezar con la base vacía se puede borrar ese archivo y volver a ejecutar.

Para borrar la bd si es que ya cargamos datos: 

```
Remove-Item PruebaTecnicaHermes.Api\pruebatecnicahermes.db*
```

Monedas cargadas por defecto (moneda base: PYG):

| Code | Name | RateToBase |
|---|---|---|
| PYG | Guaraní | 1 |
| USD | Dólar | 6100 |
| EUR | Euro | 6250 |
| BRL | Real | 1350 |

Para aplicar las migraciones manualmente:

```bash
dotnet tool restore
dotnet ef database update --project PruebaTecnicaHermes.Api
```

## API Key

Todas las peticiones requieren el header:

```
X-API-KEY: prueba-tecnica-hermes-2026
```

Si falta o es incorrecta, la API responde 401. En Swagger se carga con el botón Authorize.

## Endpoints

### Users

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/users` | Crear usuario |
| GET | `/users` | Listar usuarios (filtro opcional `?isActive=true/false`) |
| GET | `/users/{id}` | Obtener usuario |
| PUT | `/users/{id}` | Editar usuario |
| DELETE | `/users/{id}` | Eliminar usuario (y sus direcciones) |
| POST | `/users/bulk` | Carga masiva |

### Addresses

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/users/{userId}/addresses` | Crear dirección |
| GET | `/users/{userId}/addresses` | Listar direcciones del usuario |
| PUT | `/addresses/{id}` | Editar dirección |
| DELETE | `/addresses/{id}` | Eliminar dirección |

### Currencies

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/currencies` | Listar monedas |
| POST | `/currencies` | Crear moneda |
| POST | `/currency/convert` | Convertir un monto entre monedas |

Conversión: `amount * from.RateToBase / to.RateToBase`, redondeado a 2 decimales. Ejemplo: 100 USD = 610000 PYG.

### Respuestas

| Código | Caso |
|---|---|
| 200 / 201 / 204 | Correcto |
| 400 | Datos inválidos |
| 401 | API Key faltante o incorrecta |
| 404 | No existe el usuario, la dirección o la moneda |
| 409 | El email o el código de moneda ya existe |

## Carga masiva

`POST /users/bulk` recibe una lista de usuarios y los inserta en paralelo con `Task.WhenAll`. Cada inserción usa su
propio `DbContext` creado con `IDbContextFactory`. Cada usuario se valida por separado, así que si uno falla el resto
se guarda igual. La respuesta indica cuántos se crearon y el motivo de los que fallaron:

```json
{
  "total": 6,
  "created": 2,
  "failed": 4,
  "createdUsers": [ ... ],
  "errors": [
    { "index": 3, "email": "esto-no-es-un-email", "reason": "El email no tiene un formato válido." }
  ]
}
```

## Datos de prueba

En `sample-data/` hay un JSON por petición (`users/`, `addresses/`, `currencies/`) y los de carga masiva (`bulk/`).
En `sample-data/README.md` está el resultado esperado de cada uno.

## Estructura

```
PruebaTecnicaHermes.Api/
├── Domain/            Entidades: User, Address, Currency
├── Infrastructure/    AppDbContext y migraciones
├── Application/       Commands y Queries con sus validadores y handlers
├── Endpoints/         Rutas de la API
├── Middleware/        API Key y manejo de errores
└── Program.cs
```

## Decisiones

- **CQRS sin MediatR:** cada caso de uso tiene su comando o consulta, su validador y su handler, separados en
  carpetas `Commands` y `Queries`.
- **IDbContextFactory:** todos los handlers crean su propio `DbContext`, lo que permite trabajar en paralelo.
- **Paralelismo:** se usa `Task.WhenAll` en la carga masiva y en la conversión de divisas (se buscan las dos monedas
  a la vez).
- **Validaciones:** FluentValidation valida los datos de entrada. Que el email o el código de moneda no estén
  repetidos se controla en el handler y además con índices únicos en la base.
- **Password:** es opcional, ya que el enunciado solo marca como requeridos Name y Email. Si se envía debe tener al
  menos 6 caracteres y se guarda hasheada. Nunca se devuelve en las respuestas.
- Los emails se guardan en minúsculas y los códigos de moneda en mayúsculas.
- El swagger guarda el apikey que ingresamos al querer autorizarle para realizar las pruebas, para no ponerla cada vez que se lance, para realizar la prueba.