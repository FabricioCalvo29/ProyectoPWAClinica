USE master;
GO

IF DB_ID('ClinicaDB') IS NOT NULL
BEGIN
    ALTER DATABASE ClinicaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ClinicaDB;
END
GO

CREATE DATABASE ClinicaDB;
GO

USE ClinicaDB;
GO

CREATE TABLE dbo.tRol
(
    IdRol INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NombreRol VARCHAR(50) NOT NULL,
    Estado BIT NOT NULL DEFAULT (1)
);
GO

ALTER TABLE dbo.tRol
ADD CONSTRAINT UQ_tRol_NombreRol UNIQUE (NombreRol);
GO

CREATE TABLE dbo.tUsuario
(
    IdUsuario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Estado BIT NOT NULL DEFAULT (1),
    FechaCreacion DATETIME NOT NULL DEFAULT (GETDATE()),
    FechaActualizacion DATETIME NOT NULL DEFAULT (GETDATE())
);
GO

CREATE TABLE dbo.tUsuarioRol
(
    IdUsuarioRol INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUsuario INT NOT NULL,
    IdRol INT NOT NULL,
    Estado BIT NOT NULL DEFAULT (1),
    FechaCreacion DATETIME NOT NULL DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.tUsuarioRol
ADD CONSTRAINT FK_tUsuarioRol_tUsuario
FOREIGN KEY (IdUsuario) REFERENCES dbo.tUsuario(IdUsuario);
GO

ALTER TABLE dbo.tUsuarioRol
ADD CONSTRAINT FK_tUsuarioRol_tRol
FOREIGN KEY (IdRol) REFERENCES dbo.tRol(IdRol);
GO

ALTER TABLE dbo.tUsuarioRol
ADD CONSTRAINT UQ_tUsuarioRol_UsuarioRol UNIQUE (IdUsuario, IdRol);
GO

CREATE TABLE dbo.tCredencialAcceso
(
    IdCredencial INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUsuario INT NOT NULL,
    Correo VARCHAR(150) NOT NULL,
    Contrasenna VARCHAR(255) NOT NULL,
    TipoAcceso VARCHAR(20) NOT NULL,
    EsPrincipal BIT NOT NULL DEFAULT (0),
    TokenRecuperacion VARCHAR(255) NULL,
    FechaVencimientoToken DATETIME NULL,
    Estado BIT NOT NULL DEFAULT (1),
    FechaCreacion DATETIME NOT NULL DEFAULT (GETDATE()),
    FechaActualizacion DATETIME NOT NULL DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.tCredencialAcceso
ADD CONSTRAINT FK_tCredencialAcceso_tUsuario
FOREIGN KEY (IdUsuario) REFERENCES dbo.tUsuario(IdUsuario);
GO

ALTER TABLE dbo.tCredencialAcceso
ADD CONSTRAINT UQ_tCredencialAcceso_Correo UNIQUE (Correo);
GO

ALTER TABLE dbo.tCredencialAcceso
ADD CONSTRAINT CHK_tCredencialAcceso_TipoAcceso
CHECK (TipoAcceso IN ('Personal','Institucional'));
GO

CREATE TABLE dbo.tEspecialidad
(
    IdEspecialidad INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(255) NULL,
    ImagenUrl VARCHAR(255) NULL,
    Estado BIT NOT NULL DEFAULT (1)
);
GO

ALTER TABLE dbo.tEspecialidad
ADD CONSTRAINT UQ_tEspecialidad_Nombre UNIQUE (Nombre);
GO

CREATE TABLE dbo.tMedico
(
    IdMedico INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUsuario INT NOT NULL,
    IdEspecialidad INT NOT NULL,
    CodigoProfesional VARCHAR(50) NOT NULL,
    HorarioAtencion VARCHAR(100) NULL,
    FotoUrl VARCHAR(255) NULL,
    CorreoProfesional VARCHAR(150) NULL,
    Estado BIT NOT NULL DEFAULT (1)
);
GO

ALTER TABLE dbo.tMedico
ADD CONSTRAINT FK_tMedico_tUsuario
FOREIGN KEY (IdUsuario) REFERENCES dbo.tUsuario(IdUsuario);
GO

ALTER TABLE dbo.tMedico
ADD CONSTRAINT FK_tMedico_tEspecialidad
FOREIGN KEY (IdEspecialidad) REFERENCES dbo.tEspecialidad(IdEspecialidad);
GO

ALTER TABLE dbo.tMedico
ADD CONSTRAINT UQ_tMedico_IdUsuario UNIQUE (IdUsuario);
GO

ALTER TABLE dbo.tMedico
ADD CONSTRAINT UQ_tMedico_CodigoProfesional UNIQUE (CodigoProfesional);
GO

CREATE TABLE dbo.tPaciente
(
    IdPaciente INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUsuario INT NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Telefono VARCHAR(15) NULL,
    TipoSangre VARCHAR(3) NULL,
    HistorialMedico VARCHAR(MAX) NULL,
    Estado BIT NOT NULL DEFAULT (1)
);
GO

ALTER TABLE dbo.tPaciente
ADD CONSTRAINT FK_tPaciente_tUsuario
FOREIGN KEY (IdUsuario) REFERENCES dbo.tUsuario(IdUsuario);
GO

ALTER TABLE dbo.tPaciente
ADD CONSTRAINT UQ_tPaciente_IdUsuario UNIQUE (IdUsuario);
GO

ALTER TABLE dbo.tPaciente
ADD CONSTRAINT CHK_tPaciente_TipoSangre
CHECK (TipoSangre IS NULL OR TipoSangre IN ('A+','A-','B+','B-','AB+','AB-','O+','O-'));
GO

CREATE TABLE dbo.tBitacoraError
(
    IdError BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdUsuario INT NULL,
    FechaHora DATETIME NOT NULL DEFAULT (GETDATE()),
    Fuente VARCHAR(100) NOT NULL,
    Mensaje VARCHAR(MAX) NOT NULL,
    Detalle VARCHAR(MAX) NULL
);
GO

ALTER TABLE dbo.tBitacoraError
ADD CONSTRAINT FK_tBitacoraError_tUsuario
FOREIGN KEY (IdUsuario) REFERENCES dbo.tUsuario(IdUsuario);
GO

CREATE TABLE dbo.tHorarioMedico
(
    IdHorario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdMedico INT NOT NULL,
    DiaSemana VARCHAR(20) NOT NULL,
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL,
    Disponible BIT NOT NULL DEFAULT (1)
);
GO

ALTER TABLE dbo.tHorarioMedico
ADD CONSTRAINT FK_tHorarioMedico_tMedico
FOREIGN KEY (IdMedico) REFERENCES dbo.tMedico(IdMedico);
GO

ALTER TABLE dbo.tHorarioMedico
ADD CONSTRAINT CHK_tHorarioMedico_DiaSemana
CHECK (DiaSemana IN ('Lunes','Martes','Miercoles','Jueves','Viernes','Sabado','Domingo'));
GO

ALTER TABLE dbo.tHorarioMedico
ADD CONSTRAINT CHK_tHorarioMedico_Horas
CHECK (HoraInicio < HoraFin);
GO

CREATE TABLE dbo.tCita
(
    IdCita INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdPaciente INT NOT NULL,
    IdMedico INT NOT NULL,
    FechaHora DATETIME NOT NULL,
    EstadoCita VARCHAR(50) NOT NULL,
    Motivo VARCHAR(255) NOT NULL,
    NotasMedico VARCHAR(MAX) NULL
);
GO

ALTER TABLE dbo.tCita
ADD CONSTRAINT FK_tCita_tPaciente
FOREIGN KEY (IdPaciente) REFERENCES dbo.tPaciente(IdPaciente);
GO

ALTER TABLE dbo.tCita
ADD CONSTRAINT FK_tCita_tMedico
FOREIGN KEY (IdMedico) REFERENCES dbo.tMedico(IdMedico);
GO

ALTER TABLE dbo.tCita
ADD CONSTRAINT CHK_tCita_EstadoCita
CHECK (EstadoCita IN ('Pendiente','Completada','Cancelada'));
GO

CREATE TABLE dbo.tTratamiento
(
    IdTratamiento INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdCita INT NOT NULL,
    Medicamento VARCHAR(100) NOT NULL,
    Dosis VARCHAR(100) NOT NULL,
    Duracion VARCHAR(100) NOT NULL,
    Instrucciones VARCHAR(MAX) NULL
);
GO

ALTER TABLE dbo.tTratamiento
ADD CONSTRAINT FK_tTratamiento_tCita
FOREIGN KEY (IdCita) REFERENCES dbo.tCita(IdCita);
GO

CREATE TABLE dbo.tFactura
(
    IdFactura INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdCita INT NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    FechaEmision DATETIME NOT NULL DEFAULT (GETDATE()),
    EstadoPago VARCHAR(50) NOT NULL,
    MetodoPago VARCHAR(50) NOT NULL
);
GO

ALTER TABLE dbo.tFactura
ADD CONSTRAINT FK_tFactura_tCita
FOREIGN KEY (IdCita) REFERENCES dbo.tCita(IdCita);
GO

ALTER TABLE dbo.tFactura
ADD CONSTRAINT UQ_tFactura_IdCita UNIQUE (IdCita);
GO

ALTER TABLE dbo.tFactura
ADD CONSTRAINT CHK_tFactura_Monto
CHECK (Monto >= 0);
GO

ALTER TABLE dbo.tFactura
ADD CONSTRAINT CHK_tFactura_EstadoPago
CHECK (EstadoPago IN ('Pagado','Pendiente','Anulado'));
GO

ALTER TABLE dbo.tFactura
ADD CONSTRAINT CHK_tFactura_MetodoPago
CHECK (MetodoPago IN ('Efectivo','Tarjeta','Transferencia'));
GO

CREATE INDEX IX_tCredencialAcceso_IdUsuario ON dbo.tCredencialAcceso(IdUsuario);
GO

CREATE INDEX IX_tUsuarioRol_IdUsuario ON dbo.tUsuarioRol(IdUsuario);
GO

CREATE INDEX IX_tUsuarioRol_IdRol ON dbo.tUsuarioRol(IdRol);
GO

CREATE INDEX IX_tMedico_IdEspecialidad ON dbo.tMedico(IdEspecialidad);
GO

CREATE INDEX IX_tCita_IdPaciente ON dbo.tCita(IdPaciente);
GO

CREATE INDEX IX_tCita_IdMedico ON dbo.tCita(IdMedico);
GO

CREATE INDEX IX_tCita_FechaHora ON dbo.tCita(FechaHora);
GO

CREATE UNIQUE INDEX UX_tMedico_CorreoProfesional
ON dbo.tMedico(CorreoProfesional)
WHERE CorreoProfesional IS NOT NULL;
GO

CREATE UNIQUE INDEX UX_tCredencialAcceso_Principal
ON dbo.tCredencialAcceso(IdUsuario)
WHERE EsPrincipal = 1;
GO

SET IDENTITY_INSERT dbo.tRol ON;
GO

INSERT INTO dbo.tRol (IdRol, NombreRol, Estado) VALUES (1, 'Administrador', 1);
INSERT INTO dbo.tRol (IdRol, NombreRol, Estado) VALUES (2, 'Paciente', 1);
INSERT INTO dbo.tRol (IdRol, NombreRol, Estado) VALUES (3, 'Medico', 1);
GO

SET IDENTITY_INSERT dbo.tRol OFF;
GO

CREATE OR ALTER PROCEDURE dbo.RegistrarError
    @IdUsuario INT = NULL,
    @Fuente VARCHAR(100),
    @Mensaje VARCHAR(MAX),
    @Detalle VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tBitacoraError (IdUsuario, FechaHora, Fuente, Mensaje, Detalle)
    VALUES (@IdUsuario, GETDATE(), @Fuente, @Mensaje, @Detalle);
END
GO

CREATE OR ALTER PROCEDURE dbo.ValidarCorreo
    @Correo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdCredencial, IdUsuario, Correo, TipoAcceso, Estado
    FROM dbo.tCredencialAcceso
    WHERE Correo = @Correo;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdUsuario, Nombre, Apellido, Estado, FechaCreacion, FechaActualizacion
    FROM dbo.tUsuario
    WHERE IdUsuario = @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarCredencialesUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdCredencial, IdUsuario, Correo, TipoAcceso, EsPrincipal, Estado, FechaCreacion, FechaActualizacion
    FROM dbo.tCredencialAcceso
    WHERE IdUsuario = @IdUsuario
    ORDER BY EsPrincipal DESC, Correo ASC;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarRolesUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UR.IdUsuarioRol, UR.IdUsuario, R.IdRol, R.NombreRol, UR.Estado
    FROM dbo.tUsuarioRol UR
    INNER JOIN dbo.tRol R ON UR.IdRol = R.IdRol
    WHERE UR.IdUsuario = @IdUsuario
      AND UR.Estado = 1
      AND R.Estado = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.RegistrarUsuario
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tUsuario (Nombre, Apellido, Estado, FechaCreacion, FechaActualizacion)
    VALUES (@Nombre, @Apellido, 1, GETDATE(), GETDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE dbo.AsignarRolUsuario
    @IdUsuario INT,
    @IdRol INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.tUsuario WHERE IdUsuario = @IdUsuario AND Estado = 1)
    BEGIN
        RAISERROR('El usuario no existe o esta inactivo.',16,1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.tRol WHERE IdRol = @IdRol AND Estado = 1)
    BEGIN
        RAISERROR('El rol no existe o esta inactivo.',16,1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.tUsuarioRol WHERE IdUsuario = @IdUsuario AND IdRol = @IdRol)
    BEGIN
        RAISERROR('El usuario ya tiene asignado ese rol.',16,1);
        RETURN;
    END

    INSERT INTO dbo.tUsuarioRol (IdUsuario, IdRol, Estado, FechaCreacion)
    VALUES (@IdUsuario, @IdRol, 1, GETDATE());
END
GO

CREATE OR ALTER PROCEDURE dbo.RegistrarCredencialAcceso
    @IdUsuario INT,
    @Correo VARCHAR(150),
    @Contrasenna VARCHAR(255),
    @TipoAcceso VARCHAR(20),
    @EsPrincipal BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.tCredencialAcceso WHERE Correo = @Correo)
    BEGIN
        RAISERROR('El correo ya se encuentra registrado.',16,1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.tUsuario WHERE IdUsuario = @IdUsuario AND Estado = 1)
    BEGIN
        RAISERROR('El usuario no existe o esta inactivo.',16,1);
        RETURN;
    END

    IF @TipoAcceso NOT IN ('Personal','Institucional')
    BEGIN
        RAISERROR('Tipo de acceso invalido.',16,1);
        RETURN;
    END

    IF @EsPrincipal = 1
    BEGIN
        UPDATE dbo.tCredencialAcceso
        SET EsPrincipal = 0,
            FechaActualizacion = GETDATE()
        WHERE IdUsuario = @IdUsuario;
    END

    INSERT INTO dbo.tCredencialAcceso
    (
        IdUsuario,
        Correo,
        Contrasenna,
        TipoAcceso,
        EsPrincipal,
        TokenRecuperacion,
        FechaVencimientoToken,
        Estado,
        FechaCreacion,
        FechaActualizacion
    )
    VALUES
    (
        @IdUsuario,
        @Correo,
        @Contrasenna,
        @TipoAcceso,
        @EsPrincipal,
        NULL,
        NULL,
        1,
        GETDATE(),
        GETDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdCredencial;
END
GO

CREATE OR ALTER PROCEDURE dbo.RegistrarPacienteDesdeLogin
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Correo VARCHAR(150),
    @Contrasenna VARCHAR(255),
    @FechaNacimiento DATE,
    @Telefono VARCHAR(15) = NULL,
    @TipoSangre VARCHAR(3) = NULL,
    @HistorialMedico VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @IdUsuario INT;

        IF EXISTS (SELECT 1 FROM dbo.tCredencialAcceso WHERE Correo = @Correo)
        BEGIN
            RAISERROR('El correo ya se encuentra registrado.',16,1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        INSERT INTO dbo.tUsuario (Nombre, Apellido, Estado, FechaCreacion, FechaActualizacion)
        VALUES (@Nombre, @Apellido, 1, GETDATE(), GETDATE());

        SET @IdUsuario = CAST(SCOPE_IDENTITY() AS INT);

        INSERT INTO dbo.tUsuarioRol (IdUsuario, IdRol, Estado, FechaCreacion)
        VALUES (@IdUsuario, 2, 1, GETDATE());

        INSERT INTO dbo.tCredencialAcceso
        (
            IdUsuario,
            Correo,
            Contrasenna,
            TipoAcceso,
            EsPrincipal,
            TokenRecuperacion,
            FechaVencimientoToken,
            Estado,
            FechaCreacion,
            FechaActualizacion
        )
        VALUES
        (
            @IdUsuario,
            @Correo,
            @Contrasenna,
            'Personal',
            1,
            NULL,
            NULL,
            1,
            GETDATE(),
            GETDATE()
        );

        INSERT INTO dbo.tPaciente
        (
            IdUsuario,
            FechaNacimiento,
            Telefono,
            TipoSangre,
            HistorialMedico,
            Estado
        )
        VALUES
        (
            @IdUsuario,
            @FechaNacimiento,
            @Telefono,
            @TipoSangre,
            @HistorialMedico,
            1
        );

        COMMIT TRANSACTION;

        SELECT @IdUsuario AS IdUsuario;
    END TRY
    BEGIN CATCH
        DECLARE @MensajeError VARCHAR(MAX);
        DECLARE @DetalleError VARCHAR(MAX);

        SET @MensajeError = ERROR_MESSAGE();
        SET @DetalleError = 'Procedimiento: ' + ISNULL(ERROR_PROCEDURE(),'N/A')
                          + ' | Linea: ' + CAST(ERROR_LINE() AS VARCHAR(20));

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        EXEC dbo.RegistrarError NULL, 'SP_RegistrarPacienteDesdeLogin', @MensajeError, @DetalleError;

        RAISERROR(@MensajeError,16,1);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.RegistrarUsuarioInterno
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Correo VARCHAR(150),
    @Contrasenna VARCHAR(255),
    @IdRol INT,
    @TipoAcceso VARCHAR(20),
    @EsPrincipal BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @IdUsuario INT;

        IF EXISTS (SELECT 1 FROM dbo.tCredencialAcceso WHERE Correo = @Correo)
        BEGIN
            RAISERROR('El correo ya se encuentra registrado.',16,1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF @IdRol NOT IN (1,3)
        BEGIN
            RAISERROR('Este procedimiento esta pensado para Administrador o Medico.',16,1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF @TipoAcceso NOT IN ('Personal','Institucional')
        BEGIN
            RAISERROR('Tipo de acceso invalido.',16,1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        INSERT INTO dbo.tUsuario (Nombre, Apellido, Estado, FechaCreacion, FechaActualizacion)
        VALUES (@Nombre, @Apellido, 1, GETDATE(), GETDATE());

        SET @IdUsuario = CAST(SCOPE_IDENTITY() AS INT);

        INSERT INTO dbo.tUsuarioRol (IdUsuario, IdRol, Estado, FechaCreacion)
        VALUES (@IdUsuario, @IdRol, 1, GETDATE());

        INSERT INTO dbo.tCredencialAcceso
        (
            IdUsuario,
            Correo,
            Contrasenna,
            TipoAcceso,
            EsPrincipal,
            TokenRecuperacion,
            FechaVencimientoToken,
            Estado,
            FechaCreacion,
            FechaActualizacion
        )
        VALUES
        (
            @IdUsuario,
            @Correo,
            @Contrasenna,
            @TipoAcceso,
            @EsPrincipal,
            NULL,
            NULL,
            1,
            GETDATE(),
            GETDATE()
        );

        COMMIT TRANSACTION;

        SELECT @IdUsuario AS IdUsuario;
    END TRY
    BEGIN CATCH
        DECLARE @MensajeError VARCHAR(MAX);
        DECLARE @DetalleError VARCHAR(MAX);

        SET @MensajeError = ERROR_MESSAGE();
        SET @DetalleError = 'Procedimiento: ' + ISNULL(ERROR_PROCEDURE(),'N/A')
                          + ' | Linea: ' + CAST(ERROR_LINE() AS VARCHAR(20));

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        EXEC dbo.RegistrarError NULL, 'SP_RegistrarUsuarioInterno', @MensajeError, @DetalleError;

        RAISERROR(@MensajeError,16,1);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.IniciarSesion
    @Correo VARCHAR(150),
    @Contrasenna VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        U.IdUsuario,
        U.Nombre,
        U.Apellido,
        CA.IdCredencial,
        CA.Correo,
        CA.TipoAcceso,
        CA.EsPrincipal,
        U.Estado AS EstadoUsuario,
        CA.Estado AS EstadoCredencial
    FROM dbo.tCredencialAcceso CA
    INNER JOIN dbo.tUsuario U ON CA.IdUsuario = U.IdUsuario
    WHERE CA.Correo = @Correo
      AND CA.Contrasenna = @Contrasenna
      AND U.Estado = 1
      AND CA.Estado = 1;

    SELECT
        R.IdRol,
        R.NombreRol
    FROM dbo.tCredencialAcceso CA
    INNER JOIN dbo.tUsuario U ON CA.IdUsuario = U.IdUsuario
    INNER JOIN dbo.tUsuarioRol UR ON U.IdUsuario = UR.IdUsuario
    INNER JOIN dbo.tRol R ON UR.IdRol = R.IdRol
    WHERE CA.Correo = @Correo
      AND CA.Contrasenna = @Contrasenna
      AND U.Estado = 1
      AND CA.Estado = 1
      AND UR.Estado = 1
      AND R.Estado = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.IniciarSesionPorRol
    @Correo VARCHAR(150),
    @Contrasenna VARCHAR(255),
    @IdRol INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        U.IdUsuario,
        U.Nombre,
        U.Apellido,
        CA.IdCredencial,
        CA.Correo,
        CA.TipoAcceso,
        R.IdRol,
        R.NombreRol
    FROM dbo.tCredencialAcceso CA
    INNER JOIN dbo.tUsuario U ON CA.IdUsuario = U.IdUsuario
    INNER JOIN dbo.tUsuarioRol UR ON U.IdUsuario = UR.IdUsuario
    INNER JOIN dbo.tRol R ON UR.IdRol = R.IdRol
    WHERE CA.Correo = @Correo
      AND CA.Contrasenna = @Contrasenna
      AND R.IdRol = @IdRol
      AND U.Estado = 1
      AND CA.Estado = 1
      AND UR.Estado = 1
      AND R.Estado = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.IniciarSesionInstitucional
    @Correo VARCHAR(150),
    @Contrasenna VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        U.IdUsuario,
        U.Nombre,
        U.Apellido,
        CA.IdCredencial,
        CA.Correo,
        CA.TipoAcceso
    FROM dbo.tCredencialAcceso CA
    INNER JOIN dbo.tUsuario U ON CA.IdUsuario = U.IdUsuario
    WHERE CA.Correo = @Correo
      AND CA.Contrasenna = @Contrasenna
      AND CA.TipoAcceso = 'Institucional'
      AND U.Estado = 1
      AND CA.Estado = 1;

    SELECT
        R.IdRol,
        R.NombreRol
    FROM dbo.tCredencialAcceso CA
    INNER JOIN dbo.tUsuario U ON CA.IdUsuario = U.IdUsuario
    INNER JOIN dbo.tUsuarioRol UR ON U.IdUsuario = UR.IdUsuario
    INNER JOIN dbo.tRol R ON UR.IdRol = R.IdRol
    WHERE CA.Correo = @Correo
      AND CA.Contrasenna = @Contrasenna
      AND CA.TipoAcceso = 'Institucional'
      AND U.Estado = 1
      AND CA.Estado = 1
      AND UR.Estado = 1
      AND R.Estado = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.ActualizarPerfilUsuario
    @IdUsuario INT,
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tUsuario
    SET Nombre = @Nombre,
        Apellido = @Apellido,
        FechaActualizacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE dbo.ActualizarCorreoCredencial
    @IdCredencial INT,
    @Correo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.tCredencialAcceso
        WHERE Correo = @Correo
          AND IdCredencial <> @IdCredencial
    )
    BEGIN
        RAISERROR('El correo ya pertenece a otra credencial.',16,1);
        RETURN;
    END

    UPDATE dbo.tCredencialAcceso
    SET Correo = @Correo,
        FechaActualizacion = GETDATE()
    WHERE IdCredencial = @IdCredencial;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GuardarTokenRecuperacion]
    @Correo VARCHAR(150),
    @TokenRecuperacion VARCHAR(255),
    @FechaVencimientoToken DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tCredencialAcceso
    SET TokenRecuperacion = @TokenRecuperacion,
        FechaVencimientoToken = @FechaVencimientoToken,
        FechaActualizacion = GETDATE()
    WHERE Correo = @Correo
      AND Estado = 1;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('No se encontro  una credencial activa para el correo indicado.', 16, 1);
        RETURN;
    END
END
GO


CREATE OR ALTER PROCEDURE dbo.ValidarTokenRecuperacion
    @TokenRecuperacion VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdCredencial, IdUsuario, Correo, TokenRecuperacion, FechaVencimientoToken
    FROM dbo.tCredencialAcceso
    WHERE TokenRecuperacion = @TokenRecuperacion
      AND FechaVencimientoToken >= GETDATE()
      AND Estado = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.ActualizarContrasenna
    @IdCredencial INT,
    @Contrasenna VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tCredencialAcceso
    SET Contrasenna = @Contrasenna,
        TokenRecuperacion = NULL,
        FechaVencimientoToken = NULL,
        FechaActualizacion = GETDATE()
    WHERE IdCredencial = @IdCredencial
      AND Estado = 1;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('No se encontro  una credencial activa para actualizar.', 16, 1);
        RETURN;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.CambiarEstadoUsuario
    @IdUsuario INT,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tUsuario
    SET Estado = @Estado,
        FechaActualizacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE dbo.CambiarEstadoCredencial
    @IdCredencial INT,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tCredencialAcceso
    SET Estado = @Estado,
        FechaActualizacion = GETDATE()
    WHERE IdCredencial = @IdCredencial;
END
GO

CREATE OR ALTER PROCEDURE dbo.CambiarCredencialPrincipal
    @IdUsuario INT,
    @IdCredencial INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tCredencialAcceso
        WHERE IdCredencial = @IdCredencial
          AND IdUsuario = @IdUsuario
    )
    BEGIN
        RAISERROR('La credencial no pertenece al usuario indicado.',16,1);
        RETURN;
    END

    UPDATE dbo.tCredencialAcceso
    SET EsPrincipal = 0,
        FechaActualizacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;

    UPDATE dbo.tCredencialAcceso
    SET EsPrincipal = 1,
        FechaActualizacion = GETDATE()
    WHERE IdCredencial = @IdCredencial
      AND IdUsuario = @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarMedicosActivosParaCita
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        M.IdMedico,
        CONCAT(U.Nombre, ' ', U.Apellido) AS NombreMedico,
        ISNULL(E.Nombre, 'Sin especialidad') AS Especialidad
    FROM dbo.tMedico M
    INNER JOIN dbo.tUsuario U ON M.IdUsuario = U.IdUsuario
    LEFT JOIN dbo.tEspecialidad E ON M.IdEspecialidad = E.IdEspecialidad
    WHERE M.Estado = 1
      AND U.Estado = 1
    ORDER BY U.Nombre, U.Apellido;
END
GO

CREATE OR ALTER PROCEDURE dbo.RegistrarCitaPaciente
    @IdUsuario INT,
    @IdMedico INT,
    @FechaHora DATETIME,
    @Motivo VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @IdPaciente INT;

        SELECT @IdPaciente = P.IdPaciente
        FROM dbo.tPaciente P
        INNER JOIN dbo.tUsuario U ON P.IdUsuario = U.IdUsuario
        WHERE P.IdUsuario = @IdUsuario
          AND P.Estado = 1
          AND U.Estado = 1;

        IF @IdPaciente IS NULL
        BEGIN
            RAISERROR('No existe un paciente activo para el usuario indicado.',16,1);
            RETURN;
        END

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.tMedico M
            INNER JOIN dbo.tUsuario U ON M.IdUsuario = U.IdUsuario
            WHERE M.IdMedico = @IdMedico
              AND M.Estado = 1
              AND U.Estado = 1
        )
        BEGIN
            RAISERROR('El medico seleccionado no se encuentra disponible.',16,1);
            RETURN;
        END

        IF @FechaHora <= GETDATE()
        BEGIN
            RAISERROR('La fecha y hora de la cita debe ser futura.',16,1);
            RETURN;
        END

        IF EXISTS
        (
            SELECT 1
            FROM dbo.tCita
            WHERE IdMedico = @IdMedico
              AND FechaHora = @FechaHora
              AND EstadoCita = 'Pendiente'
        )
        BEGIN
            RAISERROR('El medico ya tiene una cita pendiente en la fecha y hora indicada.',16,1);
            RETURN;
        END

        IF EXISTS
        (
            SELECT 1
            FROM dbo.tCita
            WHERE IdPaciente = @IdPaciente
              AND FechaHora = @FechaHora
              AND EstadoCita = 'Pendiente'
        )
        BEGIN
            RAISERROR('El paciente ya tiene una cita pendiente en la fecha y hora indicada.',16,1);
            RETURN;
        END

        INSERT INTO dbo.tCita
        (
            IdPaciente,
            IdMedico,
            FechaHora,
            EstadoCita,
            Motivo,
            NotasMedico
        )
        VALUES
        (
            @IdPaciente,
            @IdMedico,
            @FechaHora,
            'Pendiente',
            @Motivo,
            NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdCita;
    END TRY
    BEGIN CATCH
        DECLARE @MensajeError VARCHAR(MAX);
        DECLARE @DetalleError VARCHAR(MAX);

        SET @MensajeError = ERROR_MESSAGE();
        SET @DetalleError = 'Procedimiento: ' + ISNULL(ERROR_PROCEDURE(),'N/A')
                          + ' | Linea: ' + CAST(ERROR_LINE() AS VARCHAR(20));

        EXEC dbo.RegistrarError @IdUsuario, 'SP_RegistrarCitaPaciente', @MensajeError, @DetalleError;

        RAISERROR(@MensajeError,16,1);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarCitasPaciente
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.IdCita,
        C.IdPaciente,
        C.IdMedico,
        C.FechaHora,
        C.EstadoCita,
        C.Motivo,
        C.NotasMedico,
        CONCAT(UM.Nombre, ' ', UM.Apellido) AS NombreMedico,
        E.Nombre AS Especialidad
    FROM dbo.tCita C
    INNER JOIN dbo.tPaciente P ON C.IdPaciente = P.IdPaciente
    INNER JOIN dbo.tMedico M ON C.IdMedico = M.IdMedico
    INNER JOIN dbo.tUsuario UM ON M.IdUsuario = UM.IdUsuario
    INNER JOIN dbo.tEspecialidad E ON M.IdEspecialidad = E.IdEspecialidad
    WHERE P.IdUsuario = @IdUsuario
      AND P.Estado = 1
    ORDER BY C.FechaHora DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarDetalleCitaPaciente
    @IdUsuario INT,
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        C.IdCita,
        C.IdPaciente,
        C.IdMedico,
        C.FechaHora,
        C.EstadoCita,
        C.Motivo,
        C.NotasMedico,
        CONCAT(UM.Nombre, ' ', UM.Apellido) AS NombreMedico,
        E.Nombre AS Especialidad
    FROM dbo.tCita C
    INNER JOIN dbo.tPaciente P ON C.IdPaciente = P.IdPaciente
    INNER JOIN dbo.tMedico M ON C.IdMedico = M.IdMedico
    INNER JOIN dbo.tUsuario UM ON M.IdUsuario = UM.IdUsuario
    INNER JOIN dbo.tEspecialidad E ON M.IdEspecialidad = E.IdEspecialidad
    WHERE C.IdCita = @IdCita
      AND P.IdUsuario = @IdUsuario
      AND P.Estado = 1;
END
GO

USE ClinicaDB;
GO
SELECT * FROM dbo.tRol;
GO

EXEC dbo.RegistrarPacienteDesdeLogin
    @Nombre = 'Fabricio',
    @Apellido = 'Calvo',
    @Correo = 'fabricio@gmail.com',
    @Contrasenna = 'HASH123',
    @FechaNacimiento = '2002-07-29',
    @Telefono = '88888888',
    @TipoSangre = 'O+',
    @HistorialMedico = 'Sin antecedentes';
GO

EXEC dbo.IniciarSesion
    @Correo = 'fabricio@gmail.com',
    @Contrasenna = 'HASH123';
GO

SELECT * FROM tCredencialAcceso;

-- =============================================
-- FASE 1: NUEVOS PROCEDIMIENTOS ALMACENADOS
-- =============================================

GO

CREATE OR ALTER PROCEDURE dbo.CancelarCitaPaciente
    @IdUsuario INT,
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdPaciente INT;
    SELECT @IdPaciente = IdPaciente FROM tPaciente WHERE IdUsuario = @IdUsuario;

    IF EXISTS (SELECT 1 FROM tCitas WHERE IdCita = @IdCita AND IdPaciente = @IdPaciente AND EstadoCita = 'Pendiente')
    BEGIN
        UPDATE tCitas SET EstadoCita = 'Cancelada' WHERE IdCita = @IdCita;
    END
    ELSE
    BEGIN
        THROW 50001, 'La cita no existe, ya fue cancelada, o no te pertenece.', 1;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.CompletarCita
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE tCitas SET EstadoCita = 'Completada' WHERE IdCita = @IdCita;
END
GO

CREATE OR ALTER PROCEDURE dbo.RegistrarTratamiento
    @IdCita INT,
    @Medicamento NVARCHAR(100),
    @Dosis NVARCHAR(100),
    @Duracion NVARCHAR(100),
    @Instrucciones NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO tTratamiento (IdCita, Medicamento, Dosis, Duracion, Instrucciones)
    VALUES (@IdCita, @Medicamento, @Dosis, @Duracion, @Instrucciones);
    
    SELECT SCOPE_IDENTITY() as IdGenerado;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarTratamientosPorCita
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdTratamiento, IdCita, Medicamento, Dosis, Duracion, Instrucciones
    FROM tTratamiento
    WHERE IdCita = @IdCita;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarTratamientosPaciente
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdPaciente INT;
    SELECT @IdPaciente = IdPaciente FROM tPaciente WHERE IdUsuario = @IdUsuario;

    SELECT t.IdTratamiento, t.IdCita, t.Medicamento, t.Dosis, t.Duracion, t.Instrucciones
    FROM tTratamiento t
    INNER JOIN tCitas c ON t.IdCita = c.IdCita
    WHERE c.IdPaciente = @IdPaciente
    ORDER BY c.FechaHora DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarEstadisticasAdmin
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        (SELECT COUNT(*) FROM tUsuario) as TotalUsuarios,
        (SELECT COUNT(*) FROM tCitas WHERE EstadoCita = 'Pendiente') as CitasPendientes,
        (SELECT COUNT(*) FROM tCitas WHERE EstadoCita = 'Completada') as CitasCompletadas,
        (SELECT COUNT(*) FROM tCitas WHERE CAST(FechaHora as DATE) = CAST(GETDATE() as DATE)) as CitasHoy
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarEstadisticasPaciente
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @IdPaciente INT;
    SELECT @IdPaciente = IdPaciente FROM tPaciente WHERE IdUsuario = @IdUsuario;

    SELECT 
        (SELECT COUNT(*) FROM tCitas WHERE IdPaciente = @IdPaciente AND EstadoCita = 'Pendiente') as CitasPendientes,
        (SELECT COUNT(*) FROM tCitas WHERE IdPaciente = @IdPaciente AND EstadoCita = 'Completada') as CitasCompletadas,
        (SELECT COUNT(*) FROM tCitas WHERE IdPaciente = @IdPaciente AND EstadoCita = 'Cancelada') as CitasCanceladas,
        (SELECT COUNT(*) FROM tTratamiento t INNER JOIN tCitas c ON t.IdCita = c.IdCita WHERE c.IdPaciente = @IdPaciente) as TotalTratamientos
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarTodosLosPacientes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.IdPaciente, 
        u.IdUsuario, 
        u.Nombre + ' ' + u.Apellido as NombreCompleto, 
        c.Correo, 
        u.Telefono, 
        u.TipoSangre, 
        u.EstadoUsuario as Estado
    FROM tPaciente p
    INNER JOIN tUsuario u ON p.IdUsuario = u.IdUsuario
    INNER JOIN tCredenciales c ON c.IdUsuario = u.IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarDetallePaciente
    @IdPaciente INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.IdPaciente, 
        u.IdUsuario, 
        u.Nombre + ' ' + u.Apellido as NombreCompleto, 
        c.Correo, 
        u.Telefono, 
        u.TipoSangre, 
        u.EstadoUsuario as Estado
    FROM tPaciente p
    INNER JOIN tUsuario u ON p.IdUsuario = u.IdUsuario
    INNER JOIN tCredenciales c ON c.IdUsuario = u.IdUsuario
    WHERE p.IdPaciente = @IdPaciente;
END
GO

CREATE OR ALTER PROCEDURE dbo.ConsultarTodasLasCitas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.IdCita,
        c.IdPaciente,
        c.IdMedico,
        c.FechaHora,
        c.EstadoCita,
        c.Motivo,
        c.NotasMedico,
        (u.Nombre + ' ' + u.Apellido) as NombreMedico,
        m.Especialidad
    FROM tCitas c
    INNER JOIN tMedico m ON c.IdMedico = m.IdMedico
    INNER JOIN tUsuario u ON m.IdUsuario = u.IdUsuario
    ORDER BY c.FechaHora DESC;
END
GO
