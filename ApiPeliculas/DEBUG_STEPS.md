# Pasos para Debugging del Error 401 Unauthorized

## 🔍 **Paso 1: Verificar que tienes un usuario Admin en la base de datos**

Ejecuta esta consulta SQL en tu base de datos:

```sql
SELECT Id, NombreUsuario, Nombre, Role FROM Usuario WHERE Role = 'Admin';
```

**Resultado esperado**: Debe mostrar al menos un usuario con `Role = 'Admin'`

## 🔍 **Paso 2: Hacer login para obtener el token**

```bash
POST /api/usuarios/login
Content-Type: application/json

{
  "nombreUsuario": "tu_usuario_admin",
  "password": "tu_password"
}
```

**Guarda el token** de la respuesta para usarlo en los siguientes pasos.

## 🔍 **Paso 3: Verificar el token con el endpoint de debugging**

```bash
POST /api/usuarios/verify-token
Content-Type: application/json

{
  "token": "tu_token_aqui"
}
```

**Verifica que**:
- `IsValid` sea `true`
- En `Claims` aparezca un claim con `Type: "role"` y `Value: "Admin"`
- `ExpiresAt` no haya pasado

## 🔍 **Paso 4: Probar el endpoint de debugging de autenticación**

```bash
GET /api/usuarios/debug-auth
Authorization: Bearer tu_token_aqui
```

**Verifica que**:
- `IsAuthenticated` sea `true`
- `IsInRoleAdmin` sea `true`
- `RoleClaims` contenga `"Admin"`

## 🔍 **Paso 5: Probar el endpoint de Admin**

```bash
GET /api/usuarios/test-admin
Authorization: Bearer tu_token_aqui
```

**Resultado esperado**: Debe devolver 200 OK con el mensaje "Acceso de administrador exitoso"

## 🔍 **Paso 6: Probar el endpoint original que falla**

```bash
GET /api/usuarios/1
Authorization: Bearer tu_token_aqui
```

## 🚨 **Posibles problemas y soluciones**

### **Problema 1: Token no válido**
- **Síntoma**: `IsValid: false` en verify-token
- **Solución**: Verificar que el token no haya expirado y que la clave secreta sea la misma

### **Problema 2: Token no contiene el rol**
- **Síntoma**: En verify-token no aparece el claim de role
- **Solución**: Verificar que el usuario en la BD tenga `Role = 'Admin'`

### **Problema 3: CORS bloqueando la petición**
- **Síntoma**: Error de CORS en el navegador
- **Solución**: Verificar que estés usando el puerto correcto

### **Problema 4: Header Authorization mal formado**
- **Síntoma**: 401 Unauthorized inmediato
- **Solución**: Verificar que el header sea `Authorization: Bearer token`

## 📋 **Checklist de verificación**

- [ ] Usuario existe en BD con `Role = 'Admin'`
- [ ] Login devuelve un token válido
- [ ] Token contiene el claim `role: "Admin"`
- [ ] Token no ha expirado
- [ ] Header Authorization está bien formado
- [ ] No hay errores de CORS
- [ ] La aplicación está corriendo en el puerto correcto

## 🔧 **Comandos útiles para testing**

### **Con curl (Windows PowerShell)**
```powershell
# Login
$loginResponse = Invoke-RestMethod -Uri "http://localhost:5191/api/usuarios/login" -Method POST -ContentType "application/json" -Body '{"nombreUsuario":"admin","password":"123456"}'
$token = $loginResponse.result.token

# Verificar token
$verifyResponse = Invoke-RestMethod -Uri "http://localhost:5191/api/usuarios/verify-token" -Method POST -ContentType "application/json" -Body "{\"token\":\"$token\"}"

# Probar endpoint
$headers = @{Authorization = "Bearer $token"}
$testResponse = Invoke-RestMethod -Uri "http://localhost:5191/api/usuarios/debug-auth" -Method GET -Headers $headers
```

### **Con Postman**
1. Crear una nueva colección
2. Agregar request para login
3. Guardar el token de la respuesta
4. Usar el token en el header Authorization para los otros requests

## 📞 **Si sigues teniendo problemas**

1. Ejecuta todos los pasos de debugging
2. Anota los resultados de cada paso
3. Comparte los resultados para identificar el problema específico 