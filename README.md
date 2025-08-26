# API de Películas con ASP.NET

Este proyecto consiste en una API RESTful para gestionar películas, implementada utilizando las siguientes tecnologías:

- **ASP.NET 8**
- **ASP.NET Identity** para autenticación y autorización
- **JWT (JSON Web Token)** para gestión de sesiones seguras
- **Repository Pattern** para un acceso a datos limpio y desacoplado
- **Entity Framework Core** para el acceso a la base de datos
- **SQL Server** como sistema de gestión de bases de datos
- Desplegada en **Azure App Service** para la ejecución en la nube

## Despliegue en Azure App Service

El proyecto está desplegado en **Azure App Service**. Puedes acceder a la API desde la siguiente URL:

[**URL de la API desplegada en Azure**](https://apipeliculasnet8azdeploy-b0ewe6fpdydhapb4.chilecentral-01.azurewebsites.net/index.html)

## Descripción del Proyecto

La API permite realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) sobre una base de datos de películas. Los usuarios pueden autenticarse utilizando **ASP.NET Identity** y **JWT** para realizar peticiones protegidas. La estructura del proyecto sigue el **Repository Pattern**, lo que garantiza un acceso a los datos limpio y escalable.

### Funcionalidades

- **Autenticación de usuarios** mediante **ASP.NET Identity** y **JWT**.
- **Operaciones CRUD** sobre la entidad "Película".
- **Autorización** para acceder a operaciones de modificación de películas.
- **Swagger UI** para facilitar las pruebas de la API.
- Manejo de versiones.

## Tecnologías Utilizadas

- **ASP.NET 8**: Framework principal para la construcción de la API.
- **ASP.NET Identity**: Gestión de usuarios, autenticación y autorización.
- **JWT**: Utilizado para la generación de tokens de autenticación.
- **Repository Pattern**: Patron de diseño para la gestión del acceso a datos.
- **Entity Framework Core**: ORM utilizado para la interacción con SQL Server.
- **SQL Server**: Base de datos para almacenar la información de las películas.
- **Azure App Service**: Despliegue y ejecución en la nube de la API.
