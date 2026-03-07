CREATE DATABASE PrograG7;
USE PrograG7;

CREATE TABLE BitacoraEventos (
    IdEvento int NOT NULL AUTO_INCREMENT,
    TablaDeEvento varchar(20) CHARACTER SET utf8mb4,
    TipoDeEvento varchar(20) CHARACTER SET utf8mb4,
    FechaDeEvento datetime(6),
    DescripcionDeEvento longtext CHARACTER SET utf8mb4,
    StackTrace longtext CHARACTER SET utf8mb4,
    DatosAnteriores longtext CHARACTER SET utf8mb4,
    DatosPosteriores longtext CHARACTER SET utf8mb4,
    CONSTRAINT PK_BitacoraEventos PRIMARY KEY (IdEvento)
) CHARACTER SET=utf8mb4;

CREATE TABLE Comercios (
    IdComercio int NOT NULL AUTO_INCREMENT,
    Identificacion varchar(30) CHARACTER SET utf8mb4,
    TipoIdentificacion int,
    Nombre varchar(200) CHARACTER SET utf8mb4,
    TipoDeComercio int,
    Telefono varchar(20) CHARACTER SET utf8mb4,
    CorreoElectronico varchar(200) CHARACTER SET utf8mb4,
    Direccion varchar(500) CHARACTER SET utf8mb4,
    FechaDeRegistro datetime(6),
    FechaDeModificacion datetime(6),
    Estado tinyint(1),
    CONSTRAINT PK_Comercios PRIMARY KEY (IdComercio)
) CHARACTER SET=utf8mb4;

CREATE TABLE Cajas (
    IdCaja int NOT NULL AUTO_INCREMENT,
    IdComercio int,
    Nombre varchar(200) CHARACTER SET utf8mb4,
    Codigo varchar(50) CHARACTER SET utf8mb4,
    Descripcion varchar(150) CHARACTER SET utf8mb4,
    TelefonoSINPE varchar(10) CHARACTER SET utf8mb4,
    Estado tinyint(1),
    FechaDeRegistro datetime(6),
    FechaDeModificacion datetime(6),
    UsuarioRegistro varchar(100) CHARACTER SET utf8mb4,
    UsuarioModificacion varchar(100) CHARACTER SET utf8mb4,
    CONSTRAINT PK_Cajas PRIMARY KEY (IdCaja),
    CONSTRAINT FK_Cajas_Comercios_IdComercio FOREIGN KEY (IdComercio) 
    REFERENCES Comercios (IdComercio) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE Sinpe (
    IdSinpe int NOT NULL AUTO_INCREMENT,
    TelefonoOrigen varchar(10) CHARACTER SET utf8mb4 NOT NULL,
    NombreOrigen varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    TelefonoDestinatario varchar(10) CHARACTER SET utf8mb4 NOT NULL,
    NombreDestinatario varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    Monto decimal(18,2) NOT NULL,
    FechaDeRegistro datetime(6) NOT NULL,
    Descripcion varchar(50) CHARACTER SET utf8mb4,
    Estado tinyint(1) NOT NULL DEFAULT 0,
    CONSTRAINT PK_Sinpe PRIMARY KEY (IdSinpe)
) CHARACTER SET=utf8mb4;


-- si ya tiene la DB creada solo ejecuta esto

USE PrograG7;
ALTER TABLE Cajas ADD COLUMN Descripcion varchar(150) CHARACTER SET utf8mb4, ADD COLUMN TelefonoSINPE varchar(10) CHARACTER SET utf8mb4;

CREATE TABLE Sinpe (
    IdSinpe int NOT NULL AUTO_INCREMENT,
    TelefonoOrigen varchar(10) CHARACTER SET utf8mb4 NOT NULL,
    NombreOrigen varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    TelefonoDestinatario varchar(10) CHARACTER SET utf8mb4 NOT NULL,
    NombreDestinatario varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    Monto decimal(18,2) NOT NULL,
    FechaDeRegistro datetime(6) NOT NULL,
    Descripcion varchar(50) CHARACTER SET utf8mb4,
    Estado tinyint(1) NOT NULL DEFAULT 0,
    CONSTRAINT PK_Sinpe PRIMARY KEY (IdSinpe)
) CHARACTER SET=utf8mb4;