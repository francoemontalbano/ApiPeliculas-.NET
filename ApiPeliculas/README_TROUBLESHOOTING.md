# Solución para Error 401 Unauthorized en GetUsuario

## Problemas identificados y solucionados:

### 1. Configuración de CORS
- **Problema**: CORS estaba limitado a `http://localhost:3223`
- **Solución**: Configurado para permitir todos los orígenes en desarrollo con `AllowCredentials()`

### 2. Validación de Token JWT
- **Problema**: Falta de validación de lifetime y configuración de ClockSkew
- **Solución**: Agregado `ValidateLifetime = true` y `ClockSkew = TimeSpan.Zero`

### 3. Generación de Claims
- **Problema**: Los claims de roles no se estaban generando correctamente
- **Solución**: Mejorada la generación de claims en el método Login

## Pasos para probar:

### 1. Registrar un usuario Admin
```bash
POST /api/usuarios/registro
Content-Type: application/json

{
  "nombreUsuario": "admin",
  "nombre": "Administrador",
  "password": "123456",
  "role": "Admin"
}
```

### 2. Hacer login para obtener el token
```bash
POST /api/usuarios/login
Content-Type: application/json

{
  "nombreUsuario": "admin",
  "password": "123456"
}
```

### 3. Probar el endpoint de autenticación
```bash
GET /api/usuarios/test-auth
Authorization: Bearer {token}
```

### 4. Probar el endpoint de Admin
```bash
GET /api/usuarios/test-admin
Authorization: Bearer {token}
```

### 5. Probar el endpoint GetUsuario
```bash
GET /api/usuarios/{usuarioId}
Authorization: Bearer {token}
```

## Endpoints de prueba agregados:

- `GET /api/usuarios/test-auth` - Verifica autenticación básica
- `GET /api/usuarios/test-admin` - Verifica acceso de administrador

## Verificaciones adicionales:

1. **Verificar que el usuario existe en la base de datos con rol "Admin"**
2. **Verificar que el token se está enviando correctamente en el header Authorization**
3. **Verificar que el token no ha expirado**
4. **Verificar que el token contiene el claim de rol correcto**

## Comandos útiles para debugging:

### Verificar claims del token en jwt.io
1. Copiar el token del login
2. Ir a https://jwt.io
3. Pegar el token y verificar que contiene el claim `role: "Admin"`

### Verificar en Swagger
1. Ir a `/swagger`
2. Hacer login para obtener el token
3. Hacer clic en "Authorize" en Swagger
4. Ingresar `Bearer {token}`
5. Probar los endpoints protegidos 