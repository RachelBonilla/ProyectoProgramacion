-- =====================================================
-- TABLA: G7_COMERCIOS
-- =====================================================
CREATE TABLE G7_Comercios (
    IdComercio INT AUTO_INCREMENT PRIMARY KEY,
    Identificacion VARCHAR(30) NOT NULL,
    TipoIdentificacion INT NOT NULL,            -- 1 Física, 2 Jurídica
    Nombre VARCHAR(200) NOT NULL,
    TipoDeComercio INT NOT NULL,                -- 1 Restaurantes, 2 Supermercados, 3 Ferreterías, 4 Otros
    Telefono VARCHAR(20) NOT NULL,
    CorreoElectronico VARCHAR(200) NOT NULL,
    Direccion VARCHAR(500) NOT NULL,
    FechaDeRegistro DATETIME NOT NULL,
    FechaDeModificacion DATETIME NULL,
    Estado TINYINT(1) NOT NULL                  -- 1 Activo, 0 Inactivo
);

-- =====================================================
-- TABLA: G7_USUARIOS
-- =====================================================
CREATE TABLE G7_Usuarios (
    IdUsuario INT AUTO_INCREMENT PRIMARY KEY,
    IdComercio INT NULL,
    IdNetUser CHAR(36) NULL,                    -- Guid
    Nombres VARCHAR(100) NOT NULL,
    PrimerApellido VARCHAR(100) NOT NULL,
    SegundoApellido VARCHAR(100) NOT NULL,
    Identificacion VARCHAR(10) NOT NULL,
    CorreoElectronico VARCHAR(200) NOT NULL,
    FechaDeRegistro DATETIME NOT NULL,
    FechaDeModificacion DATETIME NULL,
    Estado TINYINT(1) NOT NULL,
    CONSTRAINT FK_G7_Usuarios_Comercio
        FOREIGN KEY (IdComercio) REFERENCES G7_Comercios(IdComercio)
);

-- =====================================================
-- TABLA: G7_CAJAS
-- =====================================================
CREATE TABLE G7_Cajas (
    IdCaja INT AUTO_INCREMENT PRIMARY KEY,
    IdComercio INT NOT NULL,
    Nombre VARCHAR(200) NULL,
    Descripcion VARCHAR(500) NULL,
    TelefonoSINPE VARCHAR(20) NULL,
    FechaDeRegistro DATETIME NULL,
    FechaDeModificacion DATETIME NULL,
    Estado TINYINT(1) NOT NULL,
    CONSTRAINT FK_G7_Cajas_Comercio
        FOREIGN KEY (IdComercio) REFERENCES G7_Comercios(IdComercio)
);

-- =====================================================
-- TABLA: G7_SINPES
-- =====================================================
CREATE TABLE G7_Sinpes (
    IdSinpe INT AUTO_INCREMENT PRIMARY KEY,
    CajaId INT NULL,
    TelefonoOrigen VARCHAR(20) NULL,
    NombreOrigen VARCHAR(200) NULL,
    TelefonoDestinatario VARCHAR(20) NULL,
    NombreDestinatario VARCHAR(200) NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Descripcion VARCHAR(500) NULL,
    FechaDeRegistro DATETIME NULL,
    Estado TINYINT(1) NOT NULL,                 -- 1 Sincronizado, 0 No sincronizado
    CONSTRAINT FK_G7_Sinpes_Caja
        FOREIGN KEY (CajaId) REFERENCES G7_Cajas(IdCaja)
);

-- =====================================================
-- TABLA: G7_CONFIGURACIONES_COMERCIO
-- =====================================================
CREATE TABLE G7_ConfiguracionesComercio (
    IdConfiguracion INT AUTO_INCREMENT PRIMARY KEY,
    IdComercio INT NOT NULL,
    TipoConfiguracion INT NOT NULL,             -- 1 Plataforma, 2 Externa, 3 Ambas
    Comision INT NOT NULL,
    FechaDeRegistro DATETIME NOT NULL,
    FechaDeModificacion DATETIME NULL,
    Estado TINYINT(1) NOT NULL,
    CONSTRAINT FK_G7_Configuraciones_Comercio
        FOREIGN KEY (IdComercio) REFERENCES G7_Comercios(IdComercio)
);

-- =====================================================
-- TABLA: G7_REPORTES_MENSUALES
-- =====================================================
CREATE TABLE G7_ReportesMensuales (
    IdReporte INT AUTO_INCREMENT PRIMARY KEY,
    IdComercio INT NOT NULL,
    CantidadDeCajas INT NOT NULL,
    MontoTotalRecaudado DECIMAL(18,2) NOT NULL,
    CantidadDeSINPES INT NOT NULL,
    MontoTotalComision DECIMAL(18,2) NOT NULL,
    FechaDelReporte DATETIME NOT NULL,
    CONSTRAINT FK_G7_Reportes_Comercio
        FOREIGN KEY (IdComercio) REFERENCES G7_Comercios(IdComercio)
);

-- =====================================================
-- TABLA: G7_BITACORA
-- =====================================================
CREATE TABLE G7_Bitacora (
    IdEvento INT AUTO_INCREMENT PRIMARY KEY,
    TablaDeEvento VARCHAR(20) NOT NULL,
    TipoDeEvento VARCHAR(20) NOT NULL,
    FechaDeEvento DATETIME NOT NULL,
    DescripcionDeEvento LONGTEXT NULL,
    StackTrace LONGTEXT NULL,
    DatosAnteriores LONGTEXT NULL,
    DatosPosteriores LONGTEXT NULL
);