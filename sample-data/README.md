# Datos de prueba

Cada archivo es el body de una petición. Los resultados suponen la base vacía y las pruebas hechas en este orden.

## Users

| Petición | Archivo | Resultado |
|---|---|---|
| `POST /users` | `users/01_crear_usuario.json` | 201, usuario id 1 |
| `POST /users` | `users/02_crear_usuario_con_password.json` | 201, usuario id 2 |
| `POST /users` | `users/03_crear_usuario_email_duplicado.json` | 409, email ya existe |
| `POST /users` | `users/04_crear_usuario_datos_invalidos.json` | 400, errores en Name y Email |
| `POST /users` | `users/05_crear_usuario_password_corta.json` | 400, mínimo 6 caracteres |
| `PUT /users/1` | `users/06_editar_usuario.json` | 200, usuario editado |
| `PUT /users/1` | `users/07_editar_usuario_desactivar.json` | 200, `isActive: false` |
| `PUT /users/1` | `users/08_editar_usuario_email_de_otro.json` | 409, email de otro usuario |
| `PUT /users/1` | `users/09_editar_usuario_datos_invalidos.json` | 400, errores en Name y Email |
| `GET /users?isActive=false` | - | 200, solo el usuario 1 |
| `GET /users/999` | - | 404 |

## Addresses

| Petición | Archivo | Resultado |
|---|---|---|
| `POST /users/1/addresses` | `addresses/01_crear_direccion.json` | 201, dirección id 1 |
| `POST /users/1/addresses` | `addresses/02_crear_direccion_sin_codigo_postal.json` | 201, dirección id 2 |
| `POST /users/1/addresses` | `addresses/03_crear_direccion_datos_invalidos.json` | 400, errores en Street, City y Country |
| `POST /users/999/addresses` | `addresses/01_crear_direccion.json` | 404, el usuario no existe |
| `PUT /addresses/1` | `addresses/04_editar_direccion.json` | 200, dirección editada |
| `PUT /addresses/1` | `addresses/05_editar_direccion_datos_invalidos.json` | 400, errores en Street y Country |
| `PUT /addresses/999` | `addresses/04_editar_direccion.json` | 404 |
| `DELETE /addresses/2` | - | 204 |
| `GET /users/1/addresses` | - | 200, solo la dirección 1 |
| `DELETE /users/1` | - | 204, se borran también sus direcciones |

## Currencies

| Petición | Archivo | Resultado |
|---|---|---|
| `POST /currencies` | `currencies/01_crear_moneda.json` | 201, moneda ARS |
| `POST /currencies` | `currencies/02_crear_moneda_duplicada.json` | 409, código ya existe |
| `POST /currencies` | `currencies/03_crear_moneda_datos_invalidos.json` | 400, errores en Code, Name y RateToBase |
| `POST /currency/convert` | `currencies/04_convertir_usd_a_pyg.json` | 200, `convertedAmount: 610000` |
| `POST /currency/convert` | `currencies/05_convertir_pyg_a_usd.json` | 200, `convertedAmount: 100` |
| `POST /currency/convert` | `currencies/06_convertir_eur_a_usd.json` | 200, `convertedAmount: 51.23` |
| `POST /currency/convert` | `currencies/07_convertir_moneda_inexistente.json` | 404, la moneda no existe |
| `POST /currency/convert` | `currencies/08_convertir_monto_invalido.json` | 400, el monto debe ser mayor a 0 |

## Carga masiva

| Petición | Archivo | Resultado |
|---|---|---|
| `POST /users/bulk` | `bulk/bulk_users_test.json` | 200, 4 creados |
| `POST /users/bulk` | `bulk/bulk_users_test.json` (de nuevo) | 200, 0 creados y 4 fallidos (emails repetidos) |
| `POST /users/bulk` | `bulk/bulk_users_with_errors_test.json` | 200, 2 creados y 4 fallidos con su motivo |
