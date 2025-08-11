# Solución de Problemas de CORS

## 🚨 **Error de CORS - Soluciones**

### **Problema 1: CORS bloqueando peticiones desde el navegador**

**Síntoma:**
```
Access to fetch at 'http://localhost:5191/api/usuarios/login' from origin 'http://localhost:3000' has been blocked by CORS policy
```

**Solución 1: Configuración simple (recomendada)**
```csharp
// En Program.cs
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
}));
```

**Solución 2: Si necesitas AllowCredentials**
```csharp
// En Program.cs
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.SetIsOriginAllowed(origin => true)
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials();
}));
```

### **Problema 2: Orden incorrecto de middleware**

**Síntoma:** CORS no funciona aunque esté configurado

**Solución:** Verificar el orden en Program.cs
```csharp
var app = builder.Build();

// ✅ ORDEN CORRECTO
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("PoliticaCors"); // ✅ ANTES de Authentication
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

### **Problema 3: Configuración específica por puerto**

**Síntoma:** CORS funciona en un puerto pero no en otro

**Solución:**
```csharp
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.WithOrigins(
        "http://localhost:3000",
        "http://localhost:8080", 
        "http://localhost:5191",
        "https://localhost:7119"
    )
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
}));
```

## 🔧 **Pasos para verificar**

### **Paso 1: Verificar que CORS está habilitado**
1. Abre las herramientas de desarrollador (F12)
2. Ve a la pestaña Network
3. Haz una petición a tu API
4. Verifica que aparezca el header `Access-Control-Allow-Origin`

### **Paso 2: Verificar desde Postman**
1. Abre Postman
2. Haz una petición GET a `http://localhost:5191/api/usuarios`
3. Si funciona en Postman pero no en el navegador, es problema de CORS

### **Paso 3: Verificar desde curl**
```bash
curl -X GET http://localhost:5191/api/usuarios
```

## 🚀 **Configuración recomendada para desarrollo**

```csharp
// En Program.cs - Configuración para desarrollo
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    if (builder.Environment.IsDevelopment())
    {
        // En desarrollo: permite todo
        build.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader();
    }
    else
    {
        // En producción: configuración específica
        build.WithOrigins("https://tu-dominio.com")
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials();
    }
}));
```

## 📋 **Checklist de verificación**

- [ ] CORS está configurado en `builder.Services.AddCors()`
- [ ] `app.UseCors("PoliticaCors")` está antes de `app.UseAuthentication()`
- [ ] La aplicación está corriendo en el puerto correcto
- [ ] No hay errores en la consola de la aplicación
- [ ] Las peticiones funcionan en Postman/curl

## 🔍 **Debugging avanzado**

### **Agregar logging de CORS**
```csharp
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
}));

// Agregar logging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request from: {context.Request.Headers["Origin"]}");
    await next();
});
```

### **Verificar headers de respuesta**
```csharp
app.Use(async (context, next) =>
{
    await next();
    Console.WriteLine($"CORS Headers: {string.Join(", ", context.Response.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
});
``` 